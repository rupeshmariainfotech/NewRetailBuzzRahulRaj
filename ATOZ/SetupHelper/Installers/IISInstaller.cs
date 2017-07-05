using Microsoft.Web.Administration;
using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;

namespace SetupHelper.Installers
{
    public class IISInstaller
    {
        private static string SiteName = "RetailBuzz";
        private const string AppPoolName = "RetailBuzzAppPool";
        private static string SiteDeploymentPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Maria Infotech\\Sites\\Retail Buzz");

        public static void Install()
        {
            LogHelper.HighlightText(true, true, true, "IIS Section ");
            if (IsIISExists() == false) //check iis    
            {
                //install iis
                ProcessResult iisetupResult = null;
                string[] featureNames = null;
                //Windows 8 or Higher
                if (IsWindows8OrHigher())
                {
                    featureNames = new[] { "IIS-WebServerRole", "IIS-WebServer", "IIS-CommonHttpFeatures", "IIS-StaticContent", "IIS-DefaultDocument", "IIS-DirectoryBrowsing", "IIS-HttpErrors", "IIS-HttpRedirect", "IIS-ApplicationDevelopment", "IIS-ASPNET", "IIS-NetFxExtensibility", "IIS-ASPNET45", "IIS-NetFxExtensibility45", "IIS-ASP", "IIS-ISAPIExtensions", "IIS-ISAPIFilter", "IIS-ServerSideIncludes", "IIS-Security", "IIS-BasicAuthentication", "IIS-WindowsAuthentication", "IIS-ClientCertificateMappingAuthentication", "IIS-IISCertificateMappingAuthentication", "IIS-URLAuthorization", "IIS-RequestFiltering", "IIS-WebServerManagementTools", "IIS-ManagementScriptingTools", "IIS-ManagementService", "NetFx4Extended-ASPNET45", "IIS-ApplicationInit", "IIS-WebSockets", "IIS-CertProvider", "IIS-ManagementConsole" };
                    iisetupResult = ProcessHelper.Invoke(GetDISMPath(), string.Format("/NoRestart /Online /Enable-Feature /all {0}", string.Join(" ", featureNames.Select(name => string.Format("/FeatureName:{0}", name)))));
                }
                else //Windows 7
                {
                    featureNames = new[] { "IIS-WebServerRole", "IIS-WebServer", "IIS-CommonHttpFeatures", "IIS-StaticContent", "IIS-DefaultDocument", "IIS-DirectoryBrowsing", "IIS-HttpErrors", "IIS-HttpRedirect", "IIS-ApplicationDevelopment", "IIS-ASPNET", "IIS-NetFxExtensibility", "IIS-ASP", "IIS-ISAPIExtensions", "IIS-ISAPIFilter", "IIS-ServerSideIncludes", "IIS-Security", "IIS-BasicAuthentication", "IIS-WindowsAuthentication", "IIS-ClientCertificateMappingAuthentication", "IIS-IISCertificateMappingAuthentication", "IIS-URLAuthorization", "IIS-RequestFiltering", "IIS-WebServerManagementTools", "IIS-ManagementScriptingTools", "IIS-ManagementService", "IIS-ManagementConsole" };
                    iisetupResult = ProcessHelper.Invoke(GetDISMPath(), string.Format("/NoRestart /Online /Enable-Feature {0}", string.Join(" ", featureNames.Select(name => string.Format("/FeatureName:{0}", name)))));
                }

                if (string.IsNullOrEmpty(iisetupResult.Error) == false)
                    throw new Exception("Cannot install IIS on user machine. Please try again.");
            }

            //configure iis                
            if (IsAspNetRegistered() == false)
            {
                ProcessResult configureResult = null;
                //Windows 8 or Higher
                if (IsWindows8OrHigher())
                    configureResult = ProcessHelper.Invoke(GetDISMPath(), "/NoRestart /online /enable-feature /all /FeatureName:IIS-ASPNET45");
                else //Windows 7
                    configureResult = ProcessHelper.Invoke(GetFrameworkPath(), "-i");

                if (string.IsNullOrEmpty(configureResult.Error) == false) throw new Exception("Cannot register asp.net on IIS. Please try again.");
            }

            //configure website
            if (Directory.Exists(SiteDeploymentPath) == false)
                throw new Exception("Site contents are empty. Please try again.");

            //copy content
            using (var iisManager = new ServerManager())
            {
                RemoveSite(iisManager);

                var newPool = iisManager.ApplicationPools.Add(AppPoolName);
                newPool.ManagedRuntimeVersion = "v4.0";

                var newSite = iisManager.Sites.Add(SiteName, "http", "*:8081:", SiteDeploymentPath);
                newSite.ApplicationDefaults.ApplicationPoolName = newPool.Name;
                iisManager.CommitChanges();
            }

        }
        public static void UnInstall()
        {
            using (var iisManager = new ServerManager())
            {
                RemoveSite(iisManager);
                iisManager.CommitChanges();
            }
            try
            {
                if (Directory.Exists(SiteDeploymentPath))
                    new FileHelper().DeleteDirectory(SiteDeploymentPath);
            }
            catch (Exception ex)
            {
                LogHelper.WriteException(true, ex);
            }
        }


        private static bool IsIISExists()
        {
            var result = false;
            using (var componentsKey = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\InetStp", false))
            {
                if (componentsKey != null)
                {
                    int majorVersion = (int)componentsKey.GetValue("MajorVersion", -1);
                    int minorVersion = (int)componentsKey.GetValue("MinorVersion", -1);

                    if (majorVersion < 7 || (majorVersion == 7 && minorVersion < 5)) return false;
                    result = true;
                }
            }
            LogHelper.WriteLine("IIS Installation Status : ", result.ToString());
            return result;
        }

        private static void RemoveSite(ServerManager iisManager)
        {
            var existingSite = iisManager.Sites[SiteName];
            if (existingSite != null)
            {
                existingSite.Stop();
                iisManager.Sites.Remove(existingSite);
            }

            var existingPool = iisManager.ApplicationPools[AppPoolName];
            if (existingPool != null)
            {
                existingPool.Stop();
                iisManager.ApplicationPools.Remove(existingPool);
            }
        }

        private static bool IsAspNetRegistered()
        {
            var result = false;
            using (var iisManager = new ServerManager())
            {
                result = iisManager.ApplicationPools.Count(pool => pool.ManagedRuntimeVersion == "v4.0") > 0;
            }
            LogHelper.WriteLine("ASPNET 4.0 Registration Status : ", result.ToString());
            return result;
        }

        private static string GetDISMPath()
        {
            var dismPath = Path.Combine(Environment.ExpandEnvironmentVariables("%windir%"), "system32", "dism.exe");
            if (Environment.Is64BitOperatingSystem && Environment.Is64BitProcess == false)
            {
                // For 32-bit processes on 64-bit systems, %windir%\system32 folder can only be accessed by specifying %windir%\sysnative folder.
                dismPath = Path.Combine(Environment.ExpandEnvironmentVariables("%windir%"), "sysnative", "dism.exe");
            }
            return dismPath;
        }

        private static string GetFrameworkPath()
        {
            var frameworkPath = Path.Combine(Environment.GetEnvironmentVariable("windir"), @"Microsoft.NET\Framework\v4.0.30319\aspnet_regiis.exe");
            if (Environment.Is64BitOperatingSystem && Environment.Is64BitProcess)
            {
                frameworkPath = Path.Combine(Environment.GetEnvironmentVariable("windir"), @"Microsoft.NET\Framework64\v4.0.30319\aspnet_regiis.exe");
            }
            return frameworkPath;
        }



        private static bool IsWindows8OrHigher()
        {
            var osversion = Environment.OSVersion.Version;
            return osversion.Major > 6 || (osversion.Major == 6 && osversion.Minor > 1);
        }

    }
}

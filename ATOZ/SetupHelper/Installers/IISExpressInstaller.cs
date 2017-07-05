using System;
using System.IO;

namespace SetupHelper.Installers
{
    public class IISExpressInstaller
    {
        private static string SiteDeploymentPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Maria Infotech\\Sites\\Retail Buzz");

        public static void Install()
        {
            LogHelper.HighlightText(true, true, true, "IIS Express Section ");

            //configure website
            if (Directory.Exists(SiteDeploymentPath) == false)
                throw new Exception("Site contents are empty. Please try again.");

            StopIISExpress();
        }
        public static void UnInstall()
        {
            StopIISExpress();
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

        private static void StopIISExpress()
        {
            ProcessHelper.KillProcess("iisexpress.exe");
        }
    }
}

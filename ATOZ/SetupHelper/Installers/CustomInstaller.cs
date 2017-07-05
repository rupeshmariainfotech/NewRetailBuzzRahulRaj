using System;
using System.Collections;
using System.ComponentModel;
using System.IO;

namespace SetupHelper.Installers
{
    [RunInstaller(true)]
    public partial class CustomInstaller : System.Configuration.Install.Installer
    {
        public static string LinkName = "Retail Buzz";
        public static string DeploymentURL = "http://localhost:8081";

        public static string DesktopPath = string.Concat(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "\\", LinkName, ".url");
        public static string ProgramMenuDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Programs), "Maria Infotech\\Retail Buzz");
        public static string ProgramMenuPath = string.Concat(ProgramMenuDirectory, "\\", LinkName, ".url");
        public static string StartupPath = string.Concat(Environment.GetFolderPath(Environment.SpecialFolder.Startup), "\\Start RetailBuzz.url");

        public CustomInstaller()
        {
            InitializeComponent();
        }

        public override void Install(IDictionary stateSaver)
        {
            //System.Diagnostics.Debugger.Launch();
            base.Install(stateSaver);
            try
            {
                NetInstaller.Install();
                LocalDBInstaller.Install();
                IISExpressInstaller.Install();

                CreateShortCuts();
                LogHelper.HighlightText(true, true, true, "END ");
            }
            catch (Exception ex)
            {
                LogHelper.WriteException(true, ex);
                throw ex;
            }

        }

        public override void Uninstall(IDictionary savedState)
        {
            //System.Diagnostics.Debugger.Launch();
            try
            {
                LocalDBInstaller.UnInstall();
                IISExpressInstaller.UnInstall();
                DeleteShortCuts();
            }
            catch (Exception ex)
            {
                LogHelper.WriteException(true, ex);
                throw ex;
            }
            base.Uninstall(savedState);
        }

        private void CreateShortCuts()
        {
            if (Directory.Exists(ProgramMenuDirectory) == false) Directory.CreateDirectory(ProgramMenuDirectory);

            using (StreamWriter writer = new StreamWriter(StartupPath))
            {
                writer.WriteLine("[InternetShortcut]");
                writer.WriteLine("URL=file:///" + Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Maria Infotech\\start.vbs"));
                writer.Flush();
            }

            using (var writer = new StreamWriter(ProgramMenuPath))
            {
                writer.WriteLine("[InternetShortcut]");
                writer.WriteLine("URL=" + DeploymentURL);
                writer.Flush();
            }

            using (var writer = new StreamWriter(DesktopPath))
            {
                writer.WriteLine("[InternetShortcut]");
                writer.WriteLine("URL=" + DeploymentURL);
                writer.Flush();
            }
        }

        private void DeleteShortCuts()
        {
            try
            {
                if (File.Exists(DesktopPath)) File.Delete(DesktopPath);

                if (File.Exists(StartupPath)) File.Delete(StartupPath);

                var deletePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Programs), "Maria Infotech");
                if (Directory.Exists(deletePath)) new FileHelper().DeleteDirectory(deletePath);
            }
            catch (Exception ex)
            {
                LogHelper.WriteException(true, ex);
            }
        }
    }
    
}

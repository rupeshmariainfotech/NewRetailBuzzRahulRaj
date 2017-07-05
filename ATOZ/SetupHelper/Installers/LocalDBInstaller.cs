using Microsoft.Win32;
using System;
using System.Linq;

namespace SetupHelper.Installers
{
    public class LocalDBInstaller
    {
        private static string ServerInstance = "BS";
        private static string DatabaseName = "RetailBuzz";
        private static string SQLLOCALDB = "sqllocaldb";
        private static string SharedInstanceName = "bsshared";


        public static void Install()
        {
            LogHelper.HighlightText(true, true, true, "LocalDB Section ");
            //check sql server localdb
            if (IsLocalDBInstalled())
            {
                var infoResult = ProcessHelper.Invoke(SQLLOCALDB, "i");
                if (string.IsNullOrEmpty(infoResult.Output) == false)
                {
                    //look for our server instance
                    var instances = infoResult.Output.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
                    if (instances.Contains(ServerInstance) == false && CreateLocalDBInstance() == false) throw new Exception("Cannot create localdb instance.");
                }
                else
                    if (CreateLocalDBInstance() == false) throw new Exception("Cannot create localdb instance.");


                //start local db instance
                var startInstanceResult = ProcessHelper.Invoke(SQLLOCALDB, string.Concat("start ", ServerInstance));
                if (string.IsNullOrEmpty(startInstanceResult.Error) == false)
                    throw new Exception("Fail to start localdb instance. Please try again.");

                LogHelper.WriteLine("Localdb Instance Started at ", DateTime.Now.ToShortTimeString());
            }
            else
                throw new Exception("Fail to install localdb. Please install local db first and try again.");


            // create database and initialize it.
            var dbManager = new DatabaseManager();
            if (dbManager.Create(ServerInstance, DatabaseName))
            {
                if (dbManager.InsertData(ServerInstance, DatabaseName) == false)
                    throw new Exception("Cannot initialize database.");

                if (dbManager.CreateUser(ServerInstance, DatabaseName) == false)
                    throw new Exception("Cannot create database user.");
            }
            else
                throw new Exception("Cannot create database.");

        }

        public static void UnInstall()
        {
            var result = new DatabaseManager().Delete(ServerInstance, DatabaseName);
            LogHelper.WriteLine("Database Delete Result : ", result.ToString());

            var startInstanceResult = ProcessHelper.Invoke(SQLLOCALDB, string.Concat("stop ", ServerInstance));
            LogHelper.WriteLine("Database Server Instance Stopped : ", string.IsNullOrEmpty(startInstanceResult.Error).ToString());
        }


        private static bool IsLocalDBInstalled()
        {
            var result = false;
            RegistryKey componentsKey;
            using (componentsKey = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Microsoft SQL Server Local DB\Installed Versions\11.0", false))
            {
                if (componentsKey == null)
                {
                    using (componentsKey = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Microsoft SQL Server Local DB\Installed Versions\12.0", false))
                        result = componentsKey != null;
                }
                else
                    result = true;
            }
            LogHelper.WriteLine("LocalDB Installation Status : ", result.ToString());
            return result;
        }

        private static bool CreateLocalDBInstance()
        {
            //Create instance
            var configureLocalDB = ProcessHelper.Invoke(SQLLOCALDB, string.Concat("create ", ServerInstance));
            if (string.IsNullOrEmpty(configureLocalDB.Error))
            {
                var shareResult = ProcessHelper.Invoke(SQLLOCALDB, string.Concat("share  ", ServerInstance, " ", SharedInstanceName));
                var result = string.IsNullOrEmpty(shareResult.Error);
                LogHelper.WriteLine("Localdb new instance creation : ", result.ToString());
                return result;
            }
            return false;
        }
    }
}

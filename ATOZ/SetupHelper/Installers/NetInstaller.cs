using Microsoft.Win32;
using System;

namespace SetupHelper.Installers
{
    public class NetInstaller
    {
        public static void Install()
        {
            LogHelper.HighlightText(true, true, true, ".NET Section ");
            var netResult = CheckNetVersion(); //check .net
            if (netResult == false) throw new Exception("Invalid .net Framework. Please install .net 4.5 framework and try again.");
        }

        private static bool CheckNetVersion()
        {
            var result = false;
            using (var ndpKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey("SOFTWARE\\Microsoft\\NET Framework Setup\\NDP\\v4\\Full\\"))
            {
                if (ndpKey != null)
                {
                    var releaseKey = ndpKey.GetValue("Release");
                    if (releaseKey != null) result = (((int)releaseKey >= 378389));
                }
            }
            LogHelper.WriteLine(".Net Installation Status : ", result.ToString());
            return result;
        }

        private static bool CheckOSVersion()
        {
            //Windows 7 or Later
            var osversion = Environment.OSVersion.Version;
            if (osversion.Major < 6 || (osversion.Major == 6 && osversion.Minor <= 0)) throw new Exception("This setup requires Windows 7 or later");
            return true;
        }
    }
}

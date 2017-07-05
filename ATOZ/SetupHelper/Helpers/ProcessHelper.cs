using System;
using System.Diagnostics;
using System.Management;

public class ProcessResult
{
    public string Output { get; set; }
    public string Error { get; set; }
}

public class ProcessHelper
{
    public static ProcessResult Invoke(string path, string arguments)
    {
        var result = new ProcessResult();
        try
        {
            using (var process = Process.Start(new ProcessStartInfo { FileName = path, Arguments = arguments, CreateNoWindow = true, WindowStyle = ProcessWindowStyle.Hidden, RedirectStandardOutput = true, UseShellExecute = false, RedirectStandardError = true, Verb = "runas" }))
            {
                process.WaitForExit();
                result.Output = process.StandardOutput.ReadToEnd();
                result.Error = process.StandardError.ReadToEnd();
            }

            if (string.IsNullOrEmpty(result.Output) == false) LogHelper.WriteLine("Command Output", result.Output);
            if (string.IsNullOrEmpty(result.Error) == false) LogHelper.WriteLine("Command Error", result.Error);
        }
        catch (Exception ex)
        {
            LogHelper.WriteException(false, ex);
        }
        return result;
    }

    public static void KillProcess(string name)
    {
        try
        {
            var processList = Process.GetProcessesByName(name);
            if (processList != null && processList.Length > 0)
            {
                foreach (var process in processList)
                {
                    KillProcessAndChildren(process.Id);
                }
            }
        }
        catch (Exception ex)
        {
            LogHelper.WriteException(true, ex);
        }

    }

    private static void KillProcessAndChildren(int pid)
    {
        var searcher = new ManagementObjectSearcher("Select * From Win32_Process Where ParentProcessID=" + pid);
        var moc = searcher.Get();
        foreach (ManagementObject mo in moc)
        {
            KillProcessAndChildren(Convert.ToInt32(mo["ProcessID"]));
        }
        try
        {
            Process proc = Process.GetProcessById(pid);
            proc.Kill();
        }
        catch (ArgumentException ex)
        {
            LogHelper.WriteException(true, ex);
        }
    }

}


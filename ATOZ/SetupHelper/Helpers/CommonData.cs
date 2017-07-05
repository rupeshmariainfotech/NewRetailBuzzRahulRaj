#region ----- Using Library -----
using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
#endregion

#region ----- Enum -----


#endregion

/// <summary>
/// Common Information.
/// </summary>
public class Constants
{
    #region -----  Members and Constants -----
    public const char BLANK = ' ';
    public const char ZERO = '0';
    public const char SLASH = '\\';
    public const char Tab = '\t';
    public const string RootDirectoryForOperations = "Operations";
    public const string WorkingDirectoryForErrorOperations = "/Errors";
    public const string WorkingDirectoryForLogOperations = "/Log";
    public const string DEFAULT_DATETIME_FORMAT = @"dd-MMMM-yyyy  HH:mm:ss.fffffff";


    public readonly static DateTime CurrentUTCDate = DateTime.UtcNow;
    public static string DASH = "".PadRight(80, '-');
    private static string mstrWorkingDirectoryForThisExecution = DateTime.UtcNow.ToString("yyyyMMdd-HHmmss");
    private static string mstrLogPath = string.Concat(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "\\Maria Infotech\\Logs\\", RootDirectoryForOperations, SLASH, mstrWorkingDirectoryForThisExecution, SLASH, "log.txt");
    private static string UTCShortDate = CurrentUTCDate.ToString("yyyyMMdd");
    public static string CreationDate = CurrentUTCDate.ToString("yyMMdd");
    public static string OperationsPath = string.Concat(AppDomain.CurrentDomain.BaseDirectory, RootDirectoryForOperations, SLASH, mstrWorkingDirectoryForThisExecution, SLASH);
    #endregion

    #region -----  Properties -----

    /// <summary>
    /// Gets the working directory for this execution.
    /// </summary>
    /// <value>The working directory for this execution.</value>
    public static string WorkingDirectoryForThisExecution
    {
        get { return mstrWorkingDirectoryForThisExecution; }
    }



    /// <summary>
    /// Gets or sets the log path.
    /// </summary>
    /// <value>The log path.</value>
    public static string LogPath
    {
        get { return mstrLogPath; }
        set { mstrLogPath = value; }
    }

    #endregion
}


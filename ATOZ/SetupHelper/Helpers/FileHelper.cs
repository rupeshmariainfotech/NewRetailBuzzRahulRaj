#region ----- Using Library -----
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
#endregion

#region Enumerations
public enum Overwrite
{
    Never,
    Append,
    Always
}
#endregion

public class FileHelper
{

    #region Constants
    private static string ApplicationPath = AppDomain.CurrentDomain.BaseDirectory;
    private const char SLASH = '\\';
    #endregion

    #region Members

    #endregion

    #region Methods

    /// <summary>
    /// Creates the operation working folder.
    /// </summary>
    /// <returns></returns>
    public bool CreateOperationWorkingFolder()
    {
        bool blnStatus;
        string strApplicationPath = AppDomain.CurrentDomain.BaseDirectory;
        string strWorkingDirectoryForThisExecution = Constants.WorkingDirectoryForThisExecution;
        string strDirectory = String.Concat(strApplicationPath, Constants.RootDirectoryForOperations, "\\", strWorkingDirectoryForThisExecution);

        try
        {
            Directory.CreateDirectory(strDirectory);
            if (Directory.Exists(strDirectory))
            {
                LogHelper.Write("Successfully created operations directory: ", strDirectory);
                blnStatus = true;
            }
            else
            {
                LogHelper.Write(true, "Failed to create operations directory :", strDirectory);
                throw new Exception(string.Concat("Failed to create operations directory: ", strDirectory));
            }
        }
        catch (Exception)
        {
            blnStatus = false;
        }

        return blnStatus;
    }

    /// <summary>
    /// Creates the file.
    /// </summary>
    /// <param name="content">The content.</param>
    /// <param name="targetFilePath">The target file path.</param>
    /// <param name="enmOverwriteExistingFile">The enm overwrite existing file.</param>
    /// <returns></returns>
    public bool CreateFile(string content, string targetFilePath, Overwrite enmOverwriteExistingFile)
    {
        if (string.IsNullOrEmpty(targetFilePath))
            return false;

        bool blnResult;
        FileStream fileStream = null;
        StreamWriter streamWriter = null;
        try
        {
            string strTargetDirectory = Path.GetDirectoryName(targetFilePath);
            if (Directory.Exists(strTargetDirectory) == false)
            {
                Directory.CreateDirectory(strTargetDirectory);
            }

            if (File.Exists(targetFilePath))
            {
                switch (enmOverwriteExistingFile)
                {
                    case Overwrite.Never:
                        return false;
                    case Overwrite.Always:
                        File.Delete(targetFilePath);
                        break;
                }
            }
            fileStream = new FileStream(targetFilePath, FileMode.Append, FileAccess.Write, FileShare.Read);
            streamWriter = new StreamWriter(fileStream);
            streamWriter.Write(content);
            streamWriter.Flush();
            streamWriter.Close();
            blnResult = true;

        }
        catch (Exception)
        {
            blnResult = false;
        }
        finally
        {
            if (streamWriter != null)
                streamWriter.Close();

            if (fileStream != null)
                fileStream.Close();
        }
        return blnResult;
    }

    /// <summary>
    /// Deletes the file.
    /// </summary>
    /// <param name="targetFilePath">The target file path.</param>
    public void DeleteFile(string targetFilePath)
    {
        if (string.IsNullOrEmpty(targetFilePath) == false && File.Exists(targetFilePath))
        {
            try
            {
                File.Delete(targetFilePath);
            }
            catch (Exception)
            {
            }

        }
    }

    public void DeleteDirectory(string path)
    {
        foreach (string directory in Directory.GetDirectories(path))
        {
            DeleteDirectory(directory);
        }

        try
        {
            Directory.Delete(path, true);
        }
        catch (IOException)
        {
            Directory.Delete(path, true);
        }
        catch (UnauthorizedAccessException)
        {
            Directory.Delete(path, true);
        }
    }

    #endregion

}


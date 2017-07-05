#region ----- Using Library -----
using System;
using System.IO;
using System.Text;
#endregion


/// <summary>
/// Logs messages to log file. This class is not thread safe.
/// </summary>
/// <remarks></remarks>
public class LogHelper
{

    #region ----- Enumerations -----
    public enum SeperatorType : byte
    {
        Blank,
        None
    }

    #endregion

    #region ----- Members -----


    private static StringBuilder LogMessages = new StringBuilder(100);

    private static SeperatorType menmSeperator = SeperatorType.Blank;
    private static Int32 mintIndentSize = 0;
    private static char mcharIndentCharacter = '\0';
    private static string mstrHighlighter = Constants.DASH;

    public static SeperatorType Seperator
    {
        get { return menmSeperator; }
        set { menmSeperator = value; }
    }
    public static Int32 IndentSize
    {
        get { return mintIndentSize; }
        set { mintIndentSize = value; }
    }
    public static char IndentCharacter
    {
        get { return mcharIndentCharacter; }
        set { mcharIndentCharacter = value; }
    }
    public static string HighlightCharacter
    {
        get { return mstrHighlighter; }
        set { mstrHighlighter = value; }
    }
    #endregion

    #region ----- Methods -----


    /// <summary>
    /// Writes the specified seperator.
    /// </summary>
    /// <param name="Seperator">The seperator.</param>
    /// <param name="Args">The args.</param>
    public static void Write(SeperatorType Seperator, params string[] Args)
    {
        if (Args != null)
        {
            string strBuff = FormatAndJoinArguments(Seperator, Args);
            LogMessages.AppendLine(strBuff);
            Console.WriteLine(strBuff);
        }
    }

    /// <summary>
    /// Writes the specified args.
    /// </summary>
    /// <param name="Args">The args.</param>
    public static void Write(params string[] Args)
    {
        Write(menmSeperator, Args);
    }


    /// <summary>
    /// Writes the specified BLN save.
    /// </summary>
    /// <param name="blnSave">if set to <c>true</c> [BLN save].</param>
    /// <param name="Args">The args.</param>
    public static void Write(bool blnSave, params string[] Args)
    {
        Write(blnSave, menmSeperator, Args);
    }

    /// <summary>
    /// Append the log file.
    /// </summary>
    /// <param name="blnSave">if set to <c>true</c> [BLN save].</param>
    /// <param name="Args">The args.</param>
    public static void Write(bool blnSave, SeperatorType Seperator, params string[] Args)
    {
        Write(Seperator, Args);
        UpdateLog(blnSave);
    }




    /// <summary>
    /// Writes the line.
    /// </summary>
    public static void WriteLine()
    {
        LogMessages.AppendLine();
        Console.WriteLine();
    }

    /// <summary>
    /// Writes the line.
    /// </summary>
    public static void WriteLine(params string[] Args)
    {
        WriteLine(false, Args);
    }

    /// <summary>
    /// Writes the line.
    /// </summary>
    public static void WriteLine(bool blnSave, params string[] Args)
    {
        Write(blnSave, string.Concat(JoinArgumentsBySeperator(menmSeperator, Args), Environment.NewLine));
    }



    /// <summary>
    /// Writes the specified BLN save.
    /// </summary>
    /// <param name="blnSave">if set to <c>true</c> [BLN save].</param>
    /// <param name="ex">The exception.</param>
    /// <param name="Args">The args.</param>
    public static void WriteException(bool blnSave, Exception ex, params string[] Args)
    {
        if (ex != null)
        {

            Console.BackgroundColor = ConsoleColor.Blue;
            Console.ForegroundColor = ConsoleColor.White;

            char chrCurrentIndentChar = mcharIndentCharacter;

            Write(Args);

            SetIndentation(mintIndentSize + 1, Constants.Tab);

            Write(SeperatorType.None, "Exception Details:", Environment.NewLine, Environment.NewLine, "Message:", Environment.NewLine, ex.Message, Environment.NewLine, Environment.NewLine, "Stack Trace:", Environment.NewLine, ex.StackTrace, Environment.NewLine);

            SetIndentation(mintIndentSize - 1, chrCurrentIndentChar);

            Console.ResetColor();
        }
        else
            Write(Args);

        UpdateLog(blnSave);
    }

    /// <summary>
    /// Highlights the text.
    /// </summary>
    /// <param name="blnSave">if set to <c>true</c> [BLN save].</param>
    /// <param name="blnHighlightContent">if set to <c>true</c> [BLN highlight content].</param>
    /// <param name="blnAppendTimestamp">if set to <c>true</c> [BLN append timestamp].</param>
    /// <param name="Args">The args.</param>
    public static void HighlightText(bool blnSave, bool blnHighlightContent, bool blnAppendTimestamp, params string[] Args)
    {
        HighlightText(blnSave, true, true, blnAppendTimestamp, Args);
    }

    /// <summary>
    /// Highlights the text.
    /// </summary>
    /// <param name="blnSave">if set to <c>true</c> [BLN save].</param>
    /// <param name="blnPrefixHighlight">if set to <c>true</c> [BLN prefix highlight].</param>
    /// <param name="blnSuffixHighlight">if set to <c>true</c> [BLN suffix highlight].</param>
    /// <param name="blnAppendTimestamp">if set to <c>true</c> [BLN append timestamp].</param>
    /// <param name="Args">The args.</param>
    public static void HighlightText(bool blnSave, bool blnPrefixHighlight, bool blnSuffixHighlight, bool blnAppendTimestamp, params string[] Args)
    {
        StringBuilder sb = new StringBuilder(100);

        if (blnPrefixHighlight)
            sb.AppendLine(mstrHighlighter);

        sb.Append(JoinArgumentsBySeperator(menmSeperator, Args));

        if (blnAppendTimestamp)
            sb.Append(DateTime.UtcNow.ToString(Constants.DEFAULT_DATETIME_FORMAT));

        if (blnSuffixHighlight)
            sb.Append(string.Concat(Environment.NewLine, mstrHighlighter));

        Write(blnSave, sb.ToString());

        sb = null;

    }

    /// <summary>
    /// Sets the indentation.
    /// </summary>
    /// <param name="intIndentSize">Size of the int indent.</param>
    /// <param name="chrIndentCharacter">The CHR indent character.</param>
    public static void SetIndentation(Int32 intIndentSize, char chrIndentCharacter)
    {
        mintIndentSize = intIndentSize;
        mcharIndentCharacter = chrIndentCharacter;
    }

    /// <summary>
    /// Resets the indentation.
    /// </summary>
    public static void ResetIndentation()
    {
        mintIndentSize = 0;
        mcharIndentCharacter = '\0';
    }

    #endregion

    #region ----- Support Routines -----
    /// <summary>
    /// Updates the log.
    /// </summary>
    /// <param name="blnSave">if set to <c>true</c> [BLN save].</param>
    private static void UpdateLog(bool blnSave)
    {
        if (blnSave)
        {
            if (new FileHelper().CreateFile(LogMessages.ToString(), Constants.LogPath, Overwrite.Append) == false)
            {
            }
            LogMessages.Length = 0; //Clean memory.
        }
    }

    /// <summary>
    /// Formats the text.
    /// </summary>
    /// <param name="s">The s.</param>
    /// <returns></returns>
    private static string FormatText(string s)
    {
        if (string.IsNullOrEmpty(s) == false)
        {
            string strIndent = string.Empty.PadRight(mintIndentSize, mcharIndentCharacter);
            return s.Insert(0, strIndent).Replace(Environment.NewLine, string.Concat(Environment.NewLine, strIndent));
        }
        return s;
    }

    /// <summary>
    /// Joins the arguments by seperator.
    /// </summary>
    /// <param name="seperatorType">Type of the seperator.</param>
    /// <param name="Args">The args.</param>
    /// <returns></returns>
    private static string JoinArgumentsBySeperator(SeperatorType seperatorType, params string[] Args)
    {
        switch (seperatorType)
        {
            case SeperatorType.Blank:
                return string.Join(Constants.BLANK.ToString(), Args);

            case SeperatorType.None:
                return string.Join(string.Empty, Args);

        }
        return null;
    }

    /// <summary>
    /// Formats the and join arguments.
    /// </summary>
    /// <param name="seperatorType">Type of the seperator.</param>
    /// <param name="Args">The args.</param>
    /// <returns></returns>
    private static string FormatAndJoinArguments(SeperatorType seperatorType, params string[] Args)
    {
        if (mintIndentSize > 0)
            return FormatText(JoinArgumentsBySeperator(seperatorType, Args));
        else
            return JoinArgumentsBySeperator(seperatorType, Args);

    }


    #endregion

}

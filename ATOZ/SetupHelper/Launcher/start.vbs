Dim sitePort
Dim siteClr, iispath,sitePath
Dim fso, wshell

sitePort = 8081
siteClr = "v4.0"

Set fso = CreateObject("Scripting.FileSystemObject")
Set wshell = CreateObject("WScript.Shell")
iispath = fso.GetAbsolutePathName(wshell.ExpandEnvironmentStrings("%AllUsersProfile%") & "\\Maria Infotech\\IIS\\iisexpress.exe")
If fso.FileExists(iispath) = False Then
    WScript.Echo "Couldn't find IIS Express."
    WScript.Quit(1)
End If


sitePath = fso.GetAbsolutePathName(wshell.ExpandEnvironmentStrings("%AllUsersProfile%") & "\\Maria Infotech\\Sites\\Retail Buzz")
If fso.FolderExists(sitePath) = False Then
    WScript.Echo "Couldn't find deployed site path."
    WScript.Quit(1)
End If


wshell.Run """" & iispath & """" & " /systray:false /port:" & sitePort & " /clr:" & siteClr & " /path:""" & sitePath & """", 0, true

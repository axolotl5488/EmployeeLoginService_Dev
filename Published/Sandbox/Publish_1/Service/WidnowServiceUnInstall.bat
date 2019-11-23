@ECHO OFF
 
REM The following directory is for .NET 2.0
set DOTNETFX2=%SystemRoot%\Microsoft.NET\Framework\v4.0.30319

 
echo Uninstalling SharePointListHubService...
echo ---------------------------------------------------
%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe /u "D:\Milan\BatchScript\NKTPLEmployeePunchService\EmployeePunchService.exe"
echo ---------------------------------------------------
echo Done

pause
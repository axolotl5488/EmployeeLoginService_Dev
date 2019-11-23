@ECHO OFF
 
REM The following directory is for .NET 4.0
set DOTNETFX2=%SystemRoot%\Microsoft.NET\Framework\v4.0.30319

 
echo Installing SharePointListHubService...
echo ---------------------------------------------------
%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe /i  "D:\Milan\BatchScript\NKTPLEmployeePunchService\EmployeePunchService.exe"
echo ---------------------------------------------------
echo Done.
pause
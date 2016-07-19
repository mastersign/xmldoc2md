@ECHO OFF

REM --- Download NuGet ---

SET NUGET=%~dp0\NuGet.exe
SET NUGET_URL=https://dist.nuget.org/win-x86-commandline/latest/nuget.exe
SET TEST_RUNNER=%~dp0\packages\NUnit.ConsoleRunner.3.4.1\tools\nunit3-console.exe

IF NOT EXIST "%NUGET%" (
  ECHO.Downloading NuGet...
  SET NUGET_URL=https://dist.nuget.org/win-x86-commandline/latest/nuget.exe
  CALL powershell -NoProfile -ExecutionPolicy Unrestricted -Command "& { $(New-Object System.Net.WebClient).DownloadFile('%NUGET_URL%', '%NUGET%') }"
)

REM --- Restore NuGet Packages ---

CALL "%NUGET%" restore

REM --- Run Tests ---

CALL "%TEST_RUNNER%" "%~dp0\XmlDocParser\bin\Debug\XmlDocParser.dll"

PAUSE
@ECHO OFF
CALL %~dp0\xmldoc2md.cmd '..\out' -Assemblies '..\bin\Library.With.Dot.dll', '%~dp0\..\bin\*.exe' -UrlBase '/ref/clr-api/' -UrlFileNameExtension '/'
PAUSE
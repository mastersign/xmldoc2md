@ECHO OFF
CALL %~dp0\xmldoc2md.cmd -Assemblies '..\bin\Library.With.Dot.dll', '%~dp0\..\bin\*.exe' -TargetPath '..\out'
PAUSE
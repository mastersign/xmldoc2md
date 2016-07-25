@ECHO OFF
CALL %~dp0\xmldoc2md.cmd '..\out' -Assemblies '..\bin\Library.With.Dot.dll', '%~dp0\..\bin\*.exe' ^
  -Title 'Test Library' -Author 'Tobias Kiertscher' -Copyright 'Licensed under CC-BY-4.0' ^
  -MetaDataStyle Hugo -NoTitleHeadline -Footer ^
  -UrlBase '/ref/clr-api/' -UrlFileNameExtension '/'
PAUSE
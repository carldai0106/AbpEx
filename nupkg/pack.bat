@ECHO OFF
SETLOCAL
SET VERSION=1.0.4
SET NUGET=..\tools\nuget.exe

FOR %%G IN (nuspec\*.nuspec) DO (
  %NUGET% pack %%G -Symbols -o pkg
)

PAUSE


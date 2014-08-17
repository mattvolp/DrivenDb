@ECHO off

SET VERSION=2.0
SET PATH=Packages\Zero29.0.6.1\tools\;3rd\Ssed

ECHO.
ECHO CURRENT VERSION: %VERSION%
ECHO.

Zero29.exe -a %VERSION%

IF EXIST ".\3rd\NuGet\DrivenDb.nuspec.bak" del /F /Q ".\DrivenDb.nuspec.bak"
IF EXIST ".\build-nuget.bat.bak" del /F /Q ".\build-nuget.bat.bak"

ssed.exe -i.bak "s/<version>.*<\/version>/<version>%VERSION%<\/version>/g" ".\3rd\NuGet\DrivenDb.nuspec"
ssed.exe -i.bak "s/DrivenDb\..*\.nupkg/DrivenDb\.%VERSION%\.nupkg/g" ".\build-nuget.bat"

DEL /F /Q ".\3rd\NuGet\DrivenDb.nuspec.bak"
DEL /F /Q ".\build-nuget.bat.bak"

ECHO.
PAUSE

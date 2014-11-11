@ECHO off

SET VERSION=1.15
SET PATH=3rd\Ploeh;3rd\Ssed

ECHO.
ECHO CURRENT VERSION: %VERSION%
ECHO.

Zero29.exe -a %VERSION%

IF EXIST ".\3rd\NuGet\DrivenDb.nuspec.bak" del /F /Q ".\DrivenDb.nuspec.bak"
IF EXIST ".\build-nuget.bat.bak" del /F /Q ".\build-nuget.bat.bak"
IF EXIST ".\release.msbuild.bak" del /F /Q ".\release.msbuild.bak"

ssed.exe -i.bak "s/<version>.*<\/version>/<version>%VERSION%<\/version>/g" ".\3rd\NuGet\DrivenDb.nuspec"
ssed.exe -i.bak "s/DrivenDb\..*\.nupkg/DrivenDb\.%VERSION%\.nupkg/g" ".\build-nuget.bat"
ssed.exe -i.bak "s/DrivenDb\..*\.zip/DrivenDb\.%VERSION%\.zip/g" ".\release.msbuild"

DEL /F /Q ".\3rd\NuGet\DrivenDb.nuspec.bak"
DEL /F /Q ".\build-nuget.bat.bak"
DEL /F /Q ".\release.msbuild.bak"

ECHO.
PAUSE

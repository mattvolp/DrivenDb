@echo off

CALL build-env.bat
SET PATH=.\3rd\NuGet
SET EnableNuGetPackageRestore=true

NuGet Update -self
NuGet Pack DrivenDb.nuspec -OutputDirectory .\Build

echo Press [Enter] to publish 
pause

NuGet Push .\Build\DrivenDb.%VERSION%.nupkg

pause
@echo off

CALL build-env.bat
SET PATH=C:\Windows\Microsoft.NET\Framework64\v4.0.30319;.\3rd\7-Zip

msbuild.exe DrivenDb-2012.sln /t:rebuild /p:Configuration=Release;Targets=Clean "/p:Platform=Any CPU"

del /S /F /Q Build

copy ".\DrivenDb\bin\Release\DrivenDb.dll" ".\Build\DrivenDb.dll"
copy ".\DrivenDb\bin\Release\DrivenDb.pdb" ".\Build\DrivenDb.pdb"
copy ".\DrivenDbConsole\bin\Release\DrivenDbConsole.exe" ".\Build\DrivenDbConsole.exe"
copy ".\DrivenDbConsole\bin\Release\DrivenDbConsole.pdb" ".\Build\DrivenDbConsole.pdb"
copy ".\DrivenDbConsole\bin\Release\System.Data.SQLite.dll" ".\Build\System.Data.SQLite.dll"
copy ".\Portable\DrivenDbd\bin\Release\DrivenDbd.dll" ".\Build\Portable\DrivenDbd.dll"
copy ".\Portable\DrivenDbd\bin\Release\DrivenDbd.pdb" ".\Build\Portable\DrivenDbd.pdb"
copy ".\Portable\DrivenDbp\bin\Release\DrivenDbp.dll" ".\Build\Portable\DrivenDbp.dll"
copy ".\Portable\DrivenDbp\bin\Release\DrivenDbp.pdb" ".\Build\Portable\DrivenDbp.pdb"

7z.exe a -tzip -mx=9 ".\Build\Build-%VERSION%-fx40.zip" ".\Build\*" 

pause
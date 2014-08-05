@echo off

SET VERSION=1.12.0.0
SET PATH=.\3rd\Ssed

echo VERSION: %VERSION%

REM IF EXIST ".\Portable\DrivenDbd\Properties\AssemblyInfo.cs.bak" del /F /Q ".\Portable\DrivenDbd\Properties\AssemblyInfo.cs.bak"
REM IF EXIST ".\Portable\DrivenDbp\Properties\AssemblyInfo.cs.bak" del /F /Q ".\Portable\DrivenDbp\Properties\AssemblyInfo.cs.bak"
REM IF EXIST ".\DrivenDb\Properties\AssemblyInfo.cs.bak" del /F /Q ".\DrivenDb\Properties\AssemblyInfo.cs.bak"
IF EXIST ".\DrivenDb.nuspec.bak" del /F /Q ".\DrivenDb.nuspec.bak"

REM ssed.exe -i.bak "s/Version(\"".*\"")/Version(\""%VERSION%\"")/g" ".\Portable\DrivenDbd\Properties\AssemblyInfo.cs"
REM ssed.exe -i.bak "s/Version(\"".*\"")/Version(\""%VERSION%\"")/g" ".\Portable\DrivenDbp\Properties\AssemblyInfo.cs"
REM ssed.exe -i.bak "s/Version(\"".*\"")/Version(\""%VERSION%\"")/g" ".\DrivenDb\Properties\AssemblyInfo.cs"
ssed.exe -i.bak "s/<version>.*<\/version>/<version>%VERSION%<\/version>/g" ".\DrivenDb.nuspec"

REM del /F /Q ".\Portable\DrivenDbd\Properties\AssemblyInfo.cs.bak"
REM del /F /Q ".\Portable\DrivenDbp\Properties\AssemblyInfo.cs.bak"
REM del /F /Q ".\DrivenDb\Properties\AssemblyInfo.cs.bak"
del /F /Q ".\DrivenDb.nuspec.bak"

pause


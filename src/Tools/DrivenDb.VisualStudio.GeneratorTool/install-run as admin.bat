@echo off
@echo off

SET INSTALL_PATH=%~dp0
SET ASSEMBLY_NAME=DrivenDb.VisualStudio.GeneratorTool.dll
SET ASSEMBLY_PATH=%INSTALL_PATH%\%ASSEMBLY_NAME%
SET ASSEMBLY_URI=file:///%INSTALL_PATH%\%ASSEMBLY_NAME%
SET DEVENV_PATH=C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\devenv.exe
SET REGISTRY_PATH=install.reg
cls

echo.
echo DrivenDb Custom Entity Generator Setup
echo ----------------------------------------
echo Please close all instances of Visual Studio before proceding!
pause

REM ----------------------------------------
REM REGISTER THE ASSEMBLY
REM ----------------------------------------

echo.
echo.
echo Step 1/4 
echo Registering assembly in the current location...

C:\Windows\Microsoft.NET\Framework\v4.0.30319\regasm.exe /codebase "%ASSEMBLY_PATH%"

echo Type registration complete.

REM ----------------------------------------
REM INSTALL THE ASSEMBLY
REM ----------------------------------------

echo.
echo.
echo Step 2/4 
echo Installing assembly in the current location...

(
echo Windows Registry Editor Version 5.00
echo.
echo [HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Microsoft\VisualStudio\12.0\CLSID\{38d11784-61bd-4afe-86e1-c8f97a20cf70}]
echo "InprocServer32"="C:\\Windows\\System32\\mscoree.dll"
echo "ThreadingModel"="Both"
echo "Class"="DrivenDb.VisualStudio.GeneratorTool.DrivenGenerator"
echo "Assembly"="DrivenDb.VisualStudio.GeneratorTool, Version=0.0.0.0, Culture=neutral, PublicKeyToken=4b494ea44d7b8cc3"
echo "CodeBase"="%ASSEMBLY_URI%"
echo.
echo [HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Microsoft\VisualStudio\12.0\Generators\{fae04ec1-301f-11d3-bf4b-00c04f79efbc}\DrivenGenerator]
echo "CLSID"="{38d11784-61bd-4afe-86e1-c8f97a20cf70}"
echo "GeneratesDesignTimeSource"=dword:00000001
echo @="DrivenDb Schema Generator for C#"
) >%REGISTRY_PATH%

start /wait "" "%REGISTRY_PATH%"
echo Registry installation complete.

REM ----------------------------------------
REM SETUP STUDIO
REM ----------------------------------------

echo.
echo.
echo Step 3/4 
echo Running Visual Studio setup...
echo This may take a few seconds, be patient...

"%DEVENV_PATH%" /setup
REM start /wait "" "%DEVENV_PATH%" /setup

echo Visual Studio setup complete.

REM ----------------------------------------
REM CLEANUP
REM ----------------------------------------

echo.
echo.
echo Step 4/4 
echo Cleaning up...

del %REGISTRY_PATH%

REM ----------------------------------------
REM FINISHED
REM ----------------------------------------

echo Setup complete!
pause

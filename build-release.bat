@ECHO off

SET PATH=C:\Windows\Microsoft.NET\Framework64\v4.0.30319;C:\Program Files (x86)\Microsoft Visual Studio 12.0\VSSDK\VisualStudioIntegration\Common\Assemblies\v4.0

msbuild.exe build-release.msbuild

ECHO.
PAUSE

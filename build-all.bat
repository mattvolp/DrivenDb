@ECHO off

SET PATH=C:\Windows\Microsoft.NET\Framework64\v4.0.30319;

msbuild.exe build-all.msbuild

ECHO.
PAUSE

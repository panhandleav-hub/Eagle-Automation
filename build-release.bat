@echo off
REM Eagle Automation - Release Build Script
REM Run this on Windows to create a standalone executable

echo Building Eagle Automation for Windows x64...
echo.

cd EagleAutomation

REM Clean previous builds
dotnet clean --configuration Release

REM Restore dependencies
dotnet restore

REM Publish as self-contained Windows x64 application
dotnet publish --configuration Release --runtime win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true --output ..\publish

echo.
echo ========================================
echo Build Complete!
echo ========================================
echo.
echo Executable location:
echo %~dp0publish\EagleAutomation.exe
echo.
echo You can now distribute the entire 'publish' folder.
echo.
pause

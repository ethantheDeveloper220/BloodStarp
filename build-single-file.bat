@echo off
echo ========================================
echo   Voidstrap Single-File Build Script
echo ========================================
echo.

REM Kill all GalaxyStrap/Voidstrap processes
echo [1/4] Killing GalaxyStrap and related processes...
echo.

taskkill /F /IM "GalaxyStrap.exe" 2>nul
if %ERRORLEVEL% EQU 0 (echo     [KILLED] GalaxyStrap.exe) else (echo     [INFO] GalaxyStrap.exe not running)

taskkill /F /IM "Voidstrap.exe" 2>nul
if %ERRORLEVEL% EQU 0 (echo     [KILLED] Voidstrap.exe) else (echo     [INFO] Voidstrap.exe not running)

taskkill /F /IM "Bloodstrap.exe" 2>nul
if %ERRORLEVEL% EQU 0 (echo     [KILLED] Bloodstrap.exe) else (echo     [INFO] Bloodstrap.exe not running)

taskkill /F /IM "Bloxstrap.exe" 2>nul
if %ERRORLEVEL% EQU 0 (echo     [KILLED] Bloxstrap.exe) else (echo     [INFO] Bloxstrap.exe not running)

taskkill /F /IM "RobloxPlayerBeta.exe" 2>nul >nul
taskkill /F /IM "RobloxStudioBeta.exe" 2>nul >nul

echo.
echo [2/4] Waiting for processes to terminate...
timeout /t 2 /nobreak >nul
echo     Done!
echo.

REM Check if dotnet is available
where dotnet >nul 2>nul
if %ERRORLEVEL% NEQ 0 (
    echo [ERROR] .NET SDK not found!
    echo Please install .NET 8.0 SDK from https://dotnet.microsoft.com/download
    pause
    exit /b 1
)

echo [3/4] Cleaning previous build...
dotnet clean Bloxstrap\Voidstrap.csproj -c Release >nul 2>nul
echo     Done!
echo.

echo [4/4] Building single-file executable...
echo.
echo     This may take a few minutes...
echo.

REM Build as single file with all dependencies embedded
dotnet publish Bloxstrap\Voidstrap.csproj ^
    -c Release ^
    -r win-x64 ^
    --self-contained true ^
    -p:PublishSingleFile=true ^
    -p:IncludeNativeLibrariesForSelfExtract=true ^
    -p:EnableCompressionInSingleFile=true ^
    -p:DebugType=none ^
    -p:DebugSymbols=false

if %ERRORLEVEL% EQU 0 (
    echo.
    echo ========================================
    echo   BUILD SUCCESSFUL!
    echo ========================================
    echo.
    echo Single-file executable created at:
    echo Bloxstrap\bin\Release\net8.0-windows7.0\win-x64\publish\Voidstrap.exe
    echo.
    echo Opening output folder...
    start explorer.exe "Bloxstrap\bin\Release\net8.0-windows7.0\win-x64\publish"
    echo.
) else (
    echo.
    echo ========================================
    echo   BUILD FAILED!
    echo ========================================
    echo.
    echo Check the error messages above.
    pause
    exit /b 1
)

echo.
pause

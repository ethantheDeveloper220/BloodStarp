# Voidstrap Build Script
# This script builds the Voidstrap project and creates an executable

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Voidstrap Build Script" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Set error action preference
$ErrorActionPreference = "Stop"

# Get the script directory
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$ProjectDir = Join-Path $ScriptDir "Bloxstrap"
$SolutionFile = Join-Path $ScriptDir "Voidstrap.sln"

# Check if solution file exists
if (-not (Test-Path $SolutionFile)) {
    Write-Host "ERROR: Solution file not found at: $SolutionFile" -ForegroundColor Red
    Write-Host "Please ensure you're running this script from the correct directory." -ForegroundColor Yellow
    exit 1
}

Write-Host "Solution found: $SolutionFile" -ForegroundColor Green
Write-Host ""

# Find MSBuild
Write-Host "Locating MSBuild..." -ForegroundColor Yellow

$MSBuildPath = $null

# Try to find Visual Studio 2022
$VSWherePath = "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe"
if (Test-Path $VSWherePath) {
    $VSPath = & $VSWherePath -latest -products * -requires Microsoft.Component.MSBuild -property installationPath
    if ($VSPath) {
        $MSBuildPath = Join-Path $VSPath "MSBuild\Current\Bin\MSBuild.exe"
        if (-not (Test-Path $MSBuildPath)) {
            $MSBuildPath = Join-Path $VSPath "MSBuild\Current\Bin\amd64\MSBuild.exe"
        }
    }
}

# Fallback to older Visual Studio versions
if (-not $MSBuildPath -or -not (Test-Path $MSBuildPath)) {
    $MSBuildPaths = @(
        "${env:ProgramFiles}\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe",
        "${env:ProgramFiles}\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe",
        "${env:ProgramFiles}\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\MSBuild.exe",
        "${env:ProgramFiles(x86)}\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe",
        "${env:ProgramFiles(x86)}\Microsoft Visual Studio\2019\Professional\MSBuild\Current\Bin\MSBuild.exe",
        "${env:ProgramFiles(x86)}\Microsoft Visual Studio\2019\Enterprise\MSBuild\Current\Bin\MSBuild.exe"
    )
    
    foreach ($path in $MSBuildPaths) {
        if (Test-Path $path) {
            $MSBuildPath = $path
            break
        }
    }
}

if (-not $MSBuildPath -or -not (Test-Path $MSBuildPath)) {
    Write-Host "ERROR: MSBuild not found!" -ForegroundColor Red
    Write-Host "Please install Visual Studio 2019 or 2022 with .NET desktop development workload." -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Alternative: Use 'dotnet build' if you have .NET SDK installed:" -ForegroundColor Yellow
    Write-Host "  dotnet build `"$SolutionFile`" -c Release" -ForegroundColor Cyan
    exit 1
}

Write-Host "MSBuild found: $MSBuildPath" -ForegroundColor Green
Write-Host ""

# Restore NuGet packages
Write-Host "Restoring NuGet packages..." -ForegroundColor Yellow
try {
    & $MSBuildPath $SolutionFile /t:Restore /p:Configuration=Release /v:minimal
    Write-Host "NuGet packages restored successfully!" -ForegroundColor Green
} catch {
    Write-Host "WARNING: Failed to restore NuGet packages. Continuing anyway..." -ForegroundColor Yellow
}
Write-Host ""

# Build the project
Write-Host "Building Voidstrap (Release configuration)..." -ForegroundColor Yellow
Write-Host ""

try {
    & $MSBuildPath $SolutionFile /p:Configuration=Release /p:Platform="Any CPU" /v:minimal /m
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host ""
        Write-Host "========================================" -ForegroundColor Green
        Write-Host "  Build Successful!" -ForegroundColor Green
        Write-Host "========================================" -ForegroundColor Green
        Write-Host ""
        
        # Find the output executable
        $OutputExe = Join-Path $ProjectDir "bin\Release\net6.0-windows\Bloxstrap.exe"
        
        if (Test-Path $OutputExe) {
            Write-Host "Executable created at:" -ForegroundColor Cyan
            Write-Host "  $OutputExe" -ForegroundColor White
            Write-Host ""
            
            # Get file size
            $FileSize = (Get-Item $OutputExe).Length
            $FileSizeMB = [math]::Round($FileSize / 1MB, 2)
            Write-Host "File size: $FileSizeMB MB" -ForegroundColor Cyan
            
            # Open the output folder
            Write-Host ""
            Write-Host "Opening output folder..." -ForegroundColor Yellow
            Start-Process "explorer.exe" -ArgumentList "/select,`"$OutputExe`""
        } else {
            Write-Host "WARNING: Executable not found at expected location." -ForegroundColor Yellow
            Write-Host "Check the build output for the actual location." -ForegroundColor Yellow
        }
    } else {
        Write-Host ""
        Write-Host "========================================" -ForegroundColor Red
        Write-Host "  Build Failed!" -ForegroundColor Red
        Write-Host "========================================" -ForegroundColor Red
        Write-Host ""
        Write-Host "Please check the error messages above." -ForegroundColor Yellow
        exit 1
    }
} catch {
    Write-Host ""
    Write-Host "ERROR: Build failed with exception:" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "Press any key to exit..."
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")

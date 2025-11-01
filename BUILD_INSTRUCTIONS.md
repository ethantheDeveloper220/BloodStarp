# Voidstrap Build Instructions

This document explains how to build the Voidstrap executable from source.

## 🎯 Quick Start

### Option 1: Using Build Scripts (Easiest)

1. **Double-click `build.bat`** in the project root directory
   - This will automatically detect and use the best available build method
   - The output executable will be in `Bloxstrap\bin\Release\`

### Option 2: Using PowerShell Script

1. Open PowerShell in the project root directory
2. Run: `.\build.ps1`
3. The script will:
   - Locate MSBuild
   - Restore NuGet packages
   - Build the project
   - Open the output folder

### Option 3: Using .NET CLI

If you have .NET SDK installed:

```bash
dotnet build Voidstrap.sln -c Release
```

### Option 4: Using Visual Studio

1. Open `Voidstrap.sln` in Visual Studio
2. Select **Release** configuration
3. Build → Build Solution (or press `Ctrl+Shift+B`)
4. Find the executable in `Bloxstrap\bin\Release\net6.0-windows\`

## 📋 Prerequisites

You need **ONE** of the following:

### Option A: Visual Studio (Recommended)
- Visual Studio 2019 or 2022
- .NET desktop development workload
- Download: https://visualstudio.microsoft.com/

### Option B: .NET SDK
- .NET 6.0 SDK or later
- Download: https://dotnet.microsoft.com/download

## 🔧 Build Configurations

### Release Build (Recommended for distribution)
```bash
dotnet build Voidstrap.sln -c Release
```
- Optimized for performance
- Smaller file size
- No debug symbols

### Debug Build (For development)
```bash
dotnet build Voidstrap.sln -c Debug
```
- Includes debug symbols
- Easier to debug
- Larger file size

## 📁 Output Locations

After building, find your executable at:

**Release Build:**
```
Bloxstrap\bin\Release\net6.0-windows\Bloxstrap.exe
```

**Debug Build:**
```
Bloxstrap\bin\Debug\net6.0-windows\Bloxstrap.exe
```

## ✨ New Features in This Build

### Froststrap Features Added:
- ✅ **DebugMenu** - Advanced log viewer with search
- ✅ **AdvancedSettingsDialog** - FastFlag editor settings
- ✅ **FlagDialog** - Flag validation and management
- ✅ **RobloxSettingsPage** - Roblox game settings
- ✅ **SquareCard Control** - UI component for settings

### Voidstrap Features (Already Present):
- ✅ **AccountConsole** - Account management
- ✅ **BetterBloxDataCenterConsole** - Data center monitoring
- ✅ **ChatLogs** - In-game chat logging
- ✅ **GamePassConsole** - Game pass management
- ✅ **MusicConsole** - Music control
- ✅ **OutputConsole** - Output monitoring
- ✅ **RPCWindow** - Discord Rich Presence
- ✅ **CursorPreviewDialog** - Cursor customization
- ✅ **FFlagPresetsDialog** - FastFlag presets
- ✅ **FFlagSearchDialog** - FastFlag search

### New Global Settings Dialog:
- 🆕 **GlobalSettingsDialog** - Unified access to all features
- Access via Settings icon in the main menu
- Organized by feature category
- Quick launch buttons for all tools

## 🐛 Troubleshooting

### "MSBuild not found"
- Install Visual Studio with .NET desktop development workload
- OR install .NET SDK

### "NuGet packages failed to restore"
- Check your internet connection
- Try running: `dotnet restore Voidstrap.sln`

### "Build failed with errors"
- Check the error messages in the console
- Ensure all prerequisites are installed
- Try cleaning the solution: `dotnet clean Voidstrap.sln`

### "Executable not found after build"
- Check the build output for the actual location
- Look in both Release and Debug folders
- Ensure the build completed successfully (no errors)

## 📞 Support

If you encounter issues:
1. Check the error messages carefully
2. Ensure all prerequisites are installed
3. Try cleaning and rebuilding
4. Check the project's GitHub issues page

## 🚀 Running the Built Executable

After building, you can run the executable directly:
```
.\Bloxstrap\bin\Release\net6.0-windows\Bloxstrap.exe
```

Or simply double-click the `.exe` file in Windows Explorer.

---

**Note:** This is a merged version combining features from both Froststrap and Voidstrap projects. All features maintain their own separate CS and XAML files for easy maintenance.

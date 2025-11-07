# FastFlag Preset System - New Features

## Overview
This document describes the new preset system and additional features added to the FastFlag Editor in Voidstrap.

## Main Features

### 1. **Load Preset System** 🎯
A comprehensive preset management system that allows users to load pre-configured FastFlag configurations.

#### Features:
- **Built-in Presets**: 4 pre-configured presets optimized for different use cases:
  - Performance Boost: Optimized for better FPS
  - Graphics Quality: Enhanced visual quality
  - Low-End PC: Optimized for low-end hardware
  - Competitive: Settings for competitive gameplay

- **Repository Presets**: Load presets from a GitHub repository
  - Configurable repository URL
  - Auto-refresh on dialog open
  - Fetches JSON files from the repository automatically

- **Settings Tab**: 
  - Configure custom repository URL
  - Manage favorite presets
  - Auto-refresh toggle

- **Safety Features**:
  - Preview preset contents before loading
  - Confirmation dialog before replacing FastFlags
  - Option to clear existing flags or merge
  - Shows count of flags that will be affected

#### How to Use:
1. Click "Load Preset" button in FastFlag Editor
2. Choose between Built-in or Repository presets
3. Select a preset to preview its contents
4. Configure merge/replace options
5. Click "Load Preset" and confirm

### 2. **Import JSON from File** 📁
Import FastFlag configurations directly from JSON files on your computer.

#### Features:
- Supports .json, .txt, and all file types
- Same conflict handling as the existing import system
- Easy file selection dialog

#### How to Use:
1. Click "Import JSON" button
2. Select a JSON file from your computer
3. Confirm any conflicting flags
4. Flags are imported automatically

### 3. **Bulk Edit Operations** ⚡
Perform batch operations on multiple FastFlags at once.

#### Operations Available:
- **Find and Replace**: Replace text in flag names
  - Case-sensitive option
  - Real-time preview of changes
  
- **Multiply Values**: Multiply numeric flag values by a factor
  - Optional filter to target specific flags
  
- **Add to Values**: Add a number to all numeric flag values
  - Optional filter to target specific flags
  
- **Set All to Value**: Set multiple flags to the same value
  - Optional filter to target specific flags
  
- **Delete Matching**: Delete all flags matching a filter

#### Features:
- Real-time preview of all changes
- Shows exactly which flags will be affected
- Confirmation before applying changes
- Filter option for targeted operations

#### How to Use:
1. Click "Bulk Edit" button
2. Select operation type
3. Configure parameters
4. Review preview
5. Click "Apply Changes" and confirm

### 4. **Compare Presets/Backups** 🔍
Compare your current FastFlags with presets or backups to see differences.

#### Features:
- **Four Comparison Views**:
  - Differences: Shows flags with different values
  - Only in Current: Flags unique to your configuration
  - Only in Preset: Flags unique to the preset
  - Statistics: Detailed comparison statistics

- **Statistics Include**:
  - Total flag counts
  - Number of identical flags
  - Number of different values
  - Unique flags in each set
  - Similarity percentage

#### How to Use:
1. Click "Compare" button
2. Select a preset or backup to compare with
3. Review differences in the tabs
4. Check statistics for overview

### 5. **Favorite Presets** ⭐
Mark presets as favorites for quick access.

#### Features:
- Add presets to favorites with one click
- View all favorites in Settings tab
- Remove favorites easily
- Persists across sessions

### 6. **Repository Settings** ⚙️
Configure where presets are loaded from.

#### Settings:
- **Repository URL**: GitHub repository URL for presets
  - Default: `https://github.com/FastFlags/Presets/tree/main/presets`
  - Supports any GitHub repo structure
  
- **Auto-refresh**: Automatically load repository presets when dialog opens
  
- **Favorites Management**: View and manage favorite presets

## Technical Details

### New Files Created:
1. `Bloxstrap/PresetManager.cs` - Core preset management logic
2. `Bloxstrap/UI/Elements/Dialogs/PresetDialog.xaml` - Preset selection UI
3. `Bloxstrap/UI/Elements/Dialogs/PresetDialog.xaml.cs` - Preset dialog logic
4. `Bloxstrap/UI/Elements/Dialogs/BulkEditDialog.xaml` - Bulk edit UI
5. `Bloxstrap/UI/Elements/Dialogs/BulkEditDialog.xaml.cs` - Bulk edit logic
6. `Bloxstrap/UI/Elements/Dialogs/ComparePresetsDialog.xaml` - Compare UI
7. `Bloxstrap/UI/Elements/Dialogs/ComparePresetsDialog.xaml.cs` - Compare logic

### Modified Files:
1. `Bloxstrap/Models/Persistable/State.cs` - Added preset settings
2. `Bloxstrap/UI/Elements/Settings/Pages/FastFlagEditorPage.xaml` - Added new buttons
3. `Bloxstrap/UI/Elements/Settings/Pages/FastFlagEditorPage.xaml.cs` - Added event handlers

### State Properties Added:
```csharp
public string PresetRepositoryUrl { get; set; }
public List<string> FavoritePresets { get; set; }
public bool AutoRefreshPresets { get; set; }
```

## Repository Format

To create a compatible preset repository:

1. Create a GitHub repository
2. Add JSON files with FastFlag configurations
3. Each JSON file should be formatted as:
```json
{
  "FlagName1": "value1",
  "FlagName2": "value2",
  "FlagName3": "value3"
}
```
4. Use the repository URL in the format:
   `https://github.com/username/repo/tree/branch/folder`

## Button Layout

The FastFlag Editor now has the following buttons:
- Add New
- Delete Selected
- Delete All
- Copy All
- Copy Better JSON
- Export JSON
- **Import JSON** (NEW)
- Backups (renamed from "Presets")
- **Load Preset** (NEW - Primary button)
- Find FFlags
- **Bulk Edit** (NEW)
- **Compare** (NEW)
- Show Preset Flags (toggle)

## Safety Features

All new features include:
- ✅ Confirmation dialogs before destructive operations
- ✅ Preview of changes before applying
- ✅ Detailed information about affected flags
- ✅ Ability to cancel operations
- ✅ Error handling with user-friendly messages

## Future Enhancement Ideas

Potential additions for future versions:
- Cloud sync for presets
- Community preset sharing
- Preset versioning
- Scheduled preset switching
- Preset categories and tags
- Export comparison reports
- Undo/redo for bulk operations
- Preset templates with variables

## Notes

- All settings are persisted in the State file
- Repository presets are fetched in real-time (not cached)
- Built-in presets are hardcoded for reliability
- The system is fully compatible with existing backup functionality
- No breaking changes to existing features

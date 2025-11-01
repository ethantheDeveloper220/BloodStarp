# Configuration Backup & Restore Feature

## 🎯 Overview

The Configuration Backup & Restore feature allows users to export and import their complete Voidstrap configuration, making it easy to:
- **Backup** configurations before making changes
- **Transfer** settings between installations
- **Share** configurations with other users
- **Restore** previous configurations

## 📦 What Gets Backed Up

### ✅ FastFlags
- All custom FastFlag configurations
- FastFlag presets
- FastFlag profiles

### ⚙️ Settings
- Application preferences
- UI settings
- Behavior configurations
- Integration settings

### 🎨 Themes
- Custom theme configurations
- Appearance settings
- Color schemes

### 🔧 Mods
- Installed modifications
- Mod configurations
- Mod preferences

## 🚀 How to Use

### Accessing the Feature

1. Open **Global Settings** (Settings icon in main menu)
2. Look for **"⚙️ Configuration Management"** section at the top
3. Click **"Open Backup Manager"**

### Exporting Configuration

1. In the Backup Manager, go to the **"📤 Export Configuration"** section
2. Select what to include:
   - ✅ **Include FastFlags** - Export all FastFlag settings
   - ✅ **Include Settings** - Export application settings
   - ✅ **Include Mods** - Export installed mods
   - ✅ **Include Themes** - Export custom themes
3. Click **"Export Configuration"**
4. Choose where to save the `.voidconfig` file
5. Done! Your configuration is saved

### Importing Configuration

1. In the Backup Manager, go to the **"📥 Import Configuration"** section
2. Configure import options:
   - **Merge with Existing** - Combine with current settings (or replace all)
   - **Create Backup Before Import** - Auto-backup current config first (recommended)
3. Click **"Import Configuration"**
4. Select the `.voidconfig` file to import
5. Restart Voidstrap when prompted (if needed)

## 📁 File Format

Configuration files use the `.voidconfig` extension and are stored as JSON:

```json
{
  "exportDate": "2025-11-01T12:00:00",
  "version": "1.0",
  "includeFastFlags": true,
  "includeSettings": true,
  "includeMods": true,
  "includeThemes": true,
  "fastFlags": { ... },
  "settings": { ... },
  "mods": { ... },
  "themes": { ... }
}
```

## 🔒 Auto-Backup

When **"Create Backup Before Import"** is enabled:
- Automatic backup is created before importing
- Stored in: `%AppData%/Voidstrap/Backups/`
- Named: `AutoBackup_YYYYMMDD_HHMMSS.voidconfig`
- Keeps history of your configurations

## 💡 Use Cases

### 1. **Before Major Changes**
Export your config before making significant changes to settings or FastFlags.

### 2. **Multiple Installations**
Export from one PC and import to another to keep settings synchronized.

### 3. **Sharing Configurations**
Share your optimized settings with friends or the community.

### 4. **Testing Configurations**
Try different configurations and easily restore your previous setup.

### 5. **Fresh Install Recovery**
Quickly restore all your settings after a fresh Voidstrap installation.

## 🎨 Blood Theme Integration

The Configuration Backup dialog features the same blood/red theme as the rest of the application:
- Dark red gradient backgrounds
- Blood-red borders and accents
- Consistent with the overall Voidstrap aesthetic

## 🔧 Technical Details

### File Location
- **Export**: User-selected location
- **Auto-Backups**: `%AppData%/Voidstrap/Backups/`

### Supported Formats
- `.voidconfig` (recommended)
- `.json` (compatible)

### Merge vs Replace
- **Merge Mode**: Combines imported settings with existing ones
- **Replace Mode**: Completely replaces current configuration

## ⚠️ Important Notes

1. **Restart May Be Required**: Some settings require restarting Voidstrap to take effect
2. **Backup First**: Always create a backup before importing unknown configurations
3. **Version Compatibility**: Configs from different Voidstrap versions may not be fully compatible
4. **Manual Backup**: Auto-backups are stored locally; manually export for long-term storage

## 🆘 Troubleshooting

### Import Failed
- Check if the file is a valid `.voidconfig` or `.json` file
- Ensure the file isn't corrupted
- Try importing with "Merge with Existing" enabled

### Missing Settings After Import
- Some settings may require a restart
- Check if the exported config included those settings
- Verify the config file version compatibility

### Can't Find Auto-Backups
- Check: `%AppData%/Voidstrap/Backups/`
- Auto-backups are only created when "Create Backup Before Import" is enabled

## 📝 Future Enhancements

Planned features for future versions:
- Cloud backup integration
- Scheduled automatic backups
- Configuration comparison tool
- Selective import (choose specific settings)
- Configuration templates/presets
- Encrypted backups for sensitive data

---

**Access this feature from:** Global Settings → Configuration Management → Open Backup Manager

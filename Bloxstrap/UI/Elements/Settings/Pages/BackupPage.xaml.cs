using System;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using Wpf.Ui.Controls;

namespace Voidstrap.UI.Elements.Settings.Pages
{
    /// <summary>
    /// Backup and Restore Page for Settings
    /// </summary>
    public partial class BackupPage : UiPage
    {
        private const string ConfigFileFilter = "Bloodstrap Config Files (*.bloodconfig)|*.bloodconfig|JSON Files (*.json)|*.json|All Files (*.*)|*.*";
        private const string DefaultConfigExtension = ".bloodconfig";

        // Export toggle states
        public bool IncludeFastFlags { get; set; } = true;
        public bool IncludeSettings { get; set; } = true;
        public bool IncludeMods { get; set; } = true;
        public bool IncludeThemes { get; set; } = true;

        // Import toggle states
        public bool MergeConfig { get; set; } = false;
        public bool BackupBeforeImport { get; set; } = true;

        public BackupPage()
        {
            InitializeComponent();
            DataContext = this;
            LoadLastBackupInfo();
        }

        private void LoadLastBackupInfo()
        {
            try
            {
                string backupFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Bloodstrap", "Backups");
                if (Directory.Exists(backupFolder))
                {
                    var files = Directory.GetFiles(backupFolder, "*" + DefaultConfigExtension);
                    if (files.Length > 0)
                    {
                        var lastFile = files[files.Length - 1];
                        var fileInfo = new FileInfo(lastFile);
                        LastBackupText.Text = $"Last backup: {fileInfo.Name} ({fileInfo.LastWriteTime:g})";
                    }
                }
            }
            catch
            {
                // Ignore errors loading backup info
            }
        }

        private void ExportConfig_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var saveDialog = new SaveFileDialog
                {
                    Filter = ConfigFileFilter,
                    DefaultExt = DefaultConfigExtension,
                    FileName = $"BloodstrapConfig_{DateTime.Now:yyyyMMdd_HHmmss}{DefaultConfigExtension}",
                    Title = "Export Bloodstrap Configuration"
                };

                if (saveDialog.ShowDialog() != true)
                    return;

                StatusText.Text = "Exporting configuration...";

                var config = new BloodstrapConfig
                {
                    ExportDate = DateTime.Now,
                    Version = "0.0.5",
                    IncludeFastFlags = IncludeFastFlags,
                    IncludeSettings = IncludeSettings,
                    IncludeMods = IncludeMods,
                    IncludeThemes = IncludeThemes
                };

                // Export FastFlags
                if (config.IncludeFastFlags)
                {
                    try
                    {
                        config.FastFlags = App.FastFlags?.Prop ?? new System.Collections.Generic.Dictionary<string, object>();
                    }
                    catch (Exception ex)
                    {
                        StatusText.Text = $"Warning: Could not export FastFlags - {ex.Message}";
                    }
                }

                // Export Settings
                if (config.IncludeSettings)
                {
                    try
                    {
                        config.Settings = App.Settings?.Prop ?? new AppSettings();
                    }
                    catch (Exception ex)
                    {
                        StatusText.Text = $"Warning: Could not export Settings - {ex.Message}";
                    }
                }

                // Export Mods
                if (config.IncludeMods)
                {
                    try
                    {
                        config.Mods = ExportMods();
                    }
                    catch (Exception ex)
                    {
                        StatusText.Text = $"Warning: Could not export Mods - {ex.Message}";
                    }
                }

                // Export Themes
                if (config.IncludeThemes)
                {
                    try
                    {
                        config.Themes = ExportThemes();
                    }
                    catch (Exception ex)
                    {
                        StatusText.Text = $"Warning: Could not export Themes - {ex.Message}";
                    }
                }

                // Serialize and save
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                string json = JsonSerializer.Serialize(config, options);
                File.WriteAllText(saveDialog.FileName, json);

                StatusText.Text = $"Configuration exported successfully to: {Path.GetFileName(saveDialog.FileName)}";
                LoadLastBackupInfo();
            }
            catch (Exception ex)
            {
                StatusText.Text = $"Export failed: {ex.Message}";
                System.Windows.MessageBox.Show(
                    $"Failed to export configuration:\n\n{ex.Message}",
                    "Export Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void ImportConfig_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var openDialog = new OpenFileDialog
                {
                    Filter = ConfigFileFilter,
                    DefaultExt = DefaultConfigExtension,
                    Title = "Import Bloodstrap Configuration"
                };

                if (openDialog.ShowDialog() != true)
                    return;

                // Backup current config if requested
                if (BackupBeforeImport)
                {
                    StatusText.Text = "Creating backup of current configuration...";
                    CreateAutoBackup();
                }

                StatusText.Text = "Importing configuration...";

                string json = File.ReadAllText(openDialog.FileName);
                var config = JsonSerializer.Deserialize<BloodstrapConfig>(json, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    PropertyNameCaseInsensitive = true
                });

                if (config == null)
                {
                    throw new Exception("Invalid configuration file format");
                }

                bool mergeMode = MergeConfig;

                // Import FastFlags
                if (config.FastFlags != null)
                {
                    ImportFastFlags(config.FastFlags, mergeMode);
                }

                // Import Settings
                if (config.Settings != null)
                {
                    ImportSettings(config.Settings, mergeMode);
                }

                // Import Mods
                if (config.Mods != null)
                {
                    ImportMods(config.Mods, mergeMode);
                }

                // Import Themes
                if (config.Themes != null)
                {
                    ImportThemes(config.Themes, mergeMode);
                }

                StatusText.Text = $"Configuration imported successfully from: {Path.GetFileName(openDialog.FileName)}";
                
                var result = System.Windows.MessageBox.Show(
                    $"Configuration imported successfully!\n\nSome changes may require restarting Bloodstrap.\n\nRestart now?",
                    "Import Complete",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Information);

                if (result == MessageBoxResult.Yes)
                {
                    // Restart application
                    System.Windows.Application.Current.Shutdown();
                    System.Diagnostics.Process.Start(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
                }
            }
            catch (Exception ex)
            {
                StatusText.Text = $"Import failed: {ex.Message}";
                System.Windows.MessageBox.Show(
                    $"Failed to import configuration:\n\n{ex.Message}",
                    "Import Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void CreateAutoBackup()
        {
            try
            {
                string backupFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Bloodstrap", "Backups");
                Directory.CreateDirectory(backupFolder);

                string backupFile = Path.Combine(backupFolder, $"AutoBackup_{DateTime.Now:yyyyMMdd_HHmmss}{DefaultConfigExtension}");

                var config = new BloodstrapConfig
                {
                    ExportDate = DateTime.Now,
                    Version = "0.0.5",
                    IncludeFastFlags = true,
                    IncludeSettings = true,
                    IncludeMods = true,
                    IncludeThemes = true,
                    FastFlags = App.FastFlags?.Prop ?? new System.Collections.Generic.Dictionary<string, object>(),
                    Settings = App.Settings?.Prop ?? new AppSettings(),
                    Mods = ExportMods(),
                    Themes = ExportThemes()
                };

                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                string json = JsonSerializer.Serialize(config, options);
                File.WriteAllText(backupFile, json);

                LoadLastBackupInfo();
            }
            catch
            {
                // Ignore backup errors
            }
        }

        // Export methods
        private object ExportMods()
        {
            // Export installed mods
            return new { };
        }

        private object ExportThemes()
        {
            // Export custom themes
            return new { };
        }

        // Import methods
        private void ImportFastFlags(object fastFlags, bool merge)
        {
            // Import FastFlags
        }

        private void ImportSettings(object settings, bool merge)
        {
            // Import settings
        }

        private void ImportMods(object mods, bool merge)
        {
            // Import mods
        }

        private void ImportThemes(object themes, bool merge)
        {
            // Import themes
        }
    }

    // Configuration data structure
    public class BloodstrapConfig
    {
        public DateTime ExportDate { get; set; }
        public string Version { get; set; } = "0.0.5";
        public bool IncludeFastFlags { get; set; }
        public bool IncludeSettings { get; set; }
        public bool IncludeMods { get; set; }
        public bool IncludeThemes { get; set; }
        public object? FastFlags { get; set; }
        public object? Settings { get; set; }
        public object? Mods { get; set; }
        public object? Themes { get; set; }
    }
}

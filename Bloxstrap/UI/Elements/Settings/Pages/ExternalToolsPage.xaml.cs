using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using Wpf.Ui.Controls;
using Voidstrap.Models.Persistable;

namespace Voidstrap.UI.Elements.Settings.Pages
{
    public partial class ExternalToolsPage : UiPage
    {
        public ObservableCollection<ExternalTool> Tools { get; set; }

        public ExternalToolsPage()
        {
            InitializeComponent();
            Tools = new ObservableCollection<ExternalTool>();
            LoadTools();
            ToolsListBox.ItemsSource = Tools;
            UpdateNoToolsVisibility();
        }

        private void LoadTools()
        {
            try
            {
                if (App.Settings.Prop.ExternalTools != null)
                {
                    foreach (var tool in App.Settings.Prop.ExternalTools)
                    {
                        Tools.Add(tool);
                    }
                }
            }
            catch (Exception ex)
            {
                StatusText.Text = $"Error loading tools: {ex.Message}";
            }
        }

        private void SaveTools()
        {
            try
            {
                App.Settings.Prop.ExternalTools = Tools.ToList();
                App.Settings.Save();
                StatusText.Text = $"Saved {Tools.Count} external tool(s).";
            }
            catch (Exception ex)
            {
                StatusText.Text = $"Error saving tools: {ex.Message}";
            }
        }

        private void UpdateNoToolsVisibility()
        {
            NoToolsText.Visibility = Tools.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        private void BrowseTool_Click(object sender, RoutedEventArgs e)
        {
            var openDialog = new OpenFileDialog
            {
                Filter = "Executable Files (*.exe)|*.exe|All Files (*.*)|*.*",
                Title = "Select External Tool"
            };

            if (openDialog.ShowDialog() == true)
            {
                ToolPathTextBox.Text = openDialog.FileName;
                
                // Auto-fill name if empty
                if (string.IsNullOrWhiteSpace(ToolNameTextBox.Text))
                {
                    ToolNameTextBox.Text = Path.GetFileNameWithoutExtension(openDialog.FileName);
                }
            }
        }

        private void AddTool_Click(object sender, RoutedEventArgs e)
        {
            string name = ToolNameTextBox.Text?.Trim();
            string path = ToolPathTextBox.Text?.Trim();
            string arguments = ToolArgumentsTextBox.Text?.Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                StatusText.Text = "Please enter a tool name.";
                return;
            }

            if (string.IsNullOrWhiteSpace(path))
            {
                StatusText.Text = "Please select an executable path.";
                return;
            }

            if (!File.Exists(path))
            {
                StatusText.Text = "The selected file does not exist.";
                return;
            }

            var tool = new ExternalTool
            {
                Name = name,
                Path = path,
                Arguments = arguments ?? ""
            };

            Tools.Add(tool);
            SaveTools();
            UpdateNoToolsVisibility();

            // Clear inputs
            ToolNameTextBox.Text = "";
            ToolPathTextBox.Text = "";
            ToolArgumentsTextBox.Text = "";

            StatusText.Text = $"Added '{name}' successfully!";
        }

        private void RemoveTool_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Wpf.Ui.Controls.Button button && button.Tag is ExternalTool tool)
            {
                Tools.Remove(tool);
                SaveTools();
                UpdateNoToolsVisibility();
                StatusText.Text = $"Removed '{tool.Name}'.";
            }
        }

        private void ToolsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Optional: Handle selection changes if needed
        }
    }
}

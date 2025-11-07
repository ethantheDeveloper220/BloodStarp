using System.Windows;
using System.Windows.Controls;
using System.Text.Json;

namespace Voidstrap.UI.Elements.Dialogs
{
    public partial class PresetDialog : Window
    {
        private readonly PresetManager _presetManager = new();
        private PresetManager.PresetInfo? _selectedPreset;
        private Dictionary<string, object>? _presetContent;

        public MessageBoxResult Result { get; private set; } = MessageBoxResult.Cancel;
        public Dictionary<string, object>? SelectedPresetContent => _presetContent;
        public bool ClearExisting => ClearExistingCheckBox.IsChecked ?? false;
        public bool MergeWithExisting => MergeCheckBox.IsChecked ?? true;

        public PresetDialog()
        {
            InitializeComponent();
            
            // Load settings
            RepositoryUrlTextBox.Text = App.State.Prop.PresetRepositoryUrl;
            AutoRefreshCheckBox.IsChecked = App.State.Prop.AutoRefreshPresets;
            
            // Load favorites
            FavoritesList.ItemsSource = App.State.Prop.FavoritePresets;
        }

        private async void Dialog_Loaded(object sender, RoutedEventArgs e)
        {
            // Load built-in presets
            BuiltInPresetsList.ItemsSource = _presetManager.GetBuiltInPresets();

            // Load repository presets if auto-refresh is enabled
            if (App.State.Prop.AutoRefreshPresets)
            {
                await LoadRepositoryPresetsAsync();
            }
        }

        private async System.Threading.Tasks.Task LoadRepositoryPresetsAsync()
        {
            try
            {
                RefreshButton.IsEnabled = false;
                var presets = await _presetManager.LoadPresetsFromRepositoryAsync(App.State.Prop.PresetRepositoryUrl);
                RepositoryPresetsList.ItemsSource = presets;
            }
            catch (Exception ex)
            {
                Frontend.ShowMessageBox($"Error loading repository presets: {ex.Message}", MessageBoxImage.Error);
            }
            finally
            {
                RefreshButton.IsEnabled = true;
            }
        }

        private async void PresetsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is not ListBox listBox || listBox.SelectedItem is not PresetManager.PresetInfo preset)
            {
                PreviewTextBlock.Text = "Select a preset to preview its contents...";
                return;
            }

            _selectedPreset = preset;
            PreviewTextBlock.Text = "Loading preview...";

            try
            {
                Dictionary<string, object>? content = null;

                if (preset.Url.StartsWith("builtin://"))
                {
                    content = _presetManager.GetBuiltInPresetContent(preset.Url);
                }
                else
                {
                    content = await _presetManager.LoadPresetContentAsync(preset.Url);
                }

                if (content != null)
                {
                    _presetContent = content;
                    var options = new JsonSerializerOptions { WriteIndented = true };
                    PreviewTextBlock.Text = JsonSerializer.Serialize(content, options);
                }
                else
                {
                    PreviewTextBlock.Text = "Failed to load preset content.";
                }
            }
            catch (Exception ex)
            {
                PreviewTextBlock.Text = $"Error loading preset: {ex.Message}";
            }
        }

        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            await LoadRepositoryPresetsAsync();
        }

        private void FavoriteButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Wpf.Ui.Controls.Button button || button.Tag is not PresetManager.PresetInfo preset)
                return;

            if (!App.State.Prop.FavoritePresets.Contains(preset.Name))
            {
                App.State.Prop.FavoritePresets.Add(preset.Name);
                FavoritesList.Items.Refresh();
                Frontend.ShowMessageBox($"Added '{preset.Name}' to favorites!", MessageBoxImage.Information);
            }
            else
            {
                Frontend.ShowMessageBox($"'{preset.Name}' is already in favorites.", MessageBoxImage.Information);
            }
        }

        private void RemoveFavoriteButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Wpf.Ui.Controls.Button button || button.Tag is not string favoriteName)
                return;

            App.State.Prop.FavoritePresets.Remove(favoriteName);
            FavoritesList.Items.Refresh();
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            // Save settings
            App.State.Prop.PresetRepositoryUrl = RepositoryUrlTextBox.Text;
            App.State.Prop.AutoRefreshPresets = AutoRefreshCheckBox.IsChecked ?? true;
            App.State.Save();

            if (_presetContent == null || _presetContent.Count == 0)
            {
                Frontend.ShowMessageBox("Please select a preset to load.", MessageBoxImage.Warning);
                return;
            }

            // Confirm replacement
            var confirmResult = Frontend.ShowMessageBox(
                "Are you sure you want to replace your current FastFlags with this preset?\n\n" +
                $"This will affect {_presetContent.Count} flags.",
                MessageBoxImage.Question,
                MessageBoxButton.YesNo
            );

            if (confirmResult == MessageBoxResult.Yes)
            {
                Result = MessageBoxResult.OK;
                Close();
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            // Save settings even on cancel
            App.State.Prop.PresetRepositoryUrl = RepositoryUrlTextBox.Text;
            App.State.Prop.AutoRefreshPresets = AutoRefreshCheckBox.IsChecked ?? true;
            App.State.Save();

            Result = MessageBoxResult.Cancel;
            Close();
        }
    }
}

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Voidstrap.UI.Elements.Dialogs
{
    public partial class ComparePresetsDialog : Window
    {
        private Dictionary<string, object>? _compareWith;

        public ComparePresetsDialog()
        {
            InitializeComponent();
            CurrentCountText.Text = $"{App.FastFlags.Prop.Count} flags";
        }

        private async void SelectPreset_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new PresetDialog();
            dialog.ShowDialog();

            if (dialog.Result == MessageBoxResult.OK && dialog.SelectedPresetContent != null)
            {
                _compareWith = dialog.SelectedPresetContent;
                PerformComparison();
            }
        }

        private void PerformComparison()
        {
            if (_compareWith == null)
                return;

            var current = App.FastFlags.Prop;
            var differences = new List<string>();
            var onlyInCurrent = new List<string>();
            var onlyInPreset = new List<string>();
            var identical = 0;

            // Find differences and flags only in current
            foreach (var flag in current)
            {
                if (_compareWith.ContainsKey(flag.Key))
                {
                    var presetValue = _compareWith[flag.Key]?.ToString() ?? "";
                    var currentValue = flag.Value?.ToString() ?? "";

                    if (presetValue != currentValue)
                    {
                        differences.Add($"{flag.Key}:\n  Current: {currentValue}\n  Preset:  {presetValue}");
                    }
                    else
                    {
                        identical++;
                    }
                }
                else
                {
                    onlyInCurrent.Add($"{flag.Key} = {flag.Value}");
                }
            }

            // Find flags only in preset
            foreach (var flag in _compareWith)
            {
                if (!current.ContainsKey(flag.Key))
                {
                    onlyInPreset.Add($"{flag.Key} = {flag.Value}");
                }
            }

            // Update UI
            DifferencesPanel.Children.Clear();

            if (differences.Count == 0)
            {
                var noDiffText = new TextBlock
                {
                    Text = "No differences in common flags!",
                    FontSize = 13,
                    Foreground = new SolidColorBrush(Colors.Green),
                    Margin = new Thickness(0, 0, 0, 8)
                };
                DifferencesPanel.Children.Add(noDiffText);
            }
            else
            {
                var headerText = new TextBlock
                {
                    Text = $"Found {differences.Count} difference(s):",
                    FontWeight = FontWeights.Bold,
                    Margin = new Thickness(0, 0, 0, 16)
                };
                DifferencesPanel.Children.Add(headerText);

                foreach (var diff in differences)
                {
                    var diffText = new TextBlock
                    {
                        Text = diff,
                        FontFamily = new System.Windows.Media.FontFamily("Consolas"),
                        FontSize = 11,
                        Margin = new Thickness(0, 0, 0, 12),
                        TextWrapping = TextWrapping.Wrap
                    };
                    DifferencesPanel.Children.Add(diffText);
                }
            }

            OnlyInCurrentText.Text = onlyInCurrent.Count == 0
                ? "No flags unique to current configuration."
                : string.Join("\n", onlyInCurrent);

            OnlyInPresetText.Text = onlyInPreset.Count == 0
                ? "No flags unique to preset."
                : string.Join("\n", onlyInPreset);

            // Statistics
            var stats = new System.Text.StringBuilder();
            stats.AppendLine($"Current FastFlags: {current.Count}");
            stats.AppendLine($"Preset FastFlags: {_compareWith.Count}");
            stats.AppendLine();
            stats.AppendLine($"Identical: {identical}");
            stats.AppendLine($"Different values: {differences.Count}");
            stats.AppendLine($"Only in current: {onlyInCurrent.Count}");
            stats.AppendLine($"Only in preset: {onlyInPreset.Count}");
            stats.AppendLine();
            
            var totalUnique = current.Count + _compareWith.Count - identical - differences.Count;
            var similarity = current.Count > 0 ? (identical * 100.0 / Math.Max(current.Count, _compareWith.Count)) : 0;
            stats.AppendLine($"Similarity: {similarity:F1}%");

            StatsText.Text = stats.ToString();
        }
    }
}

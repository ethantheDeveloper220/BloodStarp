using System.Windows;
using System.Windows.Controls;

namespace Voidstrap.UI.Elements.Dialogs
{
    public partial class BulkEditDialog : Window
    {
        public MessageBoxResult Result { get; private set; } = MessageBoxResult.Cancel;
        public Dictionary<string, string> Changes { get; private set; } = new();

        public BulkEditDialog()
        {
            InitializeComponent();
            
            FindTextBox.TextChanged += (s, e) => UpdatePreview();
            ReplaceTextBox.TextChanged += (s, e) => UpdatePreview();
            ValueTextBox.TextChanged += (s, e) => UpdatePreview();
            FilterTextBox.TextChanged += (s, e) => UpdatePreview();
        }

        private void OperationComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (OperationComboBox.SelectedIndex == 0)
            {
                FindReplacePanel.Visibility = Visibility.Visible;
                ValueOperationPanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                FindReplacePanel.Visibility = Visibility.Collapsed;
                ValueOperationPanel.Visibility = Visibility.Visible;
            }
            
            UpdatePreview();
        }

        private void UpdatePreview()
        {
            Changes.Clear();
            var preview = new System.Text.StringBuilder();
            int changeCount = 0;

            try
            {
                switch (OperationComboBox.SelectedIndex)
                {
                    case 0: // Find and Replace
                        if (string.IsNullOrEmpty(FindTextBox.Text))
                        {
                            PreviewTextBlock.Text = "Enter text to find...";
                            return;
                        }

                        var comparison = CaseSensitiveCheckBox.IsChecked == true 
                            ? StringComparison.Ordinal 
                            : StringComparison.OrdinalIgnoreCase;

                        foreach (var flag in App.FastFlags.Prop)
                        {
                            if (flag.Key.Contains(FindTextBox.Text, comparison))
                            {
                                var newName = flag.Key.Replace(FindTextBox.Text, ReplaceTextBox.Text ?? "", comparison);
                                Changes[flag.Key] = newName;
                                preview.AppendLine($"{flag.Key} → {newName}");
                                changeCount++;
                            }
                        }
                        break;

                    case 1: // Multiply Values
                        if (!double.TryParse(ValueTextBox.Text, out double multiplier))
                        {
                            PreviewTextBlock.Text = "Enter a valid number...";
                            return;
                        }

                        foreach (var flag in App.FastFlags.Prop)
                        {
                            if (!string.IsNullOrEmpty(FilterTextBox.Text) && 
                                !flag.Key.Contains(FilterTextBox.Text, StringComparison.OrdinalIgnoreCase))
                                continue;

                            if (double.TryParse(flag.Value?.ToString(), out double currentValue))
                            {
                                var newValue = (currentValue * multiplier).ToString();
                                Changes[flag.Key] = newValue;
                                preview.AppendLine($"{flag.Key}: {currentValue} → {newValue}");
                                changeCount++;
                            }
                        }
                        break;

                    case 2: // Add to Values
                        if (!double.TryParse(ValueTextBox.Text, out double addValue))
                        {
                            PreviewTextBlock.Text = "Enter a valid number...";
                            return;
                        }

                        foreach (var flag in App.FastFlags.Prop)
                        {
                            if (!string.IsNullOrEmpty(FilterTextBox.Text) && 
                                !flag.Key.Contains(FilterTextBox.Text, StringComparison.OrdinalIgnoreCase))
                                continue;

                            if (double.TryParse(flag.Value?.ToString(), out double currentValue))
                            {
                                var newValue = (currentValue + addValue).ToString();
                                Changes[flag.Key] = newValue;
                                preview.AppendLine($"{flag.Key}: {currentValue} → {newValue}");
                                changeCount++;
                            }
                        }
                        break;

                    case 3: // Set All to Value
                        foreach (var flag in App.FastFlags.Prop)
                        {
                            if (!string.IsNullOrEmpty(FilterTextBox.Text) && 
                                !flag.Key.Contains(FilterTextBox.Text, StringComparison.OrdinalIgnoreCase))
                                continue;

                            Changes[flag.Key] = ValueTextBox.Text ?? "";
                            preview.AppendLine($"{flag.Key}: {flag.Value} → {ValueTextBox.Text}");
                            changeCount++;
                        }
                        break;

                    case 4: // Delete Matching
                        foreach (var flag in App.FastFlags.Prop)
                        {
                            if (!string.IsNullOrEmpty(FilterTextBox.Text) && 
                                flag.Key.Contains(FilterTextBox.Text, StringComparison.OrdinalIgnoreCase))
                            {
                                Changes[flag.Key] = "DELETE";
                                preview.AppendLine($"DELETE: {flag.Key}");
                                changeCount++;
                            }
                        }
                        break;
                }

                if (changeCount == 0)
                {
                    PreviewTextBlock.Text = "No changes will be made with current settings.";
                }
                else
                {
                    PreviewTextBlock.Text = $"Will affect {changeCount} flag(s):\n\n{preview}";
                }
            }
            catch (Exception ex)
            {
                PreviewTextBlock.Text = $"Error: {ex.Message}";
            }
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            if (Changes.Count == 0)
            {
                Frontend.ShowMessageBox("No changes to apply.", MessageBoxImage.Information);
                return;
            }

            var confirmResult = Frontend.ShowMessageBox(
                $"Are you sure you want to apply these changes to {Changes.Count} flag(s)?",
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
            Result = MessageBoxResult.Cancel;
            Close();
        }
    }
}

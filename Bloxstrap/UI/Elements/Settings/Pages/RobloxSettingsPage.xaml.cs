using Voidstrap.Models;
using Voidstrap.Models.APIs.Config;
using Voidstrap.UI.ViewModels.Settings;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Wpf.Ui.Controls;

namespace Voidstrap.UI.Elements.Settings.Pages
{
    public partial class RobloxSettingsPage : UiPage
    {
        private RobloxSettingsViewModel? _viewModel;

        public RobloxSettingsPage()
        {
            InitializeComponent();
            Loaded += RobloxSettingsPage_Loaded;
        }

        private void RobloxSettingsPage_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                _viewModel = new RobloxSettingsViewModel();
                DataContext = _viewModel;
            }
            catch (Exception ex)
            {
                // Log error if logger is available
                System.Diagnostics.Debug.WriteLine($"RobloxSettingsPage error: {ex.Message}");
                System.Windows.MessageBox.Show($"Failed to load Roblox settings:\n\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ValidateUInt32(object sender, TextCompositionEventArgs e) => e.Handled = !uint.TryParse(e.Text, out _);

        private void ValidateFloat(object sender, TextCompositionEventArgs e) => e.Handled = !float.TryParse(e.Text, out _);
    }
}

using System.Windows;
using Voidstrap.UI.Elements.Base;
using Voidstrap.UI.ViewModels.Dialogs;

namespace Voidstrap.UI.Elements.Dialogs
{
    /// <summary>
    /// Interaction logic for AdvancedSettingsDialog.xaml
    /// </summary>
    public partial class AdvancedSettingsDialog : WpfUiWindow
    {
        public AdvancedSettingViewModel ViewModel { get; } = new();
        public static AdvancedSettingViewModel SharedViewModel { get; private set; } = new();

        public event EventHandler? SettingsSaved;

        public AdvancedSettingsDialog()
        {
            InitializeComponent();
            DataContext = ViewModel;
            SharedViewModel = ViewModel;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Save settings logic here
            SettingsSaved?.Invoke(this, EventArgs.Empty);
            Close();
        }
    }
}

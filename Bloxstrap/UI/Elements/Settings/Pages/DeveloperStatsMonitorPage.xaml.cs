using System;
using System.Windows;
using Wpf.Ui.Controls;

namespace Voidstrap.UI.Elements.Settings.Pages
{
    public partial class DeveloperStatsMonitorPage : UiPage
    {
        private DeveloperStatsMonitorWindow _monitorWindow;

        public DeveloperStatsMonitorPage()
        {
            InitializeComponent();
        }

        private void OpenMonitorButton_Click(object sender, RoutedEventArgs e)
        {
            if (_monitorWindow == null || !_monitorWindow.IsVisible)
            {
                _monitorWindow = new DeveloperStatsMonitorWindow();
                _monitorWindow.Show();
            }
            else
            {
                _monitorWindow.Activate();
            }
        }
    }
}

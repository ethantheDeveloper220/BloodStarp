using System;
using System.Windows;
using Wpf.Ui.Controls;

namespace Voidstrap.UI.Elements.Settings.Pages
{
    public partial class GameOverlayPage : UiPage
    {
        private GameOverlayWindow _overlayWindow;

        public GameOverlayPage()
        {
            InitializeComponent();
        }

        private void LaunchOverlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (_overlayWindow == null || !_overlayWindow.IsVisible)
            {
                _overlayWindow = new GameOverlayWindow();
                _overlayWindow.Show();
                
                LaunchOverlayButton.Content = "Overlay Active";
                LaunchOverlayButton.Appearance = Wpf.Ui.Common.ControlAppearance.Secondary;
                
                _overlayWindow.Closed += (s, args) =>
                {
                    LaunchOverlayButton.Content = "Launch Overlay";
                    LaunchOverlayButton.Appearance = Wpf.Ui.Common.ControlAppearance.Primary;
                };
            }
            else
            {
                _overlayWindow.Activate();
            }
        }
    }
}

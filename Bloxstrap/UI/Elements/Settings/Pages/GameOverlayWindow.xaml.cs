using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Voidstrap.UI.Elements.Settings.Pages
{
    public partial class GameOverlayWindow : Window
    {
        private DispatcherTimer _updateTimer;
        private PerformanceCounter _cpuCounter;
        private Process _robloxProcess;
        private Random _random = new Random();

        public GameOverlayWindow()
        {
            InitializeComponent();
            InitializeOverlay();
        }

        private void InitializeOverlay()
        {
            try
            {
                _cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
                
                _updateTimer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromMilliseconds(500)
                };
                _updateTimer.Tick += UpdateTimer_Tick;
                _updateTimer.Start();

                // Position overlay in top-right corner
                Left = SystemParameters.PrimaryScreenWidth - Width - 20;
                Top = 20;
            }
            catch (Exception ex)
            {
                App.Logger.WriteLine("GameOverlay", $"Error initializing: {ex.Message}");
            }
        }

        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                UpdateStats();
            }
            catch (Exception ex)
            {
                App.Logger.WriteLine("GameOverlay", $"Error updating stats: {ex.Message}");
            }
        }

        private void UpdateStats()
        {
            // FPS (simulated - would need actual game integration)
            var fps = _random.Next(55, 65);
            FpsText.Text = fps.ToString();
            FpsText.Foreground = fps >= 55 ? System.Windows.Media.Brushes.LimeGreen : System.Windows.Media.Brushes.Yellow;

            // Ping (simulated)
            var ping = _random.Next(10, 100);
            PingText.Text = $"{ping} ms";
            PingText.Foreground = ping < 50 ? System.Windows.Media.Brushes.LimeGreen : 
                                  ping < 100 ? System.Windows.Media.Brushes.Yellow : 
                                  System.Windows.Media.Brushes.Red;

            // CPU
            try
            {
                var cpuUsage = _cpuCounter.NextValue();
                CpuText.Text = $"{cpuUsage:F0}%";
                CpuText.Foreground = cpuUsage < 50 ? System.Windows.Media.Brushes.LimeGreen :
                                     cpuUsage < 80 ? System.Windows.Media.Brushes.Yellow :
                                     System.Windows.Media.Brushes.Red;
            }
            catch
            {
                CpuText.Text = "N/A";
            }

            // RAM (Roblox process)
            try
            {
                if (_robloxProcess == null || _robloxProcess.HasExited)
                {
                    var processes = Process.GetProcessesByName("RobloxPlayerBeta");
                    if (processes.Length > 0)
                    {
                        _robloxProcess = processes[0];
                    }
                }

                if (_robloxProcess != null && !_robloxProcess.HasExited)
                {
                    var ramMB = _robloxProcess.WorkingSet64 / (1024.0 * 1024.0);
                    RamText.Text = $"{ramMB:F0} MB";
                    RamText.Foreground = ramMB < 1000 ? System.Windows.Media.Brushes.LimeGreen :
                                         ramMB < 2000 ? System.Windows.Media.Brushes.Yellow :
                                         System.Windows.Media.Brushes.Red;
                }
                else
                {
                    RamText.Text = "N/A";
                }
            }
            catch
            {
                RamText.Text = "N/A";
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            _updateTimer?.Stop();
            _cpuCounter?.Dispose();
            _robloxProcess?.Dispose();
        }
    }
}

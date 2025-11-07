using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Threading;

namespace Voidstrap.UI.Elements.Settings.Pages
{
    public partial class DeveloperStatsMonitorWindow : Window
    {
        private DispatcherTimer _monitorTimer;
        private PerformanceCounter _cpuCounter;
        private PerformanceCounter _ramCounter;
        private bool _isMonitoring = false;

        public DeveloperStatsMonitorWindow()
        {
            InitializeComponent();
            InitializeMonitoring();
        }

        private void InitializeMonitoring()
        {
            try
            {
                _cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
                _ramCounter = new PerformanceCounter("Memory", "Available MBytes");
                
                _monitorTimer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromSeconds(1)
                };
                _monitorTimer.Tick += MonitorTimer_Tick;
            }
            catch (Exception ex)
            {
                StatusText.Text = $"Error initializing: {ex.Message}";
            }
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            StartMonitoring();
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            StopMonitoring();
        }

        private void StartMonitoring()
        {
            if (_isMonitoring) return;

            _isMonitoring = true;
            _monitorTimer?.Start();
            
            StartButton.IsEnabled = false;
            StopButton.IsEnabled = true;
            StatusText.Text = "Monitoring active - Updating every second";
            StatusText.Foreground = System.Windows.Media.Brushes.LimeGreen;
        }

        private void StopMonitoring()
        {
            if (!_isMonitoring) return;

            _isMonitoring = false;
            _monitorTimer?.Stop();
            
            StartButton.IsEnabled = true;
            StopButton.IsEnabled = false;
            StatusText.Text = "Monitoring stopped";
            StatusText.Foreground = (System.Windows.Media.Brush)FindResource("TextFillColorSecondaryBrush");
        }

        private void MonitorTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                UpdateSystemOverview();
                UpdateTopProcesses();
                LastUpdateText.Text = DateTime.Now.ToString("HH:mm:ss");
            }
            catch (Exception ex)
            {
                StatusText.Text = $"Error: {ex.Message}";
            }
        }

        private void UpdateSystemOverview()
        {
            try
            {
                // CPU Usage
                float cpuUsage = _cpuCounter.NextValue();
                TotalCpuProgressBar.Value = cpuUsage;
                TotalCpuText.Text = $"{cpuUsage:F1}%";

                // RAM Usage
                var totalRam = new Microsoft.VisualBasic.Devices.ComputerInfo().TotalPhysicalMemory / (1024.0 * 1024.0);
                float availableRam = _ramCounter.NextValue();
                float usedRam = (float)(totalRam - availableRam);
                float ramPercentage = (usedRam / (float)totalRam) * 100;
                
                TotalRamProgressBar.Value = ramPercentage;
                TotalRamText.Text = $"{ramPercentage:F1}%";

                // Disk Usage
                var driveInfo = new System.IO.DriveInfo("C");
                if (driveInfo.IsReady)
                {
                    double totalSpace = driveInfo.TotalSize / (1024.0 * 1024.0 * 1024.0);
                    double freeSpace = driveInfo.AvailableFreeSpace / (1024.0 * 1024.0 * 1024.0);
                    double usedSpace = totalSpace - freeSpace;
                    double diskPercentage = (usedSpace / totalSpace) * 100;
                    
                    TotalDiskProgressBar.Value = diskPercentage;
                    TotalDiskText.Text = $"{diskPercentage:F1}%";
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error updating system overview: {ex.Message}");
            }
        }

        private void UpdateTopProcesses()
        {
            try
            {
                var processes = Process.GetProcesses()
                    .Where(p => !string.IsNullOrEmpty(p.ProcessName))
                    .ToList();

                // Top 3 CPU consumers (simplified)
                var topCpu = processes
                    .OrderByDescending(p => p.TotalProcessorTime.TotalMilliseconds)
                    .Take(3)
                    .ToList();

                if (topCpu.Count > 0)
                {
                    Cpu1ProcessText.Text = topCpu[0].ProcessName;
                    Cpu1ValueText.Text = "Active";
                }
                if (topCpu.Count > 1)
                {
                    Cpu2ProcessText.Text = topCpu[1].ProcessName;
                    Cpu2ValueText.Text = "Active";
                }
                if (topCpu.Count > 2)
                {
                    Cpu3ProcessText.Text = topCpu[2].ProcessName;
                    Cpu3ValueText.Text = "Active";
                }

                // Top 3 RAM consumers
                var topRam = processes
                    .OrderByDescending(p => p.WorkingSet64)
                    .Take(3)
                    .ToList();

                if (topRam.Count > 0)
                {
                    Ram1ProcessText.Text = topRam[0].ProcessName;
                    Ram1ValueText.Text = $"{topRam[0].WorkingSet64 / (1024.0 * 1024.0):F0} MB";
                }
                if (topRam.Count > 1)
                {
                    Ram2ProcessText.Text = topRam[1].ProcessName;
                    Ram2ValueText.Text = $"{topRam[1].WorkingSet64 / (1024.0 * 1024.0):F0} MB";
                }
                if (topRam.Count > 2)
                {
                    Ram3ProcessText.Text = topRam[2].ProcessName;
                    Ram3ValueText.Text = $"{topRam[2].WorkingSet64 / (1024.0 * 1024.0):F0} MB";
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error updating top processes: {ex.Message}");
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            StopMonitoring();
            
            _cpuCounter?.Dispose();
            _ramCounter?.Dispose();
            _monitorTimer?.Stop();
        }
    }
}

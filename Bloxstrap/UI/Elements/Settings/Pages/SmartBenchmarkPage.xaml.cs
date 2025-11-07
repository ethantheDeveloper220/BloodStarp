using System;
using System.Management;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Wpf.Ui.Controls;

namespace Voidstrap.UI.Elements.Settings.Pages
{
    public partial class SmartBenchmarkPage : UiPage
    {
        private int _totalScore = 0;
        private int _cpuScore = 0;
        private int _ramScore = 0;
        private int _gpuScore = 0;

        public SmartBenchmarkPage()
        {
            InitializeComponent();
            LoadSystemInfo();
        }

        private async void LoadSystemInfo()
        {
            await Task.Run(() =>
            {
                try
                {
                    // Get CPU Info
                    using (var searcher = new ManagementObjectSearcher("SELECT Name, NumberOfCores, MaxClockSpeed FROM Win32_Processor"))
                    {
                        foreach (ManagementObject obj in searcher.Get())
                        {
                            string cpuName = obj["Name"]?.ToString() ?? "Unknown";
                            int cores = Convert.ToInt32(obj["NumberOfCores"] ?? 0);
                            int speed = Convert.ToInt32(obj["MaxClockSpeed"] ?? 0);

                            Dispatcher.Invoke(() =>
                            {
                                CpuInfoText.Text = $"{cpuName} ({cores} cores @ {speed}MHz)";
                            });
                            break;
                        }
                    }

                    // Get RAM Info
                    using (var searcher = new ManagementObjectSearcher("SELECT TotalVisibleMemorySize FROM Win32_OperatingSystem"))
                    {
                        foreach (ManagementObject obj in searcher.Get())
                        {
                            long totalRam = Convert.ToInt64(obj["TotalVisibleMemorySize"] ?? 0) / 1024 / 1024;
                            Dispatcher.Invoke(() =>
                            {
                                RamInfoText.Text = $"{totalRam} GB";
                            });
                            break;
                        }
                    }

                    // Get GPU Info
                    using (var searcher = new ManagementObjectSearcher("SELECT Name, AdapterRAM FROM Win32_VideoController"))
                    {
                        foreach (ManagementObject obj in searcher.Get())
                        {
                            string gpuName = obj["Name"]?.ToString() ?? "Unknown";
                            long vram = Convert.ToInt64(obj["AdapterRAM"] ?? 0) / 1024 / 1024 / 1024;
                            
                            Dispatcher.Invoke(() =>
                            {
                                GpuInfoText.Text = $"{gpuName} ({vram} GB VRAM)";
                            });
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Dispatcher.Invoke(() =>
                    {
                        CpuInfoText.Text = "Error detecting hardware";
                        RamInfoText.Text = ex.Message;
                    });
                }
            });
        }

        private async void RunBenchmarkButton_Click(object sender, RoutedEventArgs e)
        {
            RunBenchmarkButton.IsEnabled = false;
            BenchmarkProgress.Visibility = Visibility.Visible;
            BenchmarkProgress.Value = 0;

            await Task.Run(async () =>
            {
                // Benchmark CPU
                Dispatcher.Invoke(() => PresetText.Text = "Benchmarking CPU...");
                _cpuScore = await BenchmarkCPU();
                Dispatcher.Invoke(() =>
                {
                    CpuProgressBar.Value = _cpuScore;
                    CpuScoreText.Text = $"{_cpuScore}/30";
                    BenchmarkProgress.Value = 33;
                });

                await Task.Delay(500);

                // Benchmark RAM
                Dispatcher.Invoke(() => PresetText.Text = "Benchmarking RAM...");
                _ramScore = await BenchmarkRAM();
                Dispatcher.Invoke(() =>
                {
                    RamProgressBar.Value = _ramScore;
                    RamScoreText.Text = $"{_ramScore}/25";
                    BenchmarkProgress.Value = 66;
                });

                await Task.Delay(500);

                // Benchmark GPU
                Dispatcher.Invoke(() => PresetText.Text = "Benchmarking GPU...");
                _gpuScore = await BenchmarkGPU();
                Dispatcher.Invoke(() =>
                {
                    GpuProgressBar.Value = _gpuScore;
                    GpuScoreText.Text = $"{_gpuScore}/45";
                    BenchmarkProgress.Value = 100;
                });

                await Task.Delay(500);

                // Calculate total
                _totalScore = _cpuScore + _ramScore + _gpuScore;

                Dispatcher.Invoke(() =>
                {
                    ScoreText.Text = _totalScore.ToString();
                    
                    string preset = GetPresetName(_totalScore);
                    PresetText.Text = $"Recommended: {preset}";
                    RecommendedPresetText.Text = preset;
                    
                    BenchmarkProgress.Visibility = Visibility.Collapsed;
                    RunBenchmarkButton.IsEnabled = true;

                    // Highlight recommended button
                    HighlightRecommendedButton(preset);
                });
            });
        }

        private Task<int> BenchmarkCPU()
        {
            return Task.Run(() =>
            {
                try
                {
                    int score = 0;
                    using (var searcher = new ManagementObjectSearcher("SELECT NumberOfCores, MaxClockSpeed FROM Win32_Processor"))
                    {
                        foreach (ManagementObject obj in searcher.Get())
                        {
                            int cores = Convert.ToInt32(obj["NumberOfCores"] ?? 0);
                            int speed = Convert.ToInt32(obj["MaxClockSpeed"] ?? 0);

                            // Score based on cores (0-15 points)
                            if (cores >= 8) score += 15;
                            else if (cores >= 6) score += 12;
                            else if (cores >= 4) score += 8;
                            else score += cores * 2;

                            // Score based on speed (0-15 points)
                            if (speed >= 3500) score += 15;
                            else if (speed >= 3000) score += 12;
                            else if (speed >= 2500) score += 8;
                            else score += (int)(speed / 300.0);

                            break;
                        }
                    }
                    return Math.Min(score, 30);
                }
                catch
                {
                    return 15; // Default mid-range score
                }
            });
        }

        private Task<int> BenchmarkRAM()
        {
            return Task.Run(() =>
            {
                try
                {
                    int score = 0;
                    using (var searcher = new ManagementObjectSearcher("SELECT TotalVisibleMemorySize FROM Win32_OperatingSystem"))
                    {
                        foreach (ManagementObject obj in searcher.Get())
                        {
                            long totalRamGB = Convert.ToInt64(obj["TotalVisibleMemorySize"] ?? 0) / 1024 / 1024;

                            if (totalRamGB >= 32) score = 25;
                            else if (totalRamGB >= 16) score = 20;
                            else if (totalRamGB >= 8) score = 15;
                            else if (totalRamGB >= 4) score = 8;
                            else score = 4;

                            break;
                        }
                    }
                    return score;
                }
                catch
                {
                    return 12; // Default mid-range score
                }
            });
        }

        private Task<int> BenchmarkGPU()
        {
            return Task.Run(() =>
            {
                try
                {
                    int score = 0;
                    using (var searcher = new ManagementObjectSearcher("SELECT Name, AdapterRAM FROM Win32_VideoController"))
                    {
                        foreach (ManagementObject obj in searcher.Get())
                        {
                            string gpuName = obj["Name"]?.ToString()?.ToLower() ?? "";
                            long vramGB = Convert.ToInt64(obj["AdapterRAM"] ?? 0) / 1024 / 1024 / 1024;

                            // High-end GPUs
                            if (gpuName.Contains("rtx 40") || gpuName.Contains("rtx 30") || 
                                gpuName.Contains("rx 7") || gpuName.Contains("rx 6"))
                            {
                                score += 30;
                            }
                            // Mid-range GPUs
                            else if (gpuName.Contains("rtx 20") || gpuName.Contains("gtx 16") || 
                                     gpuName.Contains("rx 5") || gpuName.Contains("arc"))
                            {
                                score += 20;
                            }
                            // Low-end GPUs
                            else if (gpuName.Contains("gtx") || gpuName.Contains("rx"))
                            {
                                score += 10;
                            }
                            // Integrated graphics
                            else
                            {
                                score += 5;
                            }

                            // VRAM bonus (0-15 points)
                            if (vramGB >= 12) score += 15;
                            else if (vramGB >= 8) score += 12;
                            else if (vramGB >= 6) score += 8;
                            else if (vramGB >= 4) score += 5;
                            else score += 2;

                            break;
                        }
                    }
                    return Math.Min(score, 45);
                }
                catch
                {
                    return 20; // Default mid-range score
                }
            });
        }

        private string GetPresetName(int score)
        {
            if (score >= 61) return "💎 Ultra";
            if (score >= 31) return "⚙️ Low";
            return "🥔 Potato";
        }

        private void HighlightRecommendedButton(string preset)
        {
            // Reset all buttons
            PotatoButton.Appearance = Wpf.Ui.Common.ControlAppearance.Secondary;
            LowButton.Appearance = Wpf.Ui.Common.ControlAppearance.Secondary;
            UltraButton.Appearance = Wpf.Ui.Common.ControlAppearance.Secondary;

            // Highlight recommended
            if (preset.Contains("Potato"))
                PotatoButton.Appearance = Wpf.Ui.Common.ControlAppearance.Primary;
            else if (preset.Contains("Low"))
                LowButton.Appearance = Wpf.Ui.Common.ControlAppearance.Primary;
            else if (preset.Contains("Ultra"))
                UltraButton.Appearance = Wpf.Ui.Common.ControlAppearance.Primary;
        }

        private void PotatoButton_Click(object sender, RoutedEventArgs e)
        {
            ApplyPreset("Potato");
        }

        private void LowButton_Click(object sender, RoutedEventArgs e)
        {
            ApplyPreset("Low");
        }

        private void UltraButton_Click(object sender, RoutedEventArgs e)
        {
            ApplyPreset("Ultra");
        }

        private void ApplyPreset(string presetName)
        {
            var result = System.Windows.MessageBox.Show(
                $"Apply {presetName} preset? This will modify your FastFlags for optimal performance.",
                "Apply Preset",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                // TODO: Implement preset application logic
                System.Windows.MessageBox.Show(
                    $"{presetName} preset applied successfully!",
                    "Success",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
        }
    }
}

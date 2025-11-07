using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace Voidstrap.UI
{
    /// <summary>
    /// Easter Eggs System for GalaxyStrap
    /// Includes Konami Code, GALAXY Code, and other hidden features
    /// </summary>
    public class EasterEggs
    {
        private static EasterEggs _instance;
        private Window _targetWindow;
        private List<Key> _keySequence = new List<Key>();
        private DispatcherTimer _resetTimer;
        private DispatcherTimer _rainbowTimer;
        private bool _konamiModeActive = false;
        private bool _galaxyModeActive = false;
        private string _typedText = "";

        // Konami Code: ↑ ↑ ↓ ↓ ← → ← → B A
        private readonly List<Key> _konamiCode = new List<Key>
        {
            Key.Up, Key.Up, Key.Down, Key.Down,
            Key.Left, Key.Right, Key.Left, Key.Right,
            Key.B, Key.A
        };

        // GALAXY text sequence
        private const string GALAXY_CODE = "GALAXY";

        public static EasterEggs Instance => _instance ?? (_instance = new EasterEggs());

        private EasterEggs()
        {
            _resetTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(2)
            };
            _resetTimer.Tick += (s, e) =>
            {
                _keySequence.Clear();
                _typedText = "";
                _resetTimer.Stop();
            };
        }

        public void Initialize(Window window)
        {
            _targetWindow = window;
            _targetWindow.PreviewKeyDown += OnKeyDown;
            _targetWindow.PreviewTextInput += OnTextInput;
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            _keySequence.Add(e.Key);
            _resetTimer.Stop();
            _resetTimer.Start();

            // Check for Konami Code
            if (_keySequence.Count >= _konamiCode.Count)
            {
                var lastKeys = _keySequence.Skip(_keySequence.Count - _konamiCode.Count).ToList();
                if (lastKeys.SequenceEqual(_konamiCode))
                {
                    ActivateKonamiMode();
                    _keySequence.Clear();
                }
            }

            // Limit sequence length
            if (_keySequence.Count > 20)
            {
                _keySequence.RemoveAt(0);
            }
        }

        private void OnTextInput(object sender, TextCompositionEventArgs e)
        {
            _typedText += e.Text.ToUpper();
            _resetTimer.Stop();
            _resetTimer.Start();

            // Check for GALAXY Code
            if (_typedText.Contains(GALAXY_CODE))
            {
                ActivateGalaxyMode();
                _typedText = "";
            }

            // Limit typed text length
            if (_typedText.Length > 20)
            {
                _typedText = _typedText.Substring(_typedText.Length - 20);
            }
        }

        /// <summary>
        /// Activates Konami Code Easter Egg - Rainbow Mode
        /// </summary>
        public void ActivateKonamiMode()
        {
            if (_konamiModeActive) return;

            _konamiModeActive = true;

            MessageBox.Show(
                "🎮 Konami Code Activated! 🌈\n\nRainbow Mode ENABLED!\n\nPress ESC to disable.",
                "Easter Egg Unlocked!",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            StartRainbowMode();
        }

        /// <summary>
        /// Activates GALAXY Code Easter Egg - Power Mode
        /// </summary>
        public void ActivateGalaxyMode()
        {
            if (_galaxyModeActive) return;

            _galaxyModeActive = true;

            MessageBox.Show(
                "⚡ GALAXYSTRAP POWER MODE ⚡\n\nAll signature effects enabled!\n\n" +
                "✨ Glassmorphism\n" +
                "🎭 3D Tilt & Shadows\n" +
                "💫 Neon Glow\n\n" +
                "Press ESC to disable.",
                "Easter Egg Unlocked!",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            ApplyGalaxyEffects();
        }

        private void StartRainbowMode()
        {
            if (_rainbowTimer != null && _rainbowTimer.IsEnabled)
                return;

            _rainbowTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(50)
            };

            double hue = 0;
            _rainbowTimer.Tick += (s, e) =>
            {
                if (!_konamiModeActive)
                {
                    _rainbowTimer.Stop();
                    ResetWindowColors();
                    return;
                }

                hue = (hue + 2) % 360;
                var color = ColorFromHSV(hue, 0.8, 0.9);
                
                if (_targetWindow != null)
                {
                    var brush = new SolidColorBrush(color);
                    
                    // Apply rainbow gradient to window background
                    var gradientBrush = new LinearGradientBrush
                    {
                        StartPoint = new Point(0, 0),
                        EndPoint = new Point(1, 1)
                    };
                    gradientBrush.GradientStops.Add(new GradientStop(color, 0.0));
                    gradientBrush.GradientStops.Add(new GradientStop(ColorFromHSV((hue + 60) % 360, 0.8, 0.9), 0.5));
                    gradientBrush.GradientStops.Add(new GradientStop(ColorFromHSV((hue + 120) % 360, 0.8, 0.9), 1.0));
                    
                    // Apply to window if it has a Background property
                    if (_targetWindow.Background != null)
                    {
                        _targetWindow.Background = gradientBrush;
                    }
                }
            };

            _rainbowTimer.Start();

            // Listen for ESC key to disable
            _targetWindow.PreviewKeyDown += OnEscapePressed;
        }

        private void ApplyGalaxyEffects()
        {
            if (_targetWindow == null) return;

            try
            {
                // Apply glassmorphism effect
                var blurEffect = new System.Windows.Media.Effects.BlurEffect
                {
                    Radius = 10
                };

                // Apply 3D transform
                var transform3D = new ScaleTransform(1.02, 1.02);
                
                // Apply drop shadow
                var dropShadow = new System.Windows.Media.Effects.DropShadowEffect
                {
                    Color = Colors.Cyan,
                    Direction = 320,
                    ShadowDepth = 10,
                    BlurRadius = 20,
                    Opacity = 0.8
                };

                // Animate the window
                var scaleAnimation = new DoubleAnimation
                {
                    From = 1.0,
                    To = 1.02,
                    Duration = TimeSpan.FromSeconds(0.5),
                    AutoReverse = true,
                    RepeatBehavior = RepeatBehavior.Forever
                };

                _targetWindow.Effect = dropShadow;
                _targetWindow.RenderTransform = transform3D;

                // Listen for ESC key to disable
                _targetWindow.PreviewKeyDown += OnEscapePressed;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error applying Galaxy effects: {ex.Message}");
            }
        }

        private void OnEscapePressed(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                DisableAllEffects();
            }
        }

        private void DisableAllEffects()
        {
            _konamiModeActive = false;
            _galaxyModeActive = false;

            _rainbowTimer?.Stop();
            ResetWindowColors();
            
            if (_targetWindow != null)
            {
                _targetWindow.Effect = null;
                _targetWindow.RenderTransform = null;
                _targetWindow.PreviewKeyDown -= OnEscapePressed;
            }
        }

        private void ResetWindowColors()
        {
            if (_targetWindow != null)
            {
                // Reset to default gradient
                var defaultBrush = new LinearGradientBrush
                {
                    StartPoint = new Point(0, 0),
                    EndPoint = new Point(1, 1)
                };
                
                // Use resource colors if available
                try
                {
                    var primaryColor = (Color)_targetWindow.FindResource("WindowBackgroundColorPrimary");
                    var secondaryColor = (Color)_targetWindow.FindResource("WindowBackgroundColorSecondary");
                    var thirdColor = (Color)_targetWindow.FindResource("WindowBackgroundColorThird");
                    
                    defaultBrush.GradientStops.Add(new GradientStop(primaryColor, 0.0));
                    defaultBrush.GradientStops.Add(new GradientStop(secondaryColor, 0.5));
                    defaultBrush.GradientStops.Add(new GradientStop(thirdColor, 1.0));
                }
                catch
                {
                    // Fallback colors
                    defaultBrush.GradientStops.Add(new GradientStop(Color.FromRgb(20, 20, 30), 0.0));
                    defaultBrush.GradientStops.Add(new GradientStop(Color.FromRgb(30, 30, 50), 0.5));
                    defaultBrush.GradientStops.Add(new GradientStop(Color.FromRgb(20, 20, 30), 1.0));
                }
                
                _targetWindow.Background = defaultBrush;
            }
        }

        private Color ColorFromHSV(double hue, double saturation, double value)
        {
            int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            double f = hue / 60 - Math.Floor(hue / 60);

            value = value * 255;
            byte v = Convert.ToByte(value);
            byte p = Convert.ToByte(value * (1 - saturation));
            byte q = Convert.ToByte(value * (1 - f * saturation));
            byte t = Convert.ToByte(value * (1 - (1 - f) * saturation));

            switch (hi)
            {
                case 0: return Color.FromRgb(v, t, p);
                case 1: return Color.FromRgb(q, v, p);
                case 2: return Color.FromRgb(p, v, t);
                case 3: return Color.FromRgb(p, q, v);
                case 4: return Color.FromRgb(t, p, v);
                default: return Color.FromRgb(v, p, q);
            }
        }

        public void Cleanup()
        {
            DisableAllEffects();
            
            if (_targetWindow != null)
            {
                _targetWindow.PreviewKeyDown -= OnKeyDown;
                _targetWindow.PreviewTextInput -= OnTextInput;
            }

            _resetTimer?.Stop();
            _rainbowTimer?.Stop();
        }
    }
}

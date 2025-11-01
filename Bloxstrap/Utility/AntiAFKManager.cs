using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace Voidstrap.Utility
{
    public class AntiAFKManager
    {
        private static Timer? _afkTimer;
        private static Random _random = new Random();
        private static Process? _robloxProcess;
        private static DateTime _lastClickTime = DateTime.MinValue;

        [DllImport("user32.dll")]
        private static extern bool SetCursorPos(int X, int Y);

        [DllImport("user32.dll")]
        private static extern void mouse_event(uint dwFlags, int dx, int dy, uint dwData, UIntPtr dwExtraInfo);

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        private const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
        private const uint MOUSEEVENTF_LEFTUP = 0x0004;

        public static void Start(Process robloxProcess)
        {
            const string LOG_IDENT = "AntiAFKManager::Start";

            if (!App.Settings.Prop.EnableAntiAFK)
                return;

            _robloxProcess = robloxProcess;
            Stop(); // Stop any existing timer

            int intervalMinutes = App.Settings.Prop.AntiAFKInterval;
            
            // Add randomization if enabled
            if (App.Settings.Prop.AntiAFKRandomize)
            {
                int variance = _random.Next(-2, 3); // ±2 minutes
                intervalMinutes += variance;
                intervalMinutes = Math.Max(5, Math.Min(30, intervalMinutes)); // Clamp between 5-30
            }

            int intervalMs = intervalMinutes * 60 * 1000;

            App.Logger.WriteLine(LOG_IDENT, $"Starting Anti-AFK with {intervalMinutes} minute interval");

            _afkTimer = new Timer(PerformAntiAFKAction, null, intervalMs, intervalMs);
        }

        public static void Stop()
        {
            _afkTimer?.Dispose();
            _afkTimer = null;
        }

        private static void PerformAntiAFKAction(object? state)
        {
            const string LOG_IDENT = "AntiAFKManager::PerformAntiAFKAction";

            try
            {
                if (_robloxProcess == null || _robloxProcess.HasExited)
                {
                    App.Logger.WriteLine(LOG_IDENT, "Roblox process not found or has exited");
                    Stop();
                    return;
                }

                // Check if user is actually active (if they've clicked recently, skip)
                if ((DateTime.Now - _lastClickTime).TotalMinutes < 2)
                {
                    App.Logger.WriteLine(LOG_IDENT, "User appears active, skipping Anti-AFK action");
                    return;
                }

                IntPtr windowHandle = _robloxProcess.MainWindowHandle;
                if (windowHandle == IntPtr.Zero)
                {
                    App.Logger.WriteLine(LOG_IDENT, "Could not get Roblox window handle");
                    return;
                }

                // Get window bounds
                if (!GetWindowRect(windowHandle, out RECT rect))
                {
                    App.Logger.WriteLine(LOG_IDENT, "Could not get window rect");
                    return;
                }

                int clickX, clickY;

                if (App.Settings.Prop.AntiAFKRandomPosition)
                {
                    // Click at random position within window
                    int width = rect.Right - rect.Left;
                    int height = rect.Bottom - rect.Top;
                    
                    // Avoid edges (10% margin)
                    int marginX = width / 10;
                    int marginY = height / 10;
                    
                    clickX = rect.Left + marginX + _random.Next(width - 2 * marginX);
                    clickY = rect.Top + marginY + _random.Next(height - 2 * marginY);
                }
                else
                {
                    // Click at center
                    clickX = (rect.Left + rect.Right) / 2;
                    clickY = (rect.Top + rect.Bottom) / 2;
                }

                // Perform click
                SetCursorPos(clickX, clickY);
                Thread.Sleep(50); // Small delay
                mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, UIntPtr.Zero);
                Thread.Sleep(50);
                mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, UIntPtr.Zero);

                _lastClickTime = DateTime.Now;

                App.Logger.WriteLine(LOG_IDENT, $"Anti-AFK click performed at ({clickX}, {clickY})");
            }
            catch (Exception ex)
            {
                App.Logger.WriteLine(LOG_IDENT, $"Error performing Anti-AFK action: {ex.Message}");
            }
        }

        public static void UpdateLastActivity()
        {
            _lastClickTime = DateTime.Now;
        }
    }
}

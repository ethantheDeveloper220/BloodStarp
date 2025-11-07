using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;
using System.Windows.Threading;
using Wpf.Ui.Controls;

namespace Voidstrap.UI.Elements.Settings.Pages
{
    public partial class GameSessionTimerPage : UiPage
    {
        private DispatcherTimer _sessionTimer;
        private DateTime _sessionStartTime;
        private bool _isSessionActive = false;
        private string _currentGameName = "";
        private long _currentPlaceId = 0;
        private ObservableCollection<SessionHistoryEntry> _sessionHistory;
        private string _historyFilePath;
        private TimeSpan _todayTime;
        private TimeSpan _weekTime;
        private TimeSpan _totalTime;

        public GameSessionTimerPage()
        {
            InitializeComponent();
            _historyFilePath = Path.Combine(Paths.Base, "SessionHistory.json");
            _sessionHistory = new ObservableCollection<SessionHistoryEntry>();
            
            _sessionTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _sessionTimer.Tick += SessionTimer_Tick;

            LoadHistory();
            CalculateStatistics();
            
            Unloaded += Page_Unloaded;
        }

        private void LoadHistory()
        {
            try
            {
                if (File.Exists(_historyFilePath))
                {
                    var json = File.ReadAllText(_historyFilePath);
                    var entries = JsonSerializer.Deserialize<SessionHistoryEntry[]>(json);
                    
                    _sessionHistory.Clear();
                    if (entries != null)
                    {
                        foreach (var entry in entries.OrderByDescending(e => e.StartTime))
                        {
                            _sessionHistory.Add(entry);
                        }
                    }
                }

                UpdateHistoryUI();
            }
            catch (Exception ex)
            {
                App.Logger.WriteLine("GameSessionTimer", $"Error loading history: {ex.Message}");
            }
        }

        private void SaveHistory()
        {
            try
            {
                var json = JsonSerializer.Serialize(_sessionHistory.ToArray(), new JsonSerializerOptions
                {
                    WriteIndented = true
                });
                File.WriteAllText(_historyFilePath, json);
            }
            catch (Exception ex)
            {
                App.Logger.WriteLine("GameSessionTimer", $"Error saving history: {ex.Message}");
            }
        }

        private void UpdateHistoryUI()
        {
            SessionHistoryList.ItemsSource = _sessionHistory;
            EmptyHistoryState.Visibility = _sessionHistory.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        private void CalculateStatistics()
        {
            var now = DateTime.Now;
            var today = now.Date;
            var weekStart = today.AddDays(-(int)today.DayOfWeek);

            _todayTime = TimeSpan.Zero;
            _weekTime = TimeSpan.Zero;
            _totalTime = TimeSpan.Zero;

            foreach (var session in _sessionHistory)
            {
                var duration = session.EndTime - session.StartTime;
                _totalTime += duration;

                if (session.StartTime.Date == today)
                {
                    _todayTime += duration;
                }

                if (session.StartTime >= weekStart)
                {
                    _weekTime += duration;
                }
            }

            TodayTimeText.Text = FormatTimeSpan(_todayTime);
            WeekTimeText.Text = FormatTimeSpan(_weekTime);
            TotalTimeText.Text = FormatTimeSpan(_totalTime);
        }

        private string FormatTimeSpan(TimeSpan time)
        {
            if (time.TotalHours >= 1)
                return $"{(int)time.TotalHours}h {time.Minutes}m";
            return $"{time.Minutes}m";
        }

        public void StartSession(long placeId, string gameName)
        {
            if (!EnableTrackingToggle.IsChecked == true)
                return;

            if (_isSessionActive)
                StopSession();

            _isSessionActive = true;
            _sessionStartTime = DateTime.Now;
            _currentGameName = gameName;
            _currentPlaceId = placeId;

            CurrentGameName.Text = gameName;
            CurrentPlaceIdText.Text = $"Place ID: {placeId}";
            SessionStartText.Text = _sessionStartTime.ToString("HH:mm:ss");
            StopSessionButton.IsEnabled = true;

            _sessionTimer.Start();
            App.Logger.WriteLine("GameSessionTimer", $"Session started: {gameName}");
        }

        public void StopSession()
        {
            if (!_isSessionActive)
                return;

            _sessionTimer.Stop();
            _isSessionActive = false;

            var endTime = DateTime.Now;
            var duration = endTime - _sessionStartTime;

            if (AutoSaveToggle.IsChecked == true && duration.TotalMinutes >= 1)
            {
                var entry = new SessionHistoryEntry
                {
                    GameName = _currentGameName,
                    PlaceId = _currentPlaceId,
                    StartTime = _sessionStartTime,
                    EndTime = endTime,
                    Duration = FormatDuration(duration),
                    DatePlayed = _sessionStartTime.ToString("MMM dd, yyyy"),
                    TimeRange = $"{_sessionStartTime:HH:mm} - {endTime:HH:mm}"
                };

                _sessionHistory.Insert(0, entry);
                SaveHistory();
                UpdateHistoryUI();
                CalculateStatistics();
            }

            CurrentSessionTime.Text = "00:00:00";
            CurrentGameName.Text = "No active session";
            CurrentPlaceIdText.Text = "--";
            SessionStartText.Text = "--";
            StopSessionButton.IsEnabled = false;

            App.Logger.WriteLine("GameSessionTimer", $"Session stopped. Duration: {duration}");
        }

        private void SessionTimer_Tick(object sender, EventArgs e)
        {
            if (!_isSessionActive)
                return;

            var elapsed = DateTime.Now - _sessionStartTime;
            CurrentSessionTime.Text = $"{(int)elapsed.TotalHours:D2}:{elapsed.Minutes:D2}:{elapsed.Seconds:D2}";

            // Check for notifications
            if (ShowNotificationsToggle.IsChecked == true)
            {
                var notifyInterval = GetNotifyInterval();
                if (notifyInterval > 0 && elapsed.TotalMinutes % notifyInterval == 0 && elapsed.Seconds == 0)
                {
                    ShowPlaytimeNotification(elapsed);
                }
            }
        }

        private int GetNotifyInterval()
        {
            var selected = ((System.Windows.Controls.ComboBoxItem)NotifyIntervalCombo.SelectedItem)?.Content?.ToString();
            if (selected == "Never") return 0;
            return int.TryParse(selected, out int interval) ? interval : 0;
        }

        private void ShowPlaytimeNotification(TimeSpan elapsed)
        {
            try
            {
                Frontend.ShowBalloonTip(
                    "Session Timer",
                    $"You've been playing {_currentGameName} for {FormatDuration(elapsed)}",
                    System.Windows.Forms.ToolTipIcon.Info,
                    5
                );
            }
            catch { }
        }

        private string FormatDuration(TimeSpan duration)
        {
            if (duration.TotalHours >= 1)
                return $"{(int)duration.TotalHours}h {duration.Minutes}m";
            return $"{duration.Minutes}m {duration.Seconds}s";
        }

        private void StopSessionButton_Click(object sender, RoutedEventArgs e)
        {
            StopSession();
        }

        private void ClearHistoryButton_Click(object sender, RoutedEventArgs e)
        {
            var result = System.Windows.MessageBox.Show(
                "Are you sure you want to clear all session history?",
                "Clear History",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                _sessionHistory.Clear();
                SaveHistory();
                UpdateHistoryUI();
                CalculateStatistics();
            }
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            if (_isSessionActive)
                StopSession();
        }
    }

    public class SessionHistoryEntry
    {
        public string GameName { get; set; } = "";
        public long PlaceId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Duration { get; set; } = "";
        public string DatePlayed { get; set; } = "";
        public string TimeRange { get; set; } = "";
    }
}

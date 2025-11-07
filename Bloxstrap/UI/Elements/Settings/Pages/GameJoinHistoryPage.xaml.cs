using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using Wpf.Ui.Controls;

namespace Voidstrap.UI.Elements.Settings.Pages
{
    public partial class GameJoinHistoryPage : UiPage
    {
        private ObservableCollection<GameHistoryEntry> _historyEntries;
        private string _historyFilePath;

        public GameJoinHistoryPage()
        {
            InitializeComponent();
            _historyFilePath = Path.Combine(Paths.Base, "GameHistory.json");
            _historyEntries = new ObservableCollection<GameHistoryEntry>();
            LoadHistory();
        }

        private void LoadHistory()
        {
            try
            {
                if (File.Exists(_historyFilePath))
                {
                    var json = File.ReadAllText(_historyFilePath);
                    var entries = JsonSerializer.Deserialize<GameHistoryEntry[]>(json);
                    
                    _historyEntries.Clear();
                    if (entries != null)
                    {
                        foreach (var entry in entries.OrderByDescending(e => e.LastPlayedDate))
                        {
                            _historyEntries.Add(entry);
                        }
                    }
                }

                UpdateUI();
            }
            catch (Exception ex)
            {
                App.Logger.WriteLine("GameJoinHistory", $"Error loading history: {ex.Message}");
            }
        }

        private void SaveHistory()
        {
            try
            {
                var json = JsonSerializer.Serialize(_historyEntries.ToArray(), new JsonSerializerOptions
                {
                    WriteIndented = true
                });
                File.WriteAllText(_historyFilePath, json);
            }
            catch (Exception ex)
            {
                App.Logger.WriteLine("GameJoinHistory", $"Error saving history: {ex.Message}");
            }
        }

        private void UpdateUI()
        {
            HistoryList.ItemsSource = _historyEntries;
            CountText.Text = $"{_historyEntries.Count} game{(_historyEntries.Count != 1 ? "s" : "")} in history";
            
            EmptyState.Visibility = _historyEntries.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        public void AddGameToHistory(long placeId, string gameName)
        {
            if (!EnableHistoryToggle.IsChecked == true)
                return;

            try
            {
                var existing = _historyEntries.FirstOrDefault(e => e.PlaceId == placeId);
                if (existing != null)
                {
                    existing.LastPlayedDate = DateTime.Now;
                    existing.LastPlayed = "Just now";
                    existing.PlayCount++;
                }
                else
                {
                    var entry = new GameHistoryEntry
                    {
                        PlaceId = placeId,
                        GameName = gameName,
                        LastPlayedDate = DateTime.Now,
                        LastPlayed = "Just now",
                        PlayCount = 1
                    };
                    _historyEntries.Insert(0, entry);
                }

                // Enforce max entries limit
                var maxEntries = int.Parse(((ComboBoxItem)MaxEntriesCombo.SelectedItem).Content.ToString());
                while (_historyEntries.Count > maxEntries)
                {
                    _historyEntries.RemoveAt(_historyEntries.Count - 1);
                }

                SaveHistory();
                UpdateUI();
            }
            catch (Exception ex)
            {
                App.Logger.WriteLine("GameJoinHistory", $"Error adding game: {ex.Message}");
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadHistory();
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            var result = System.Windows.MessageBox.Show(
                "Are you sure you want to clear all game history?",
                "Clear History",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                _historyEntries.Clear();
                SaveHistory();
                UpdateUI();
            }
        }

        private void RejoinButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Wpf.Ui.Controls.Button button && button.Tag is long placeId)
            {
                try
                {
                    var url = $"roblox://placeId={placeId}";
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = url,
                        UseShellExecute = true
                    });
                    
                    App.Logger.WriteLine("GameJoinHistory", $"Rejoining game: {placeId}");
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(
                        $"Failed to rejoin game: {ex.Message}",
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Wpf.Ui.Controls.Button button && button.Tag is long placeId)
            {
                var entry = _historyEntries.FirstOrDefault(e => e.PlaceId == placeId);
                if (entry != null)
                {
                    _historyEntries.Remove(entry);
                    SaveHistory();
                    UpdateUI();
                }
            }
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var searchText = SearchBox.Text?.ToLower() ?? "";
            
            if (string.IsNullOrWhiteSpace(searchText))
            {
                HistoryList.ItemsSource = _historyEntries;
            }
            else
            {
                var filtered = _historyEntries.Where(entry =>
                    entry.GameName.ToLower().Contains(searchText) ||
                    entry.PlaceId.ToString().Contains(searchText)
                ).ToList();
                
                HistoryList.ItemsSource = filtered;
            }
        }
    }

    public class GameHistoryEntry
    {
        public long PlaceId { get; set; }
        public string GameName { get; set; } = "Unknown Game";
        public DateTime LastPlayedDate { get; set; }
        public string LastPlayed { get; set; } = "";
        public int PlayCount { get; set; }
    }
}

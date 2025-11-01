using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Wpf.Ui.Controls;

namespace Voidstrap.UI.Elements.Settings.Pages
{
    public partial class GameplayToolsPage : UiPage, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public GameplayToolsPage()
        {
            InitializeComponent();
            DataContext = this;
            UpdateStatus();
        }

        // Anti-AFK Properties
        public bool EnableAntiAFK
        {
            get => App.Settings.Prop.EnableAntiAFK;
            set
            {
                App.Settings.Prop.EnableAntiAFK = value;
                OnPropertyChanged();
                UpdateStatus();
            }
        }

        public int AntiAFKInterval
        {
            get => App.Settings.Prop.AntiAFKInterval;
            set
            {
                App.Settings.Prop.AntiAFKInterval = value;
                OnPropertyChanged();
                UpdateStatus();
            }
        }

        public bool AntiAFKRandomize
        {
            get => App.Settings.Prop.AntiAFKRandomize;
            set
            {
                App.Settings.Prop.AntiAFKRandomize = value;
                OnPropertyChanged();
            }
        }

        public bool AntiAFKRandomPosition
        {
            get => App.Settings.Prop.AntiAFKRandomPosition;
            set
            {
                App.Settings.Prop.AntiAFKRandomPosition = value;
                OnPropertyChanged();
            }
        }

        // Auto-Rejoin Properties
        public bool EnableAutoRejoin
        {
            get => App.Settings.Prop.EnableAutoRejoin;
            set
            {
                App.Settings.Prop.EnableAutoRejoin = value;
                OnPropertyChanged();
            }
        }

        public int AutoRejoinDelay
        {
            get => App.Settings.Prop.AutoRejoinDelay;
            set
            {
                App.Settings.Prop.AutoRejoinDelay = value;
                OnPropertyChanged();
            }
        }

        public int MaxRejoinAttempts
        {
            get => App.Settings.Prop.MaxRejoinAttempts;
            set
            {
                App.Settings.Prop.MaxRejoinAttempts = value;
                OnPropertyChanged();
            }
        }

        // Performance Properties
        public bool AutoClearCache
        {
            get => App.Settings.Prop.AutoClearCache;
            set
            {
                App.Settings.Prop.AutoClearCache = value;
                OnPropertyChanged();
            }
        }

        public bool ReduceMotionBlur
        {
            get => App.Settings.Prop.ReduceMotionBlur;
            set
            {
                App.Settings.Prop.ReduceMotionBlur = value;
                OnPropertyChanged();
            }
        }

        public bool BoostLoadingSpeed
        {
            get => App.Settings.Prop.BoostLoadingSpeed;
            set
            {
                App.Settings.Prop.BoostLoadingSpeed = value;
                OnPropertyChanged();
            }
        }

        private void UpdateStatus()
        {
            if (EnableAntiAFK)
            {
                string randomText = AntiAFKRandomize ? " (±2 min variance)" : "";
                AntiAFKStatusText.Text = $"Anti-AFK is ACTIVE - Clicking every {AntiAFKInterval} minutes{randomText}";
            }
            else
            {
                AntiAFKStatusText.Text = "Anti-AFK is disabled";
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

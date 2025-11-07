using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Voidstrap.UI.ViewModels.Settings
{
    public class RobloxSettingsViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<SettingsSection> Sections { get; } = new();

        public event PropertyChangedEventHandler? PropertyChanged;

        private bool _readOnly;

        public RobloxSettingsViewModel()
        {
            // Placeholder - in full implementation this would load from RemoteDataManager
            LoadPlaceholderData();
        }

        private void LoadPlaceholderData()
        {
            // Add placeholder sections
            var section = new SettingsSection
            {
                Title = "Display Settings",
                Controls = new ObservableCollection<SettingsControl>()
            };

            // Add some placeholder controls
            section.Controls.Add(new SettingsControl
            {
                Title = "Example Setting",
                Description = "This is a placeholder",
                Type = ControlType.ToggleSwitch,
                Value = "false"
            });

            Sections.Add(section);
        }

        public bool ReadOnly
        {
            get => _readOnly;
            set
            {
                if (_readOnly != value)
                {
                    _readOnly = value;
                    OnPropertyChanged();
                }
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    // Supporting classes for the ViewModel
    public class SettingsSection
    {
        public string Title { get; set; } = "";
        public ObservableCollection<SettingsControl> Controls { get; set; } = new();
    }

    public class SettingsControl : INotifyPropertyChanged
    {
        private string _value = "";

        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public ControlType Type { get; set; }
        
        public string Value
        {
            get => _value;
            set
            {
                if (_value != value)
                {
                    _value = value;
                    OnPropertyChanged();
                }
            }
        }

        public double MinValue { get; set; }
        public double MaxValue { get; set; } = 100;
        public double Step { get; set; } = 1;
        
        public double TypedValue
        {
            get => double.TryParse(Value, out var v) ? v : 0;
            set => Value = value.ToString();
        }

        public string VectorX
        {
            get
            {
                var parts = Value.Split(',');
                return parts.Length > 0 ? parts[0] : "0";
            }
            set
            {
                var parts = Value.Split(',');
                Value = $"{value},{(parts.Length > 1 ? parts[1] : "0")}";
            }
        }

        public string VectorY
        {
            get
            {
                var parts = Value.Split(',');
                return parts.Length > 1 ? parts[1] : "0";
            }
            set
            {
                var parts = Value.Split(',');
                Value = $"{(parts.Length > 0 ? parts[0] : "0")},{value}";
            }
        }

        public ObservableCollection<ControlOption> Options { get; set; } = new();

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public class ControlOption
    {
        public string DisplayName { get; set; } = "";
        public string Value { get; set; } = "";
    }

    public enum ControlType
    {
        TextBox,
        Slider,
        ToggleSwitch,
        ComboBox,
        Vector2
    }
}

using System.Windows.Forms;

namespace Voidstrap.Models.Persistable
{
    public class State
    {
        public bool TestModeWarningShown { get; set; } = false;

        public bool ShowBloxshadeWarning { get; set; } = false;

        public bool IgnoreOutdatedChannel { get; set; } = false;

        public bool WatcherRunning { get; set; } = false;

        public bool PromptWebView2Install { get; set; } = true;

        public int LastPage { get; set; } = 0;

        public AppState Player { get; set; } = new();

        public AppState Studio { get; set; } = new();

        public WindowState SettingsWindow { get; set; } = new();

        public List<string> ModManifest { get; set; } = new();

        // Preset Repository Settings
        public string PresetRepositoryUrl { get; set; } = "https://github.com/FastFlags/Presets/tree/main/presets";
        
        public List<string> FavoritePresets { get; set; } = new();
        
        public bool AutoRefreshPresets { get; set; } = true;
    }
}
using System.Net.Http;
using System.Text.Json;

namespace Voidstrap
{
    public class PresetManager
    {
        private readonly HttpClient _httpClient = new();
        
        public class PresetInfo
        {
            public string Name { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public string Url { get; set; } = string.Empty;
            public string Category { get; set; } = string.Empty;
        }

        public async Task<List<PresetInfo>> LoadPresetsFromRepositoryAsync(string repoUrl)
        {
            var presets = new List<PresetInfo>();
            
            try
            {
                // Parse GitHub repo URL to get API endpoint
                var apiUrl = ConvertToGitHubApiUrl(repoUrl);
                
                var response = await _httpClient.GetStringAsync(apiUrl);
                var files = JsonSerializer.Deserialize<List<GitHubFile>>(response);
                
                if (files != null)
                {
                    foreach (var file in files.Where(f => f.Name.EndsWith(".json", StringComparison.OrdinalIgnoreCase)))
                    {
                        presets.Add(new PresetInfo
                        {
                            Name = Path.GetFileNameWithoutExtension(file.Name),
                            Description = $"Preset from {file.Name}",
                            Url = file.DownloadUrl,
                            Category = "Repository"
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                App.Logger.WriteLine("PresetManager::LoadPresetsFromRepositoryAsync", $"Error loading presets: {ex.Message}");
            }
            
            return presets;
        }

        public async Task<Dictionary<string, object>?> LoadPresetContentAsync(string url)
        {
            try
            {
                var json = await _httpClient.GetStringAsync(url);
                var options = new JsonSerializerOptions
                {
                    ReadCommentHandling = JsonCommentHandling.Skip,
                    AllowTrailingCommas = true
                };
                
                return JsonSerializer.Deserialize<Dictionary<string, object>>(json, options);
            }
            catch (Exception ex)
            {
                App.Logger.WriteLine("PresetManager::LoadPresetContentAsync", $"Error loading preset content: {ex.Message}");
                return null;
            }
        }

        public List<PresetInfo> GetBuiltInPresets()
        {
            return new List<PresetInfo>
            {
                new PresetInfo
                {
                    Name = "Performance Boost",
                    Description = "Optimized settings for better FPS and performance",
                    Url = "builtin://performance",
                    Category = "Built-in"
                },
                new PresetInfo
                {
                    Name = "Graphics Quality",
                    Description = "Enhanced visual quality settings",
                    Url = "builtin://graphics",
                    Category = "Built-in"
                },
                new PresetInfo
                {
                    Name = "Low-End PC",
                    Description = "Optimized for low-end hardware",
                    Url = "builtin://lowend",
                    Category = "Built-in"
                },
                new PresetInfo
                {
                    Name = "Competitive",
                    Description = "Settings for competitive gameplay",
                    Url = "builtin://competitive",
                    Category = "Built-in"
                }
            };
        }

        public Dictionary<string, object> GetBuiltInPresetContent(string presetId)
        {
            return presetId switch
            {
                "builtin://performance" => new Dictionary<string, object>
                {
                    { "DFIntTaskSchedulerTargetFps", "240" },
                    { "FFlagTaskSchedulerLimitTargetFpsTo2402", "False" },
                    { "DFFlagDisableDPIScale", "True" },
                    { "FFlagDebugGraphicsPreferVulkan", "True" },
                    { "DFIntDebugFRMQualityLevelOverride", "1" }
                },
                "builtin://graphics" => new Dictionary<string, object>
                {
                    { "DFIntDebugFRMQualityLevelOverride", "21" },
                    { "FIntDebugForceMSAASamples", "4" },
                    { "FFlagDisablePostFx", "False" },
                    { "DFIntTextureQualityOverride", "3" }
                },
                "builtin://lowend" => new Dictionary<string, object>
                {
                    { "DFIntDebugFRMQualityLevelOverride", "1" },
                    { "FFlagDisablePostFx", "True" },
                    { "DFIntTextureQualityOverride", "0" },
                    { "FIntDebugTextureManagerSkipMips", "8" },
                    { "DFIntCSGLevelOfDetailSwitchingDistance", "0" }
                },
                "builtin://competitive" => new Dictionary<string, object>
                {
                    { "DFIntTaskSchedulerTargetFps", "240" },
                    { "FFlagDebugDisplayFPS", "True" },
                    { "DFIntDebugFRMQualityLevelOverride", "3" },
                    { "FFlagDisablePostFx", "True" },
                    { "FIntRenderShadowIntensity", "0" }
                },
                _ => new Dictionary<string, object>()
            };
        }

        private string ConvertToGitHubApiUrl(string repoUrl)
        {
            // Convert GitHub repo URL to API URL
            // Example: https://github.com/user/repo/tree/main/folder
            // To: https://api.github.com/repos/user/repo/contents/folder
            
            if (repoUrl.Contains("api.github.com"))
                return repoUrl;
            
            var uri = new Uri(repoUrl);
            var parts = uri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);
            
            if (parts.Length >= 2)
            {
                var user = parts[0];
                var repo = parts[1];
                var path = parts.Length > 4 ? string.Join("/", parts.Skip(4)) : "";
                
                return $"https://api.github.com/repos/{user}/{repo}/contents/{path}";
            }
            
            return repoUrl;
        }

        private class GitHubFile
        {
            public string Name { get; set; } = string.Empty;
            public string DownloadUrl { get; set; } = string.Empty;
            
            [System.Text.Json.Serialization.JsonPropertyName("name")]
            public string JsonName
            {
                get => Name;
                set => Name = value;
            }
            
            [System.Text.Json.Serialization.JsonPropertyName("download_url")]
            public string JsonDownloadUrl
            {
                get => DownloadUrl;
                set => DownloadUrl = value;
            }
        }
    }
}

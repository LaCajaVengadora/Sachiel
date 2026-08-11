using System.IO;
using System.Text.Json;

namespace Sachiel.Services
{
    public class AppSettingsService
    {
        private readonly string _configDirectory;
        private readonly string _configPath;

        public string? ExportFolder { get; set; }
        public AppSettingsService()
        {
            _configDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config");
            _configPath = Path.Combine(_configDirectory, "settings.json");
        }
        private class AppSettings
        {
            public string? ExportFolder { get; set; }
        }


        public void Load()
        {
            try
            {
                if (!File.Exists(_configPath)) return;
                string json = File.ReadAllText(_configPath);
                AppSettings? settings = JsonSerializer.Deserialize<AppSettings>(json);
                if (settings != null) ExportFolder = settings.ExportFolder;
            }
            catch
            {
                ExportFolder = null;
            }
        }

        public bool Save()
        {
            try
            {
                Directory.CreateDirectory(_configDirectory);
                AppSettings settings = new() { ExportFolder = ExportFolder };
                string json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_configPath, json);
                return true;
            }
            catch
            {
                return false;
            }
        }

    }
}
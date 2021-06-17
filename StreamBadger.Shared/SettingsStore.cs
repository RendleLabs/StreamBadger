using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using StreamBadger.Models;

namespace StreamBadger.Shared
{
    public class SettingsStore
    {
        private readonly string _directory;
        private readonly string _filePath;
        public SettingsStore()
        {
            var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            _directory = Path.Combine(userProfile, ".streambadger");
            _filePath = Path.Combine(_directory, "config.json");
        }

        public async Task<SettingsModel> LoadAsync()
        {
            if (!File.Exists(_filePath)) return new SettingsModel();
            using var reader = File.OpenText(_filePath);
            var json = await reader.ReadToEndAsync();
            var model = JsonSerializer.Deserialize<SettingsModel>(json);
            return model ?? new SettingsModel();
        }

        public async Task SaveAsync(SettingsModel settings)
        {
            if (!Directory.Exists(_directory))
            {
                Directory.CreateDirectory(_directory);
            }

            var json = JsonSerializer.Serialize(settings);
            await using var writer = File.CreateText(_filePath);
            await writer.WriteAsync(json);
        }
    }
}
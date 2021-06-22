namespace StreamBadger.Models
{
    public class SettingsModel
    {
        public int Port { get; set; } = 25293;
        public int ObsWebSocketsPort { get; set; } = 4444;
        public string? ObsWebSocketsPassword { get; set; }
    }
}
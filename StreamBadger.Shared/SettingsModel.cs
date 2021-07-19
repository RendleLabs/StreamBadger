namespace StreamBadger.Shared
{
    public class SettingsModel
    {
        public int Port { get; set; } = 25293;
        public string? SlobsBrowserSourceName { get; set; }
        public string? ObsBrowserSourceName { get; set; }
        public int ObsWebSocketsPort { get; set; } = 4444;
        public string? ObsWebSocketsPassword { get; set; }
    }
}
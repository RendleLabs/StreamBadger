namespace StreamBadger.Models
{
    public class SettingsModel
    {
        public string Port { get; set; } = "25293";
        public string ObsWebSocketsPort { get; set; }
        public string ObsWebSocketsPassword { get; set; }
    }
}
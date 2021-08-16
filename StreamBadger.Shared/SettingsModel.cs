namespace StreamBadger.Shared
{
    public class SettingsModel
    {
        public int Port { get; set; } = 25293;
        public string? SlobsBrowserSourceName { get; set; }
        public string? ObsBrowserSourceName { get; set; }
        public int ObsWebSocketsPort { get; set; } = 4444;
        public string? ObsWebSocketsPassword { get; set; }
        public EventSettings? FollowEvent { get; set; } = new();
    }

    public class EventSettings
    {
        public string Image { get; set; }
        public string Sound { get; set; }
        public string TextTemplate { get; set; }
        public TextPosition TextPosition { get; set; }
        public string TextFont { get; set; }
    }

    public enum TextPosition
    {
        TopLeft,
        TopCenter,
        TopRight,
        MiddleLeft,
        MiddleCenter,
        MiddleRight,
        BottomLeft,
        BottomCenter,
        BottomRight,
    }
}
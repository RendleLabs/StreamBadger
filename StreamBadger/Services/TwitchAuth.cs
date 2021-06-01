using StreamBadger.Models;
using System;
using StreamBadgerOverlay.Services;

namespace StreamBadger.Services
{
    public class TwitchAuth
    {
        public void SetSessionData(SessionData sessionData)
        {
            SessionData = sessionData;
            if (sessionData.AccessToken is { Length: > 0 })
            {
                Authenticated?.Invoke();
                TwitchAuthStatic.SetValues(sessionData.AccessToken, sessionData.Id, sessionData.Name);
            }
        }

        public SessionData SessionData { get; private set; }

        public event Action Authenticated;
    }
}
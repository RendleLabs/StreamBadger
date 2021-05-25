using StreamBadger.Models;
using System;

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
            }
        }

        public SessionData SessionData { get; private set; }

        public event Action Authenticated;
    }
}
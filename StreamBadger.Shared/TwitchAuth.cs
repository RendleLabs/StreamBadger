using StreamBadger.Clients;
using StreamBadger.Models;
using System;
using System.IO;
using System.Threading.Tasks;

namespace StreamBadger.Shared
{
    public class TwitchAuth
    {
        private readonly LoginClient _loginClient;
        private readonly string _filePath;
        private string? _sessionId;

        public TwitchAuth(LoginClient loginClient)
        {
            _loginClient = loginClient;
            var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var directory = Path.Combine(userProfile, ".streambadger");
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            _filePath = Path.Combine(directory, "session.id");
        }

        public void SetSessionData(string sessionId, SessionData sessionData)
        {
            SessionData = sessionData;
            if (sessionData.AccessToken is { Length: > 0 })
            {
                Authenticated?.Invoke();
                TwitchAuthStatic.SetValues(sessionData.AccessToken, sessionData.Id, sessionData.Name);

                if (_sessionId != sessionId)
                {
                    using var writer = File.CreateText(_filePath);
                    writer.Write(sessionId);
                    _sessionId = sessionId;
                }
            }
        }

        public async Task<SessionData?> TryGetSessionData()
        {
            _sessionId = await ReadSessionId();
            if (_sessionId is {Length: > 0})
            {
                var data = await _loginClient.TryGetAsync(_sessionId, default);
                if (data is not null)
                {
                    SetSessionData(_sessionId, data);
                    return data;
                }
            }
            return null;
        }

        private async Task<string?> ReadSessionId()
        {
            if (!File.Exists(_filePath)) return null;
            using var reader = File.OpenText(_filePath);
            return (await reader.ReadToEndAsync()).Trim();
        }

        public SessionData? SessionData { get; private set; }

        public event Action? Authenticated;
    }
}
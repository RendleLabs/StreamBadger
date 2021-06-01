using System;

namespace StreamBadgerOverlay.Services
{
    public static class TwitchAuthStatic
    {
        public static string AccessToken { get; private set; }
        public static string Id { get; private set; }
        public static string Name { get; private set; }

        public static event Action Authenticated;

        public static void SetValues(string accessToken, string id, string name)
        {
            AccessToken = accessToken;
            Id = id;
            Name = name;

            if (AccessToken is {Length: >0})
            {
                Authenticated?.Invoke();
            }
        }
    }
}
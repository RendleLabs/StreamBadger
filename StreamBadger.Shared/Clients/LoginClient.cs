using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using StreamBadger.Models;

namespace StreamBadger.Shared.Clients
{
    public class LoginClient
    {
        private readonly HttpClient _client;

        public LoginClient(HttpClient client)
        {
            _client = client;
        }

        public async Task<SessionData?> PingAsync(string sessionId, CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                var response = await _client.GetAsync($"/ping/{sessionId}", token);
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadFromJsonAsync<SessionData>(cancellationToken: token);
                    return data;
                }

                await Task.Delay(TimeSpan.FromSeconds(1), token);
            }

            return null;
        }

        public async Task<SessionData?> TryGetAsync(string sessionId, CancellationToken token)
        {
            var response = await _client.GetAsync($"/ping/{sessionId}", token);
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<SessionData>(cancellationToken: token);
                return data;
            }

            return null;
        }
    }
}
using System.Net.Http;
using System.Threading.Tasks;

namespace StreamBadger.Shared.Clients
{
    public class ServerClient : IServerClient
    {
        private readonly HttpClient _client;

        public ServerClient(HttpClient client)
        {
            _client = client;
        }

        public async Task ShowImage(string name)
        {
            await _client.GetAsync($"/show/{name}");
        }

        public async Task PlaySound(string name)
        {
            await _client.GetAsync($"/play/{name}");
        }
    }
}
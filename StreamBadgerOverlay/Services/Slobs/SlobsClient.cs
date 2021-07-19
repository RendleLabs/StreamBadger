using System;
using System.Linq;
using System.Threading.Tasks;

namespace SlobsSpike
{
    public class SlobsClient : IDisposable
    {
        private readonly SlobsPipeClient _client;

        public SlobsClient()
        {
            _client = new SlobsPipeClient();
        }

        public Task ConnectAsync() => _client.ConnectAsync();

        public async Task SetSourceVisibility(string sourceName, bool visible)
        {
            var request = new SlobsRequest("activeScene", "ScenesService");

            var response = await _client.RunAsync<ActiveSceneResponse>(request);

            var sourceNode = response.Nodes.FirstOrDefault(n => n.Name == "StreamBadger");
            if (sourceNode is not null)
            {
                if (sourceNode.Visible == visible) return;
                
                request = new SlobsRequest("setVisibility", sourceNode.ResourceId, visible);
                await _client.RunAsync(request);
            }
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}
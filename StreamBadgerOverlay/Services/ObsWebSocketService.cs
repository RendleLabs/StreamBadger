using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OBSWebsocketDotNet;
using SlobsSpike;
using StreamBadger.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace StreamBadgerOverlay.Services
{
    public class ObsWebSocketService : BackgroundService
    {
        private readonly SettingsStore _settingsStore;
        private readonly ILogger<ObsWebSocketService> _logger;

        public ObsWebSocketService(SettingsStore settingsStore, ILogger<ObsWebSocketService> logger)
        {
            _settingsStore = settingsStore;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await ToggleOBSSource(true);
            await stoppingToken.WaitAsync();
            await ToggleOBSSource(false);
        }

        private async Task ToggleOBSSource(bool toggle)
        {
            var settings = await _settingsStore.LoadAsync();

            if (settings == null) return;
            if (settings.SlobsBrowserSourceName is {Length: > 0})
            {
                using var client = new SlobsClient();
                await client.ConnectAsync();
                await client.SetSourceVisibility("StreamBadger", toggle);
            }
            else if (settings.ObsBrowserSourceName is {Length: > 0})
            {
                var browserSourceName = settings.ObsBrowserSourceName;

                if (string.IsNullOrEmpty(browserSourceName)) return;

                OBSWebsocket obsWebsocket = new OBSWebsocket();
                var obsWebSocketUrl = $"ws://localhost:{settings.ObsWebSocketsPort}";
                var obsWebSocketPassword = settings.ObsWebSocketsPassword;

                try
                {
                    obsWebsocket.Connect(obsWebSocketUrl, obsWebSocketPassword);
                    obsWebsocket.SetSourceRender(browserSourceName, toggle);
                    if (toggle)
                    {
                        obsWebsocket.RefreshBrowserSource(browserSourceName);
                    }
                    obsWebsocket.Disconnect();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                } 
            }
        }
    }

    internal static class CancellationTokenExtension
    {
        public static Task WaitAsync(this CancellationToken token)
        {
            var completionSource = new TaskCompletionSource();
            token.Register(() => completionSource.SetResult());
            return completionSource.Task;
        }
    }
}

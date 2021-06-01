using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using StreamBadger.Shared;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;
using TwitchLib.PubSub;

namespace StreamBadgerOverlay.Services
{
    public class TwitchBot : BackgroundService
    {
        private readonly ImageStore _imageStore;
        private readonly SoundStore _soundStore;
        private readonly ControlBus _controlBus;
        private TwitchClient _client;
        private TwitchPubSub _pubSub;
        private ConnectionCredentials _credentials;

        public TwitchBot(ImageStore imageStore,
            SoundStore soundStore,
            ControlBus controlBus)
        {
            _imageStore = imageStore;
            _soundStore = soundStore;
            _controlBus = controlBus;

            TwitchAuthStatic.Authenticated += OnAuthenticated;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var completionSource = new TaskCompletionSource();
            stoppingToken.Register(() => completionSource.SetResult());
            return completionSource.Task;
        }

        private void OnAuthenticated()
        {
            var userName = TwitchAuthStatic.Name;
            var accessToken = TwitchAuthStatic.AccessToken;

            _credentials = new ConnectionCredentials(userName, accessToken);
            var clientOptions = new ClientOptions
            {
                MessagesAllowedInPeriod = 750,
                ThrottlingPeriod = TimeSpan.FromSeconds(30)
            };
            WebSocketClient customClient = new(clientOptions);
            _client = new TwitchClient(customClient);
            _client.Initialize(_credentials, userName.ToLowerInvariant());
            _client.OnMessageReceived += OnMessageReceived;
            _client.OnLog += ClientOnOnLog;
            _client.Connect();
            
            // _pubSub = new TwitchPubSub();
            // _pubSub.OnChannelPointsRewardRedeemed += OnChannelPointsRewardRedeemed;
            // _pubSub.OnPubSubServiceError += OnPubSubServiceError;
            // _pubSub.ListenToChannelPoints(_twitchAuth.SessionData.Id);
            // _pubSub.Connect();
            // _pubSub.SendTopics(accessToken);
        }

        private async void OnMessageReceived(object? sender, OnMessageReceivedArgs e)
        {
            if (TryGetCommand(e.ChatMessage.Message, out var command))
            {
                if (command.Equals("images"))
                {
                    var images = _imageStore.GetImageNames().Select(s => $"!{s}");
                    var text = string.Join(", ", images);
                    var message = $"These image commands are available: {text}";
                    _client.SendMessage(TwitchAuthStatic.Name.ToLowerInvariant(), message);
                    return;
                }

                var image = await _imageStore.GetImage(command);
                if (image is not null)
                {
                    var showImage = new ToggleImage(image.Name);
                    await _controlBus.AddAsync(showImage);
                    return;
                }
                //
                // var sound = await _soundStore.GetSound(command);
                // if (sound is not null)
                // {
                //     await _serverClient.PlaySound(sound.Name);
                //     return;
                // }
            }
        }

        private static void ClientOnOnLog(object? sender, OnLogArgs e)
        {
            Debug.WriteLine(e.Data);
        }


        private bool TryGetCommand(ReadOnlySpan<char> text, [NotNullWhen(true)] out string? command)
        {
            command = null;
            if (text is not {Length: > 0}) return false;

            if (text[0] != '!') return false;

            text = text.Slice(1);
            
            foreach (char c in text)
            {
                if (char.IsLetterOrDigit(c)) continue;
                if (c is '_' or '-') continue;
                return false;
            }

            command = new string(text);
            return true;
        }
    }
}
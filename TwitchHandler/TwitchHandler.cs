using System;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Shared.Events.Twitch;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;

namespace TwitchHandler
{
    public class TwitchHandler : BackgroundService
    {
        private readonly ILogger<TwitchHandler> _logger;
        private readonly IConfiguration _config;
        private readonly IPublishEndpoint _publishEndpoint;
        private TwitchClient client;

        public TwitchHandler(ILogger<TwitchHandler> logger, IConfiguration config, IPublishEndpoint publishEndpoint)
        {
            _logger = logger;
            _config = config;
            _publishEndpoint = publishEndpoint;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var credentials = new ConnectionCredentials(
                _config.GetSection("Twitch")["Username"], 
                _config.GetSection("Twitch")["OAuth"]);
            
            var clientOptions = new ClientOptions
            {
                MessagesAllowedInPeriod = 750,
                ThrottlingPeriod = TimeSpan.FromSeconds(30)
            };
            
            var customClient = new WebSocketClient(clientOptions);
            client = new TwitchClient(customClient);
            client.Initialize(credentials, _config.GetSection("Twitch")["Channel"]);
            
            client.OnMessageReceived += Client_OnMessageReceived;
            
            //client.OnChatCommandReceived += OnChatCommand;
            client.OnJoinedChannel += Client_OnJoinedChannel;

            client.Connect();
            
            while (!stoppingToken.IsCancellationRequested)
            {
                //_logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
        }

        private void Client_OnJoinedChannel(object? sender, OnJoinedChannelArgs e)
        {
            _logger.LogInformation("Joined channel " + e.Channel);
        }

        private async void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            var chatEvent = new TwitchChatEvent();
            chatEvent.Id = e.ChatMessage.Id;
            //TODO: Look at caching this so we don't call UtcNow thousands of time per second.
            //NOTE: https://stackoverflow.com/a/57486136/5411549
            chatEvent.ReceivedTime = DateTime.UtcNow;
            chatEvent.UserId = e.ChatMessage.UserId;
            chatEvent.Username = e.ChatMessage.Username;
            chatEvent.RoomId = e.ChatMessage.RoomId;
            chatEvent.Channel = e.ChatMessage.Channel;
            chatEvent.Message = e.ChatMessage.Message;

            await _publishEndpoint.Publish<TwitchChatEvent>(chatEvent);
        }
    }
}

using System;

namespace Shared.Events.Twitch
{
    public class TwitchChatEvent
    {
        public string Id { get; set; }
        
        public DateTime ReceivedTime { get; set; }
        public string UserId { get; set; }
        public string Username { get; set; }
        
        public string RoomId { get; set; }
        public string Channel { get; set; }
        
        public string Message { get; set; }
    }
}
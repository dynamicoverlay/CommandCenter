using AutoMapper;
using CommandCenter.Data;
using Shared.Events.Twitch;

namespace CommandCenter.Profiles
{
    public class EventProfile : Profile
    {
        public EventProfile()
        {
            CreateMap<TwitchChatEvent, TwitchChatMessage>();
        }
    }
}
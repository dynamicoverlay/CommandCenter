using System.Threading.Tasks;
using AutoMapper;
using CommandCenter.Data;
using MassTransit;
using Microsoft.Extensions.Logging;
using Shared.Events.Twitch;

namespace CommandCenter.Consumers
{
    public class TwitchChatConsumer : IConsumer<TwitchChatEvent>
    {
        private readonly ILogger<TwitchChatConsumer> _logger;
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public TwitchChatConsumer(ILogger<TwitchChatConsumer> logger, AppDbContext context, IMapper mapper)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
        }
        
        public async Task Consume(ConsumeContext<TwitchChatEvent> context)
        {
            await _context.TwitchChatMessages.AddAsync(_mapper.Map<TwitchChatMessage>(context.Message));
            await _context.SaveChangesAsync();
        }
    }
}
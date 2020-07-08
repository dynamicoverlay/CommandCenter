using System.Linq;
using System.Threading.Tasks;
using CommandCenter.Data;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared.Events.Twitch.RequestsResponses;

namespace CommandCenter.Consumers
{
    public class ChannelsToMonitorConsumer : IConsumer<RequestChannelsToMonitor>
    {
        private readonly AppDbContext _context;

        public ChannelsToMonitorConsumer(AppDbContext context)
        {
            _context = context;
        }

        public async Task Consume(ConsumeContext<RequestChannelsToMonitor> context)
        {
            var channels = await _context
                .Channels
                .Where(c => c.Monitor)
                .Select(c => new
                {
                    Name = c.Name
                })
                .ToArrayAsync();

            await context.RespondAsync<ResponseChannelsToMonitor>(new
            {
                Channels = channels
            });
        }
    }
}
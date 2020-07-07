using System;
using System.Threading;
using System.Threading.Tasks;
using CommandCenter.Data;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CommandCenter
{
    public class CommandCenter : BackgroundService
    {
        private readonly ILogger<CommandCenter> _logger;
        private readonly AppDbContext _context;
        private readonly IPublishEndpoint _publishEndpoint;

        public CommandCenter(ILogger<CommandCenter> logger, AppDbContext context, IPublishEndpoint publishEndpoint)
        {
            _logger = logger;
            _context = context;
            _publishEndpoint = publishEndpoint;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Starting migration");
            await _context.Database.MigrateAsync(cancellationToken: stoppingToken);
            _logger.LogInformation("Finished migration");
            
            while (!stoppingToken.IsCancellationRequested)
            {

                //_logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}

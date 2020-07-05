using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace TwitchHandler
{
    class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.AddEnvironmentVariables(prefix: "CC_");
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddMassTransit(x =>
                    {
                        x.UsingRabbitMq((context, cfg) =>
                        {
                            cfg.Host(hostContext.Configuration.GetSection("RabbitMQ")["Host"], h =>
                            {
                                h.Username(hostContext.Configuration.GetSection("RabbitMQ")["Username"]);
                                h.Password(hostContext.Configuration.GetSection("RabbitMQ")["Password"]);
                            });
                        });
                    });
                    
                    services.AddMassTransitHostedService();
                    
                    services.AddHostedService<TwitchHandler>();
                });
    }
}

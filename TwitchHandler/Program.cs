using System;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Shared.Events.Twitch.RequestsResponses;

namespace TwitchHandler
{
    class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .Enrich.FromLogContext()
                .Enrich.WithProperty("AppName", "TwitchHandler")
                .WriteTo.Console()
                .WriteTo.Seq("http://seq:5341")
                .CreateLogger();
            
            try
            {
                Log.Information("Starting TwitchHandler host");
                CreateHostBuilder(args).Build().Run();
                return;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
                return;
            }
            finally
            {
                Log.CloseAndFlush();
            }
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
                        
                        x.AddRequestClient<RequestChannelsToMonitor>();
                    });
                    
                    services.AddMassTransitHostedService();
                    
                    services.AddHostedService<TwitchHandler>();
                })
                .UseSerilog();
    }
}

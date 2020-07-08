using System;
using AutoMapper;
using CommandCenter.Consumers;
using CommandCenter.Data;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace CommandCenter
{
    class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .Enrich.FromLogContext()
                .Enrich.WithProperty("AppName", "CommandCenter")
                .WriteTo.Console()
                .WriteTo.Seq("http://seq:5341")
                .CreateLogger();
            
            try
            {
                Log.Information("Starting CommandCenter host");
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
                .ConfigureLogging(builder =>
                {
                    builder.AddSerilog();
                })
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.AddEnvironmentVariables(prefix: "CC_");
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddDbContext<AppDbContext>(options =>
                        options.UseNpgsql(hostContext.Configuration.GetConnectionString("DefaultConnection")));

                    //services.AddScoped<TwitchChatConsumer>();

                    services.AddMassTransit(x =>
                    {
                        x.AddConsumer<TwitchChatConsumer>();
                        x.AddConsumer<ChannelsToMonitorConsumer>();

                        x.UsingRabbitMq((context, cfg) =>
                        {
                            cfg.Host(hostContext.Configuration.GetSection("RabbitMQ")["Host"], h =>
                            {
                                h.Username(hostContext.Configuration.GetSection("RabbitMQ")["Username"]);
                                h.Password(hostContext.Configuration.GetSection("RabbitMQ")["Password"]);
                            });

                            cfg.ReceiveEndpoint("twitch_chat_event", e =>
                            {
                                e.ConfigureConsumer<TwitchChatConsumer>(context);
                            });
                            
                            cfg.ReceiveEndpoint("twitch_channels_request", e =>
                            {
                                e.ConfigureConsumer<ChannelsToMonitorConsumer>(context);
                            });
                        });
                    });

                    services.AddMassTransitHostedService();

                    services.AddAutoMapper(typeof(Program));
                    
                    services.AddHostedService<CommandCenter>();
                })
                .UseSerilog();
    }
}

using AutoMapper;
using CommandCenter.Consumers;
using CommandCenter.Data;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CommandCenter
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
                    services.AddDbContext<AppDbContext>(options =>
                        options.UseNpgsql(hostContext.Configuration.GetConnectionString("DefaultConnection")));

                    //services.AddScoped<TwitchChatConsumer>();

                    services.AddMassTransit(x =>
                    {
                        x.AddConsumer<TwitchChatConsumer>();

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
                        });
                    });

                    services.AddMassTransitHostedService();

                    services.AddAutoMapper(typeof(Program));
                    
                    services.AddHostedService<CommandCenter>();
                });
    }
}

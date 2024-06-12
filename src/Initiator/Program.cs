using EasyNetQ;
using Initiator.Controllers;
using Initiator.Services;
using Initiator.Services.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Initiator;

    public class Program
    {
        public static async Task Main(string[] args)
        {
            var rabbitConnectionString = Environment.GetEnvironmentVariable("RabbitMQ__ConnectionString");
            var numberOfLaunches = int.Parse(Environment.GetEnvironmentVariable("NUMBER_OF_LAUNCHES") ?? "5");
            var queueName = Environment.GetEnvironmentVariable("QUEUE_NAME");
            var calculatorUrl = Environment.GetEnvironmentVariable("CALCULATOR_URL");
            var messageTtl = int.Parse(Environment.GetEnvironmentVariable("MESSAGE_TTL") ?? "8");

            if (rabbitConnectionString is null || queueName is null || calculatorUrl is null)
            {
                Console.WriteLine("One or more required environment variables are not set.");
                return;
            }

            var host = new HostBuilder()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        .UseKestrel()
                        .ConfigureServices((context, services) =>
                        {
                            services.AddControllers();
                            services.AddHttpClient();

                            services.AddSingleton<IBus>(provider =>
                                RabbitHutch.CreateBus(rabbitConnectionString));

                            services.AddSingleton<ILogStrategy, ConsoleLogStrategy>();
                            services.AddSingleton<IFibonacciService, FibonacciService>();

                            services.AddSingleton<IMessageQueueService>(provider =>
                                new MessageQueueService(
                                    bus: provider.GetRequiredService<IBus>(),
                                    httpClientService: provider.GetRequiredService<IHttpClientService>(),
                                    queueName: queueName,
                                    messageTtl: messageTtl,
                                    logStrategy: provider.GetRequiredService<ILogStrategy>()
                                ));

                            services.AddSingleton<IHttpClientService>(provider =>
                                new HttpClientService(
                                    httpClientFactory: provider.GetRequiredService<IHttpClientFactory>(),
                                    calculatorUrl: calculatorUrl,
                                    logStrategy: provider.GetRequiredService<ILogStrategy>()
                                ));

                            services.AddSingleton<InitiatorController>();
                            services.AddHostedService(provider => new InitialCalculationService(provider, numberOfLaunches));
                        })
                        .Configure(app =>
                        {
                            app.UseRouting();
                            app.UseEndpoints(endpoints =>
                            {
                                endpoints.MapControllers();
                            });
                        });
                })
                .Build();

            await host.RunAsync().ConfigureAwait(false);
        }
    }

using EasyNetQ;
using Initiator.Controllers;
using Initiator.Services;
using Initiator.Services.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SharedModels;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using SharedModels.Interfaces;

namespace Initiator;

public class Program
{
    public static async Task Main(string[] args)
    {
        var rabbitConnectionString = Environment.GetEnvironmentVariable("RabbitMQ__ConnectionString");
        var queueName = Environment.GetEnvironmentVariable("QUEUE_NAME");
        var calculatorUrl = Environment.GetEnvironmentVariable("CALCULATOR_URL");

        int messageTtl = ParseEnvironmentVariable("MESSAGE_TTL", defaultValue: 8);
        int numberOfLaunches = ParseEnvironmentVariable("NUMBER_OF_LAUNCHES", defaultValue: 5);
        int startPrevious = ParseEnvironmentVariable("START_PREVIOUS", defaultValue: 0);
        int startCurrent = ParseEnvironmentVariable("START_CURRENT", defaultValue: 1);

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

                        services.AddTransient<IMessageQueueService>(provider =>
                            new MessageQueueService(
                                bus: provider.GetRequiredService<IBus>(),
                                httpClientService: provider.GetRequiredService<IHttpClientService>(),
                                queueName: queueName,
                                messageTtl: messageTtl,
                                logStrategy: provider.GetRequiredService<ILogStrategy>()
                            ));

                        services.AddTransient<ILogStrategy, ConsoleLogStrategy>();

                        services.AddTransient<IFibonacciStartService>(provider =>
                            new FibonacciStartService(
                                httpClientService: provider.GetRequiredService<IHttpClientService>(),
                                messageQueueService: provider.GetRequiredService<IMessageQueueService>(),
                                startPrevious: startPrevious,
                                startCurrent: startCurrent,
                                logStrategy: provider.GetRequiredService<ILogStrategy>()
                            ));

                        services.AddTransient<ICalculateNextService>(provider =>
                            new CalculateNextService(
                                startPrevious: startPrevious.ToString(),
                                startCurrent: startCurrent.ToString(),
                                logStrategy: provider.GetRequiredService<ILogStrategy>()
                            ));

                        services.AddTransient<IHttpClientService>(provider =>
                            new HttpClientService(
                                httpClientFactory: provider.GetRequiredService<IHttpClientFactory>(),
                                calculatorUrl: calculatorUrl,
                                logStrategy: provider.GetRequiredService<ILogStrategy>(),
                                calculateNextService: provider.GetRequiredService<ICalculateNextService>()
                            ));

                        services.AddSingleton<InitiatorController>();
                        services.AddHostedService(provider =>
                            new InitialCalculationService(provider, numberOfLaunches));
                    })
                    .Configure(app =>
                    {
                        app.UseRouting();
                        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
                    });
            })
            .Build();

        await host.RunAsync().ConfigureAwait(false);
    }

    private static int ParseEnvironmentVariable(string variableName, int defaultValue)
    {
        var value = Environment.GetEnvironmentVariable(variableName);
        if (int.TryParse(value, out var result))
        {
            return result;
        }

        Console.WriteLine($"{variableName} is not set or invalid. Using default value: {defaultValue}");
        return defaultValue;
    }
}
using System;
using System.Threading.Tasks;
using EasyNetQ;
using Initiator.Services.Interfaces;
using SharedModels;

namespace Initiator.Services;

public class MessageQueueService : IMessageQueueService
{
    private readonly IBus _bus;
    private readonly IHttpClientService _httpClientService;

    public MessageQueueService(IBus bus, IHttpClientService httpClientService)
    {
        _bus = bus;
        _httpClientService = httpClientService;
    }

    public async Task DeclareAndSubscribeToQueueWithTtlAsync()
    {
        const string queueName = "Initiator_queue";
        const int messageTtl = 10;

        _bus.Advanced.QueueDeclare(queueName, c => c
            .WithMessageTtl(TimeSpan.FromSeconds(messageTtl)));

        await SubscribeToMessages(queueName).ConfigureAwait(false);
    }

    private async Task SubscribeToMessages(string queueName, int retryCount = 5)
    {
        for (int i = 0; i < retryCount; i++)
        {
            try
            {
                await _bus.PubSub.SubscribeAsync<FibonacciState>(queueName,
                    async state =>
                    {
                        var difference = DateTime.Now - state.SendTime;
                        const int maxDifference = 8;
                        
                        if (difference.TotalSeconds > maxDifference) return;
                        
                        await _httpClientService.SendStateToCalculatorAsync(state).ConfigureAwait(false);
                    });
                return;
            }
            catch (TaskCanceledException)
            {
                Console.WriteLine($"Failed to subscribe to messages. Retry attempt {i + 1}/{retryCount}...");
                await Task.Delay(1000).ConfigureAwait(false);
            }
        }

        Console.WriteLine($"Failed to subscribe to messages after {retryCount} attempts.");
    }
}

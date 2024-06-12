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
    private readonly string _queueName;
    private readonly int _messageTtl;
    private readonly int _subscribeRetryCount;

    public MessageQueueService(
        IBus bus,
        IHttpClientService httpClientService,
        string queueName,
        int messageTtl,
        int subscribeRetryCount
        )
    {
        _bus = bus;
        _httpClientService = httpClientService;
        _queueName = queueName;
        _messageTtl = messageTtl;
        _subscribeRetryCount = subscribeRetryCount;
    }

    public async Task DeclareAndSubscribeToQueueWithTtlAsync()
    {
        _bus.Advanced.QueueDeclare(_queueName, c => c
            .WithMessageTtl(TimeSpan.FromSeconds(_messageTtl)));

        await SubscribeToMessages(_queueName).ConfigureAwait(false);
    }


    private async Task SubscribeToMessages(string queueName)
    {
        for (int i = 0; i < _subscribeRetryCount; i++)
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
                Console.WriteLine($"Failed to subscribe to messages. Retry attempt {i + 1}/{_subscribeRetryCount}...");
                await Task.Delay(1000).ConfigureAwait(false);
            }
        }

        Console.WriteLine($"Failed to subscribe to messages after {_subscribeRetryCount} attempts.");
    }
}

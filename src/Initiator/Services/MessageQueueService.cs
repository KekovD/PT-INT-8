using EasyNetQ;
using Initiator.Services.Interfaces;
using SharedModels;
using System;
using System.Text;
using System.Threading.Tasks;
using SharedModels.Interfaces;

namespace Initiator.Services;

public class MessageQueueService : IMessageQueueService
{
    private readonly IBus _bus;
    private readonly IHttpClientService _httpClientService;
    private readonly string _queueName;
    private readonly int _messageTtl;
    private readonly ILogStrategy _logStrategy;

    public MessageQueueService(
        IBus bus,
        IHttpClientService httpClientService,
        string queueName,
        int messageTtl,
        ILogStrategy logStrategy
        )
    {
        _bus = bus;
        _httpClientService = httpClientService;
        _queueName = queueName;
        _messageTtl = messageTtl;
        _logStrategy = logStrategy;
    }

    public async Task DeclareAndSubscribeToQueueWithTtlAsync()
    {
        _bus.Advanced.QueueDeclare(_queueName, c => c
            .WithMessageTtl(TimeSpan.FromSeconds(_messageTtl)));

        await SubscribeToMessages(_queueName).ConfigureAwait(false);
    }


    private async Task SubscribeToMessages(string queueName)
    {
        var logBuilder = new StringBuilder();
        const int retryDelayMilliseconds = 1000;
        const int subscribeRetryCount = 5;
        
        for (int i = 0; i < subscribeRetryCount; i++)
        {
            try
            {
                await _bus.PubSub.SubscribeAsync<FibonacciState>(queueName,
                    async state =>
                    {
                        var difference = DateTime.UtcNow - state.SendTime;
                        const int maxTimeDifference = 8;
                        
                        if (difference.TotalSeconds > maxTimeDifference) return;
                        
                        await _httpClientService.SendStateToCalculatorAsync(state).ConfigureAwait(false);
                    });
                return;
            }
            catch (TaskCanceledException)
            {
                logBuilder.Clear();

                int retryNumber = i + 1;
                
                logBuilder
                    .Append("Failed to subscribe to messages. Retry attempt ")
                    .Append(retryNumber)
                    .Append('/')
                    .Append(subscribeRetryCount)
                    .Append("...");
                
                await _logStrategy.LogAsync(logBuilder.ToString()).ConfigureAwait(false);
                
                await Task.Delay(retryDelayMilliseconds).ConfigureAwait(false);
            }
        }

        logBuilder.Clear();
        
        logBuilder.Append("Failed to subscribe to messages after ")
            .Append(subscribeRetryCount)
            .Append(" attempts.");
        
        await _logStrategy.LogAsync(logBuilder.ToString()).ConfigureAwait(false);
    }
}

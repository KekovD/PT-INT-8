using System;
using System.Text;
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
    private readonly ILogStrategy _logStrategy;

    public MessageQueueService(
        IBus bus,
        IHttpClientService httpClientService,
        string queueName,
        int messageTtl,
        int subscribeRetryCount,
        ILogStrategy logStrategy
        )
    {
        _bus = bus;
        _httpClientService = httpClientService;
        _queueName = queueName;
        _messageTtl = messageTtl;
        _subscribeRetryCount = subscribeRetryCount;
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
                logBuilder.Clear();
                
                logBuilder.Append("Failed to subscribe to messages. Retry attempt ")
                    .Append(i + 1)
                    .Append('/')
                    .Append(_subscribeRetryCount)
                    .Append("...");
                
                _logStrategy.Log(logBuilder.ToString());
                
                await Task.Delay(retryDelayMilliseconds).ConfigureAwait(false);
            }
        }

        logBuilder.Clear();
        
        logBuilder.Append("Failed to subscribe to messages after ")
            .Append(_subscribeRetryCount)
            .Append(" attempts.");
        
        _logStrategy.Log(logBuilder.ToString());
    }
}

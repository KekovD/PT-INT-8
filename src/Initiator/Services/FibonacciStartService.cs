using Initiator.Services.Interfaces;
using SharedModels;
using SharedModels.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Initiator.Services;

public class FibonacciStartService : IFibonacciStartService
{
    private readonly IHttpClientService _httpClientService;
    private readonly IMessageQueueService _messageQueueService;
    private readonly int _startPrevious;
    private readonly int _startCurrent;
    private readonly ILogStrategy _logStrategy;

    public FibonacciStartService(
        IHttpClientService httpClientService,
        IMessageQueueService messageQueueService,
        int startPrevious,
        int startCurrent,
        ILogStrategy logStrategy
        )
    {
        _httpClientService = httpClientService;
        _messageQueueService = messageQueueService;
        _startPrevious = startPrevious;
        _startCurrent = startCurrent;
        _logStrategy = logStrategy;
    }

    public async Task StartCalculationsAsync(int numberOfLaunches)
    {
        if (numberOfLaunches <= 0)
        {
            var errorMessage = $"Number of launches {numberOfLaunches} must be greater than zero.";
            await _logStrategy.LogAsync(errorMessage).ConfigureAwait(false);
            throw new ArgumentOutOfRangeException(nameof(numberOfLaunches), errorMessage);
        }
        
        await _messageQueueService.SubscribeToMessages().ConfigureAwait(false);

        var tasks = new List<Task>();

        for (int i = 0; i < numberOfLaunches; i++)
        {
            var state = new FibonacciState(_startPrevious.ToString(), _startCurrent.ToString(), StartId: i, DateTime.UtcNow);
            tasks.Add(_httpClientService.SendStateToCalculatorAsync(state));
        }

        await Task.WhenAll(tasks).ConfigureAwait(false);
    }
}
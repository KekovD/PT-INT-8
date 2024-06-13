using Initiator.Services.Interfaces;
using SharedModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Initiator.Services;

public class FibonacciService : IFibonacciService
{
    private readonly IHttpClientService _httpClientService;
    private readonly IMessageQueueService _messageQueueService;
    private readonly string _startPrevious;
    private readonly string _startCurrent;
    private readonly ILogStrategy _logStrategy;

    public FibonacciService(
        IHttpClientService httpClientService,
        IMessageQueueService messageQueueService,
        string startPrevious,
        string startCurrent, ILogStrategy logStrategy)
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
        
        await _messageQueueService.DeclareAndSubscribeToQueueWithTtlAsync().ConfigureAwait(false);

        var tasks = new List<Task>();

        for (int i = 0; i < numberOfLaunches; i++)
        {
            var state = new FibonacciState(_startPrevious, _startCurrent, StartId: i, DateTime.Now);
            tasks.Add(_httpClientService.SendStateToCalculatorAsync(state));
        }

        await Task.WhenAll(tasks).ConfigureAwait(false);
    }
}
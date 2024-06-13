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

    public FibonacciService(
        IHttpClientService httpClientService,
        IMessageQueueService messageQueueService,
        string startPrevious,
        string startCurrent
        )
    {
        _httpClientService = httpClientService;
        _messageQueueService = messageQueueService;
        _startPrevious = startPrevious;
        _startCurrent = startCurrent;
    }

    public async Task StartCalculationsAsync(int numberOfLaunches)
    {
        await _messageQueueService.DeclareAndSubscribeToQueueWithTtlAsync();

        var tasks = new List<Task>();

        for (int i = 0; i < numberOfLaunches; i++)
        {
            var state = new FibonacciState(_startPrevious, _startCurrent, StartId: i, DateTime.Now);
            tasks.Add(_httpClientService.SendStateToCalculatorAsync(state));
        }

        await Task.WhenAll(tasks).ConfigureAwait(false);
    }
}
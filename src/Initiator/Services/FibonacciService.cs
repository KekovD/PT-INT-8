using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Initiator.Services.Interfaces;
using SharedModels;

namespace Initiator.Services;

public class FibonacciService : IFibonacciService
{
    private readonly IHttpClientService _httpClientService;
    private readonly IMessageQueueService _messageQueueService;

    public FibonacciService(IHttpClientService httpClientService, IMessageQueueService messageQueueService)
    {
        _httpClientService = httpClientService;
        _messageQueueService = messageQueueService;
    }

    public async Task StartCalculationsAsync(int count)
    {
        await _messageQueueService.DeclareAndSubscribeToQueueWithTtlAsync();

        const string previous = "0";
        const string current = "1";
        var tasks = new List<Task>();

        for (int i = 0; i < count; i++)
        {
            var state = new FibonacciState(previous, current, StartId: i, DateTime.Now);
            tasks.Add(_httpClientService.SendStateToCalculatorAsync(state));
        }

        await Task.WhenAll(tasks).ConfigureAwait(false);
    }
}
using EasyNetQ;
using SharedModels;
using System;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Calculator.Services.Interfaces;

namespace Calculator.Services;

public class SendNextService : ISendNextService
{
    private readonly IBus _bus;
    private readonly ILogStrategy _logStrategy;
    private readonly ICalculateNextService _calculateNextService;

    public SendNextService(IBus bus, ILogStrategy logStrategy, ICalculateNextService calculateNextService)
    {
        _bus = bus;
        _logStrategy = logStrategy;
        _calculateNextService = calculateNextService;
    }

    public async Task SendNext(FibonacciState state)
    {
        var newState = _calculateNextService.CalculateNext(state);
        
        var logBuilder = new StringBuilder();
        
        logBuilder.Append("Sent Fibonacci state: Previous=")
            .Append(newState.Previous)
            .Append(", Current=")
            .Append(newState.Current)
            .Append(", StartId=")
            .Append(newState.StartId);

        _logStrategy.LogAsync(logBuilder.ToString());

        await _bus.PubSub.PublishAsync(newState).ConfigureAwait(false);
    }
}
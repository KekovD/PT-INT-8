using EasyNetQ;
using SharedModels;
using System;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Calculator.Services.Interfaces;

namespace Calculator.Services;

public class CalculateNextService : ICalculateNextService
{
    private readonly IBus _bus;
    private readonly ILogStrategy _logStrategy;

    public CalculateNextService(IBus bus, ILogStrategy logStrategy)
    {
        _bus = bus;
        _logStrategy = logStrategy;
    }

    public async Task CalculateNext(FibonacciState state)
    {
        var previous = BigInteger.Parse(state.Previous);
        var current = BigInteger.Parse(state.Current);
        var newCurrent = BigInteger.Add(previous, current);
        
        var newState = new FibonacciState(state.Current, newCurrent.ToString(), state.StartId, DateTime.Now);
        
        var logBuilder = new StringBuilder();
        
        logBuilder.Append("Sent Fibonacci state: Previous=")
            .Append(newState.Previous)
            .Append(", Current=")
            .Append(newState.Current)
            .Append(", StartId=")
            .Append(newState.StartId);

        _logStrategy.Log(logBuilder.ToString());

        await _bus.PubSub.PublishAsync(newState).ConfigureAwait(false);
    }
}
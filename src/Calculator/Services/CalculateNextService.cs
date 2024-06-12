using System;
using System.Numerics;
using System.Threading.Tasks;
using EasyNetQ;
using SharedModels;

namespace Calculator.Services;

public class CalculateNextService : ICalculateNextService
{
    private readonly IBus _bus;

    public CalculateNextService(IBus bus)
    {
        _bus = bus;
    }

    public async Task CalculateNext(FibonacciState state)
    {
        var previous = BigInteger.Parse(state.Previous);
        var current = BigInteger.Parse(state.Current);
        var newCurrent = BigInteger.Add(previous, current);
        
        var newState = new FibonacciState(state.Current, newCurrent.ToString(), state.StartId, DateTime.Now);

        await _bus.PubSub.PublishAsync(newState).ConfigureAwait(false);
    }
}
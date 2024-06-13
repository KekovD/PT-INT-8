using SharedModels;
using System;
using System.Numerics;
using System.Threading.Tasks;
using Initiator.Services.Interfaces;

namespace Initiator.Services;

public class CalculateNextService : ICalculateNextService
{
    private readonly string _startPrevious;
    private readonly string _startCurrent;

    public CalculateNextService(string startPrevious, string startCurrent)
    {
        _startPrevious = startPrevious;
        _startCurrent = startCurrent;
    }

    public Task<FibonacciState> CalculateNextAsync(FibonacciState state)
    {
        if (state.Previous.Equals(_startPrevious, StringComparison.InvariantCulture) &&
            state.Current.Equals(_startCurrent, StringComparison.InvariantCulture)) 
            return Task.FromResult(state);
        
        var previous = BigInteger.Parse(state.Previous);
        var current = BigInteger.Parse(state.Current);
        var newCurrent = BigInteger.Add(previous, current);
        
        return Task.FromResult(
            new FibonacciState(state.Current, newCurrent.ToString(), state.StartId, DateTime.Now));
    }
}
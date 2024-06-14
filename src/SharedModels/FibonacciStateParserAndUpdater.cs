using System;
using System.Numerics;
using System.Threading.Tasks;
using SharedModels.Interfaces;

namespace SharedModels;

public class FibonacciStateParserAndUpdater : IFibonacciStateParserAndUpdater
{
    public Task<FibonacciState> ParseAndUpdateStateAsync(FibonacciState state)
    {
        var previous = BigInteger.Parse(state.Previous);
        var current = BigInteger.Parse(state.Current);
        var newCurrent = BigInteger.Add(previous, current);

        return Task.FromResult(
            new FibonacciState(state.Current, newCurrent.ToString(), state.StartId, DateTime.UtcNow));
    }
}
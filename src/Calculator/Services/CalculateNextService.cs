using System;
using System.Numerics;
using System.Threading.Tasks;
using Calculator.Services.Interfaces;
using SharedModels;

namespace Calculator.Services;

public class CalculateNextService : ICalculateNextService
{
    public Task<FibonacciState> CalculateNextAsync(FibonacciState state)
    {
        var previous = BigInteger.Parse(state.Previous);
        var current = BigInteger.Parse(state.Current);
        var newCurrent = BigInteger.Add(previous, current);

        return Task.FromResult(
            new FibonacciState(state.Current, newCurrent.ToString(), state.StartId, DateTime.Now));
    }
}
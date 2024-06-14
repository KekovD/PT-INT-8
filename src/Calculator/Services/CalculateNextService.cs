using System;
using System.Numerics;
using System.Threading.Tasks;
using Calculator.Services.Interfaces;
using SharedModels;

namespace Calculator.Services;

public class CalculateNextService : ICalculateNextService
{
    private readonly ILogStrategy _logStrategy;

    public CalculateNextService(ILogStrategy logStrategy)
    {
        _logStrategy = logStrategy;
    }

    public async Task<FibonacciState> CalculateNextAsync(FibonacciState state)
    {
        try
        {
            var previous = BigInteger.Parse(state.Previous);
            var current = BigInteger.Parse(state.Current);
            var newCurrent = BigInteger.Add(previous, current);

            return new FibonacciState(state.Current, newCurrent.ToString(), state.StartId, DateTime.Now);
        }
        catch (FormatException ex)
        {
            var errorMessage = $"Parsing error: {ex.Message}";
            await _logStrategy.LogAsync(errorMessage).ConfigureAwait(false);
            throw new InvalidOperationException(errorMessage, ex);
        }
    }
}
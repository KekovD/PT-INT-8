using SharedModels;
using System;
using System.Numerics;
using System.Threading.Tasks;
using Initiator.Services.Interfaces;
using SharedModels.Interfaces;

namespace Initiator.Services;

public class CalculateNextService : ICalculateNextService
{
    private readonly string _startPrevious;
    private readonly string _startCurrent;
    private readonly ILogStrategy _logStrategy;

    public CalculateNextService(string startPrevious, string startCurrent, ILogStrategy logStrategy)
    {
        _startPrevious = startPrevious;
        _startCurrent = startCurrent;
        _logStrategy = logStrategy;
    }

    public async Task<FibonacciState> CalculateNextAsync(FibonacciState state)
    {
        if (state.Previous.Equals(_startPrevious, StringComparison.InvariantCulture) &&
            state.Current.Equals(_startCurrent, StringComparison.InvariantCulture)) 
            return state;

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
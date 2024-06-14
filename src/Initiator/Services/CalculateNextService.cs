using Initiator.Services.Interfaces;
using SharedModels;
using SharedModels.Interfaces;
using System;
using System.Threading.Tasks;

namespace Initiator.Services;

public class CalculateNextService : ICalculateNextService
{
    private readonly string _startPrevious;
    private readonly string _startCurrent;
    private readonly ILogStrategy _logStrategy;
    private readonly IFibonacciStateParserAndUpdater _parserAndUpdater;

    public CalculateNextService(
        string startPrevious,
        string startCurrent,
        ILogStrategy logStrategy,
        IFibonacciStateParserAndUpdater parserAndUpdater
        )
    {
        _startPrevious = startPrevious;
        _startCurrent = startCurrent;
        _logStrategy = logStrategy;
        _parserAndUpdater = parserAndUpdater;
    }

    public async Task<FibonacciState> CalculateNextAsync(FibonacciState state)
    {
        if (state.Previous.Equals(_startPrevious, StringComparison.InvariantCulture) &&
            state.Current.Equals(_startCurrent, StringComparison.InvariantCulture)) 
            return state;

        try
        {
            return await _parserAndUpdater.ParseAndUpdateStateAsync(state).ConfigureAwait(false);
        }
        catch (FormatException ex)
        {
            var errorMessage = $"Parsing error: {ex.Message}";
            await _logStrategy.LogAsync(errorMessage).ConfigureAwait(false);
            throw new InvalidOperationException(errorMessage, ex);
        }
    }
}
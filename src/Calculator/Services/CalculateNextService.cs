using System;
using System.Threading.Tasks;
using Calculator.Services.Interfaces;
using SharedModels;
using SharedModels.Interfaces;

namespace Calculator.Services;

public class CalculateNextService : ICalculateNextService
{
    private readonly ILogStrategy _logStrategy;
    private readonly IFibonacciStateParserAndUpdater _parserAndUpdater;

    public CalculateNextService(ILogStrategy logStrategy, IFibonacciStateParserAndUpdater parserAndUpdater)
    {
        _logStrategy = logStrategy;
        _parserAndUpdater = parserAndUpdater;
    }

    public async Task<FibonacciState> CalculateNextAsync(FibonacciState state)
    {
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
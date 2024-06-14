using System.Threading.Tasks;

namespace SharedModels.Interfaces;

public interface IFibonacciStateParserAndUpdater
{
    Task<FibonacciState> ParseAndUpdateStateAsync(FibonacciState state);
}
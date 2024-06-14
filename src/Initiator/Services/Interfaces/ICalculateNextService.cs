using SharedModels;
using System.Threading.Tasks;

namespace Initiator.Services.Interfaces;

public interface ICalculateNextService
{
    Task<FibonacciState> CalculateNextAsync(FibonacciState state);
}
using SharedModels;

namespace Initiator.Services.Interfaces;

public interface ICalculateNextService
{
    FibonacciState CalculateNext(FibonacciState state);
}
using SharedModels;

namespace Initiator.Services.Interfaces;

public interface IFibonacciCalculatorService
{
    FibonacciState CalculateNewState(FibonacciState state);
}
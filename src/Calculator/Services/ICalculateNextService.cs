using System.Threading.Tasks;
using SharedModels;

namespace Calculator.Services;

public interface ICalculateNextService
{
    Task CalculateNext(FibonacciState state);
}
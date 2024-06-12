using SharedModels;
using System.Threading.Tasks;

namespace Calculator.Services;

public interface ICalculateNextService
{
    Task CalculateNext(FibonacciState state);
}
using System.Threading.Tasks;
using SharedModels;

namespace Calculator.Services.Interfaces;

public interface ICalculateNextService
{
    FibonacciState CalculateNext(FibonacciState state);
}
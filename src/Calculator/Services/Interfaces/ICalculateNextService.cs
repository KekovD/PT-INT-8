using System.Threading.Tasks;
using SharedModels;

namespace Calculator.Services.Interfaces;

public interface ICalculateNextService
{
    Task CalculateNext(FibonacciState state);
}
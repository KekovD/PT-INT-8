using System.Threading.Tasks;
using SharedModels;

namespace Calculator.Services.Interfaces;

public interface ISendNextService
{
    Task SendNextAsync(FibonacciState state);
}
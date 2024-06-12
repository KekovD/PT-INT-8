using System.Threading.Tasks;
using SharedModels;

namespace Initiator.Services.Interfaces;

public interface IHttpClientService
{
    Task SendStateToCalculatorAsync(FibonacciState state);
}
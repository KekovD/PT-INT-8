using SharedModels;
using System.Threading.Tasks;

namespace Initiator.Services.Interfaces;

public interface IHttpClientService
{
    Task SendStateToCalculatorAsync(FibonacciState state);
}
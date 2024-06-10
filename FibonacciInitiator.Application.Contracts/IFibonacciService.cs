using System.Threading.Tasks;
using FibonacciInitiatorService.Application.Models;

namespace FibonacciInitiatorService.Application.Contracts;

public interface IFibonacciService
{
    public Task<FibonacciResult> CalculateNext(FibonacciResult previousResult);
}
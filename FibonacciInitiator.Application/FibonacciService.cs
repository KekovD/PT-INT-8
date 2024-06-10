using System.Threading.Tasks;
using FibonacciInitiatorService.Application.Contracts;
using FibonacciInitiatorService.Application.Models;

namespace FibonacciInitiatorService.Application;

internal class FibonacciService : IFibonacciService
{
    public Task<FibonacciResult> CalculateNext(FibonacciResult previousResult) =>
        Task.FromResult(new FibonacciResult(previousResult.Current, previousResult.Current + previousResult.Previous));
}
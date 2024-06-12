using System.Threading.Tasks;

namespace Initiator.Services.Interfaces;

public interface IFibonacciService
{
    Task StartCalculationsAsync(int count);
}
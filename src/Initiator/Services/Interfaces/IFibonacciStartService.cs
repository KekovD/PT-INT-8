using System.Threading.Tasks;

namespace Initiator.Services.Interfaces;

public interface IFibonacciStartService
{
    Task StartCalculationsAsync(int numberOfLaunches);
}
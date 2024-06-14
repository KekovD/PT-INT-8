using Calculator.Services.Interfaces;
using EasyNetQ;
using SharedModels;
using SharedModels.Interfaces;
using System.Text;
using System.Threading.Tasks;

namespace Calculator.Services;

public class SendNextService : ISendNextService
{
    private readonly IBus _bus;
    private readonly ILogStrategy _logStrategy;
    private readonly ICalculateNextService _calculateNextService;

    public SendNextService(IBus bus, ILogStrategy logStrategy, ICalculateNextService calculateNextService)
    {
        _bus = bus;
        _logStrategy = logStrategy;
        _calculateNextService = calculateNextService;
    }

    public async Task SendNextAsync(FibonacciState state)
    {
        try
        {
            var newState = await _calculateNextService.CalculateNextAsync(state).ConfigureAwait(false);

            var logBuilder = new StringBuilder();

            logBuilder
                .Append("Sent Fibonacci state: Previous=")
                .Append(newState.Previous)
                .Append(", Current=")
                .Append(newState.Current)
                .Append(", StartId=")
                .Append(newState.StartId);

            await _logStrategy.LogAsync(logBuilder.ToString()).ConfigureAwait(false);

            await _bus.PubSub.PublishAsync(newState).ConfigureAwait(false);
        }
        catch (EasyNetQException exception)
        {
            var errorMessage = $"Error publishing Fibonacci state: {exception.Message}";
            await _logStrategy.LogAsync(errorMessage).ConfigureAwait(false);
            throw new EasyNetQException(errorMessage, exception);
        }
    }
}
using Initiator.Services.Interfaces;
using SharedModels;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using SharedModels.Interfaces;

namespace Initiator.Services;

public class HttpClientService : IHttpClientService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _calculatorUrl;
    private readonly ILogStrategy _logStrategy;
    private readonly ICalculateNextService _calculateNextService;
        
    public HttpClientService(
        IHttpClientFactory httpClientFactory,
        string calculatorUrl,
        ILogStrategy logStrategy,
        ICalculateNextService calculateNextService
        )
    {
        _httpClientFactory = httpClientFactory;
        _calculatorUrl = calculatorUrl;
        _logStrategy = logStrategy;
        _calculateNextService = calculateNextService;
    }

    public async Task SendStateToCalculatorAsync(FibonacciState state)
    {
        try
        {
            HttpClient client = _httpClientFactory.CreateClient();

            state = await _calculateNextService.CalculateNextAsync(state).ConfigureAwait(false);

            var logBuilder = new StringBuilder();

            logBuilder
                .Append("Sent Fibonacci state: Previous=")
                .Append(state.Previous)
                .Append(", Current=")
                .Append(state.Current)
                .Append(", StartId=")
                .Append(state.StartId);

            await _logStrategy.LogAsync(logBuilder.ToString()).ConfigureAwait(false);

            await client.PostAsJsonAsync(_calculatorUrl, state).ConfigureAwait(false);
        }
        catch (HttpRequestException exception)
        {
            var errorMessage = $"HTTP request exception: {exception.Message}";
            await _logStrategy.LogAsync(errorMessage).ConfigureAwait(false);
            throw new HttpRequestException(errorMessage, exception);
        }
    }
}
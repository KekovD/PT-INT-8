using Initiator.Services.Interfaces;
using SharedModels;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Initiator.Services;

public class HttpClientService : IHttpClientService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _calculatorUrl;
    private readonly ILogStrategy _logStrategy;
    private readonly IFibonacciCalculatorService _fibonacciCalculatorService;
        
    public HttpClientService(
        IHttpClientFactory httpClientFactory,
        string calculatorUrl,
        ILogStrategy logStrategy,
        IFibonacciCalculatorService fibonacciCalculatorService
        )
    {
        _httpClientFactory = httpClientFactory;
        _calculatorUrl = calculatorUrl;
        _logStrategy = logStrategy;
        _fibonacciCalculatorService = fibonacciCalculatorService;
    }

    public async Task SendStateToCalculatorAsync(FibonacciState state)
    {
        HttpClient client = _httpClientFactory.CreateClient();

        state = _fibonacciCalculatorService.CalculateNewState(state);

        var logBuilder = new StringBuilder();
        
        logBuilder.Append("Sent Fibonacci state: Previous=")
            .Append(state.Previous)
            .Append(", Current=")
            .Append(state.Current)
            .Append(", StartId=")
            .Append(state.StartId);

        _logStrategy.Log(logBuilder.ToString());

        await client.PostAsJsonAsync(_calculatorUrl, state).ConfigureAwait(false);
    }
}
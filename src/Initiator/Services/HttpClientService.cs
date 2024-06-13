using System;
using Initiator.Services.Interfaces;
using SharedModels;
using System.Net.Http;
using System.Net.Http.Json;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Initiator.Services;

public class HttpClientService : IHttpClientService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _calculatorUrl;
    private readonly ILogStrategy _logStrategy;
    private readonly string _startPrevious;
    private readonly string _startCurrent;
        
    public HttpClientService(
        IHttpClientFactory httpClientFactory,
        string calculatorUrl,
        ILogStrategy logStrategy,
        string startPrevious,
        string startCurrent
        )
    {
        _httpClientFactory = httpClientFactory;
        _calculatorUrl = calculatorUrl;
        _logStrategy = logStrategy;
        _startPrevious = startPrevious;
        _startCurrent = startCurrent;
    }

    public async Task SendStateToCalculatorAsync(FibonacciState state)
    {
        HttpClient client = _httpClientFactory.CreateClient();

        if (!state.Previous.Equals(_startPrevious, StringComparison.InvariantCulture) ||
            !state.Current.Equals(_startCurrent, StringComparison.InvariantCulture))
        {
            var previous = BigInteger.Parse(state.Previous);
            var current = BigInteger.Parse(state.Current);
            var newCurrent = BigInteger.Add(previous, current);
        
            state = new FibonacciState(state.Current, newCurrent.ToString(), state.StartId, DateTime.Now);
        }

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
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

    public HttpClientService(IHttpClientFactory httpClientFactory, string calculatorUrl, ILogStrategy logStrategy)
    {
        _httpClientFactory = httpClientFactory;
        _calculatorUrl = calculatorUrl;
        _logStrategy = logStrategy;
    }

    public async Task SendStateToCalculatorAsync(FibonacciState state)
    {
        HttpClient client = _httpClientFactory.CreateClient();

        if (!state.Previous.Equals("0", StringComparison.InvariantCulture)
            && !state.Previous.Equals("1", StringComparison.InvariantCulture))
        {
            var previous = BigInteger.Parse(state.Previous);
            var current = BigInteger.Parse(state.Current);
            var newCurrent = BigInteger.Add(previous, current);
        
            state = new FibonacciState(state.Current, newCurrent.ToString(), state.StartId, DateTime.Now);
        }

        var logBuilder = new StringBuilder();
        
        logBuilder.Append("Received Fibonacci state: Previous=")
            .Append(state.Previous)
            .Append(", Current=")
            .Append(state.Current)
            .Append(", StartId=")
            .Append(state.StartId);

        _logStrategy.Log(logBuilder.ToString());

        await client.PostAsJsonAsync(_calculatorUrl, state).ConfigureAwait(false);
    }
}
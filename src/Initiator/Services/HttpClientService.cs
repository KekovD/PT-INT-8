using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Initiator.Services.Interfaces;
using SharedModels;

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

        _logStrategy.Log(
            $"Received Fibonacci state: Previous={state.Previous}, Current={state.Current}, StartId={state.StartId}");

        await client.PostAsJsonAsync(_calculatorUrl, state).ConfigureAwait(false);
    }
}
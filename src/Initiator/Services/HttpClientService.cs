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

    public HttpClientService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task SendStateToCalculatorAsync(FibonacciState state)
    {
        HttpClient client = _httpClientFactory.CreateClient();
        
        Console.WriteLine(
            $"Received Fibonacci state: Previous={state.Previous}, Current={state.Current}, StartId={state.StartId}");
        
        await client.PostAsJsonAsync("http://calculator:8080/calculator/receive", state).ConfigureAwait(false);
    }
}
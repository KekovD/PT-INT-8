using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using EasyNetQ;
using Microsoft.AspNetCore.Mvc;
using SharedModels;

namespace Initiator.Controllers;

[ApiController]
[Route("[controller]")]
public class FibonacciController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IBus _bus;

    public FibonacciController(IHttpClientFactory httpClientFactory, IBus bus)
    {
        _httpClientFactory = httpClientFactory;
        _bus = bus;
    }

    [HttpPost]
    public async Task<IActionResult> StartCalculations([FromQuery] int count)
    {
        await SubscribeToMessages();
        
        string previous = "0";
        string current = "1";
        var tasks = new List<Task>();

        for (int i = 0; i < count; i++)
        {
            var state = new FibonacciState(previous, current);
            tasks.Add(SendStateToCalculator(state));
        }

        await Task.WhenAll(tasks).ConfigureAwait(false);
        return Ok("Calculations started");
    }

    private async Task SendStateToCalculator(FibonacciState state)
    {
        HttpClient client = _httpClientFactory.CreateClient();
        Console.WriteLine($"Received Fibonacci state: Previous={state.Previous}, Current={state.Current}");
        await client.PostAsJsonAsync("http://calculator:8080/calculator/receive", state).ConfigureAwait(false);
    }
    
    private async Task SubscribeToMessages(int retryCount = 5)
    {
        for (int i = 0; i < retryCount; i++)
        {
            try
            {
                await _bus.PubSub.SubscribeAsync<FibonacciState>("Initiator_queue", async state =>
                {
                    await SendStateToCalculator(state).ConfigureAwait(false);
                });
                return;
            }
            catch (TaskCanceledException)
            {
                Console.WriteLine($"Failed to subscribe to messages. Retry attempt {i + 1}/{retryCount}...");
                await Task.Delay(1000).ConfigureAwait(false);
            }
        }

        Console.WriteLine($"Failed to subscribe to messages after {retryCount} attempts.");
    }
}
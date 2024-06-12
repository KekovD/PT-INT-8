using System.Numerics;
using System.Threading.Tasks;
using EasyNetQ;
using Microsoft.AspNetCore.Mvc;
using SharedModels;

namespace Calculator.Controllers;

[ApiController]
[Route("[controller]")]
public class CalculatorController(IBus bus) : ControllerBase
{
    public async Task CalculateNext(FibonacciState state)
    {
        BigInteger newCurrent = BigInteger.Add(BigInteger.Parse(state.Previous), BigInteger.Parse(state.Current));
        var newState = new FibonacciState(state.Current, newCurrent.ToString());

        await bus.PubSub.PublishAsync(newState).ConfigureAwait(false);
    }

    [HttpPost("receive")]
    public async Task<IActionResult> ReceiveMessage([FromBody] FibonacciState state)
    {
        await CalculateNext(state).ConfigureAwait(false);
        return Ok();
    }
}
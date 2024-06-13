using Calculator.Services.Interfaces;
using EasyNetQ;
using Microsoft.AspNetCore.Mvc;
using SharedModels;
using System.Threading.Tasks;

namespace Calculator.Controllers;

[ApiController]
[Route("[controller]")]
public class CalculatorController : ControllerBase
{
    private readonly ICalculateNextService _calculateNextService;

    public CalculatorController(ICalculateNextService calculateNextService, IBus bus)
    {
        _calculateNextService = calculateNextService;
    }

    [HttpPost("receive")]
    public async Task<IActionResult> ReceiveMessage([FromBody] FibonacciState state)
    {
        await _calculateNextService.CalculateNext(state).ConfigureAwait(false);
        return Ok();
    }
}
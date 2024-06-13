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
    private readonly ISendNextService _sendNextService;

    public CalculatorController(ISendNextService sendNextService, IBus bus)
    {
        _sendNextService = sendNextService;
    }

    [HttpPost("receive")]
    public async Task<IActionResult> ReceiveMessage([FromBody] FibonacciState state)
    {
        await _sendNextService.SendNext(state).ConfigureAwait(false);
        return Ok();
    }
}
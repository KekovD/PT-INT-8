using System;
using Calculator.Services.Interfaces;
using EasyNetQ;
using Microsoft.AspNetCore.Mvc;
using SharedModels;
using System.Threading.Tasks;
using SharedModels.Interfaces;

namespace Calculator.Controllers;

[ApiController]
[Route("[controller]")]
public class CalculatorController : ControllerBase
{
    private readonly ISendNextService _sendNextService;
    private readonly ILogStrategy _logStrategy;

    public CalculatorController(ISendNextService sendNextService, ILogStrategy logStrategy)
    {
        _sendNextService = sendNextService;
        _logStrategy = logStrategy;
    }

    [HttpPost("receive")]
    public async Task<IActionResult> ReceiveMessageAsync([FromBody] FibonacciState state)
    {
        try
        {
            await _sendNextService.SendNextAsync(state).ConfigureAwait(false);
            return Ok();
        }
        catch (Exception)
        {
            var errorMessage = "An error occurred while receiving state";
            await _logStrategy.LogAsync(errorMessage).ConfigureAwait(false);
            return BadRequest(errorMessage);
        }
    }
}
using System;
using Initiator.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using SharedModels;

namespace Initiator.Controllers;

[ApiController]
[Route("[controller]")]
public class InitiatorController : ControllerBase
{
    private readonly IFibonacciStartService _fibonacciStartService;
    private readonly ILogStrategy _logStrategy;

    public InitiatorController(IFibonacciStartService fibonacciStartService, ILogStrategy logStrategy)
    {
        _fibonacciStartService = fibonacciStartService;
        _logStrategy = logStrategy;
    }

    [HttpPost]
    public async Task<IActionResult> StartCalculationsAsync([FromQuery] int numberOfLaunches)
    {
        try
        {
            await _fibonacciStartService.StartCalculationsAsync(numberOfLaunches).ConfigureAwait(false);
            return Ok("Calculations started");
        }
        catch (Exception)
        {
            var errorMessage = "An error occurred while starting calculations";
            await _logStrategy.LogAsync(errorMessage).ConfigureAwait(false);
            return BadRequest(errorMessage);
        }
    }
}
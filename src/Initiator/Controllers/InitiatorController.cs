using Initiator.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Initiator.Controllers;

[ApiController]
[Route("[controller]")]
public class InitiatorController : ControllerBase
{
    private readonly IFibonacciStartService _fibonacciStartService;

    public InitiatorController(IFibonacciStartService fibonacciStartService)
    {
        _fibonacciStartService = fibonacciStartService;
    }

    [HttpPost]
    public async Task<IActionResult> StartCalculationsAsync([FromQuery] int numberOfLaunches)
    {
        await _fibonacciStartService.StartCalculationsAsync(numberOfLaunches).ConfigureAwait(false);
        return Ok("Calculations started");
    }
}
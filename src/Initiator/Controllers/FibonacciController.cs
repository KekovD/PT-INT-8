using Initiator.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Initiator.Controllers;

[ApiController]
[Route("[controller]")]
public class FibonacciController : ControllerBase
{
    private readonly IFibonacciService _fibonacciService;

    public FibonacciController(IFibonacciService fibonacciService)
    {
        _fibonacciService = fibonacciService;
    }

    [HttpPost]
    public async Task<IActionResult> StartCalculations([FromQuery] int numberOfLaunches)
    {
        await _fibonacciService.StartCalculationsAsync(numberOfLaunches).ConfigureAwait(false);
        return Ok("Calculations started");
    }
}
using System.Threading.Tasks;
using Initiator.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
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
        await _fibonacciService.StartCalculationsAsync(numberOfLaunches);
        return Ok("Calculations started");
    }
}
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
    public async Task<IActionResult> StartCalculations([FromQuery] int count)
    {
        await _fibonacciService.StartCalculationsAsync(count);
        return Ok("Calculations started");
    }
}
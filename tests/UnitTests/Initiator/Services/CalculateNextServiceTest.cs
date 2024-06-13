using System;
using System.Threading.Tasks;
using Initiator.Services;
using SharedModels;

namespace Tests.Initiator.Services;

public class CalculateNextServiceTest
{
    [Fact]
    public async Task CalculateNextAsync_ReturnsStateUnchanged_WhenAlreadyMatchingStart()
    {
        var startPrevious = "0";
        var startCurrent = "1";
        int startId = 0;
        var state = new FibonacciState(Previous: "0", Current: "1", startId, DateTime.Now);
        var service = new CalculateNextService(startPrevious, startCurrent);

        var result = await service.CalculateNextAsync(state);

        Assert.Same(state, result);
    }

    [Fact]
    public async Task CalculateNextAsync_ReturnsNewState_WhenNotMatchingStart()
    {
        var startPrevious = "0";
        var startCurrent = "1";
        int startId = 0;
        var state = new FibonacciState(Previous: "1", Current: "1", startId, DateTime.Now);
        var service = new CalculateNextService(startPrevious, startCurrent);

        var result = await service.CalculateNextAsync(state);

        Assert.NotSame(state, result);
        Assert.Equal("1", result.Previous);
        Assert.Equal("2", result.Current);
        Assert.Equal(state.StartId, result.StartId);
    }
}
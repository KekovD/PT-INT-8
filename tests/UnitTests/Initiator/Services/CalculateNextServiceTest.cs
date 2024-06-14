using System;
using System.Threading.Tasks;
using Initiator.Services;
using SharedModels;
using SharedModels.Interfaces;

namespace Tests.Initiator.Services;

public class CalculateNextServiceTest
{
    [Fact]
    public async Task CalculateNextAsync_ReturnsStateUnchanged_WhenAlreadyMatchingStart()
    {
        var mockLogger = new Mock<ILogStrategy>();
        var startPrevious = "0";
        var startCurrent = "1";
        int startId = 0;
        var state = new FibonacciState(Previous: "0", Current: "1", startId, DateTime.Now);
        var service = new CalculateNextService(startPrevious, startCurrent, mockLogger.Object);

        var result = await service.CalculateNextAsync(state);

        Assert.Same(state, result);
    }

    [Fact]
    public async Task CalculateNextAsync_ReturnsNewState_WhenNotMatchingStart()
    {
        var mockLogger = new Mock<ILogStrategy>();
        var startPrevious = "0";
        var startCurrent = "1";
        int startId = 0;
        var state = new FibonacciState(Previous: "1", Current: "1", startId, DateTime.Now);
        var service = new CalculateNextService(startPrevious, startCurrent, mockLogger.Object);

        var result = await service.CalculateNextAsync(state);

        Assert.NotSame(state, result);
        Assert.Equal("1", result.Previous);
        Assert.Equal("2", result.Current);
        Assert.Equal(state.StartId, result.StartId);
    }
    
    [Fact]
    public async Task CalculateNextAsync_ThrowsFormatException_WhenParsingFails()
    {
        var mockLogger = new Mock<ILogStrategy>();
        var startPrevious = "0";
        var startCurrent = "1";
        int startId = 0;
        var invalidState = new FibonacciState(Previous: "not_a_number", Current: "1", startId, DateTime.Now);
        var service = new CalculateNextService(startPrevious, startCurrent, mockLogger.Object);

        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await service.CalculateNextAsync(invalidState);
        });
    }
}
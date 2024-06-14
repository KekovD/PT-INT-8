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
        var startPrevious = "0";
        var startCurrent = "1";
        var mockLogStrategy = new Mock<ILogStrategy>();
        var mockParserAndUpdater = new Mock<IFibonacciStateParserAndUpdater>();

        mockParserAndUpdater.Setup(x =>
                x.ParseAndUpdateStateAsync(It.IsAny<FibonacciState>()))
            .ReturnsAsync(new FibonacciState(Previous: "1", Current: "1", StartId: 1, DateTime.UtcNow));

        var initialState = new FibonacciState(Previous: "0", Current: "1", StartId: 1, DateTime.UtcNow);

        var service = new CalculateNextService(
            startPrevious, startCurrent, mockLogStrategy.Object, mockParserAndUpdater.Object);

        var result = await service.CalculateNextAsync(initialState);

        Assert.Same(initialState, result);
    }

    [Fact]
    public async Task CalculateNextAsync_ReturnsNewState_WhenNotMatchingStart()
    {
        var startPrevious = "0";
        var startCurrent = "1";
        var mockLogStrategy = new Mock<ILogStrategy>();
        var mockParserAndUpdater = new Mock<IFibonacciStateParserAndUpdater>();

        mockParserAndUpdater.Setup(x =>
                x.ParseAndUpdateStateAsync(It.IsAny<FibonacciState>()))
            .ReturnsAsync(new FibonacciState(Previous: "1", Current: "2", StartId: 1, DateTime.UtcNow));

        var initialState = new FibonacciState(Previous: "1", Current: "1", StartId: 1, DateTime.UtcNow);
        var updatedState = new FibonacciState(Previous: "1", Current: "2", StartId: 1, DateTime.UtcNow);

        var service = new CalculateNextService(
            startPrevious, startCurrent, mockLogStrategy.Object, mockParserAndUpdater.Object);

        var result = await service.CalculateNextAsync(initialState);

        Assert.Equal(updatedState.Previous, result.Previous);
        Assert.Equal(updatedState.Current, result.Current);
        Assert.Equal(updatedState.StartId, result.StartId);

        mockLogStrategy.Verify(x => x.LogAsync(It.IsAny<string>()), Times.Never);
    }
    
    [Fact]
    public async Task CalculateNextAsync_ThrowsFormatException_WhenParsingFails()
    {
        var mockLogStrategy = new Mock<ILogStrategy>();
        var mockParserAndUpdater = new Mock<IFibonacciStateParserAndUpdater>();
        
        mockParserAndUpdater.Setup(x =>
                x.ParseAndUpdateStateAsync(It.IsAny<FibonacciState>()))
            .ThrowsAsync(new FormatException());
        
        var startPrevious = "0";
        var startCurrent = "1";
        int startId = 0;
        var invalidState = new FibonacciState(Previous: "not_a_number", Current: "1", startId, DateTime.UtcNow);
        var service = new CalculateNextService(startPrevious, startCurrent, mockLogStrategy.Object, mockParserAndUpdater.Object);

        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await service.CalculateNextAsync(invalidState);
        });
        
        
        mockLogStrategy.Verify(x => x.LogAsync(It.IsAny<string>()), Times.Once);
    }
}
using Calculator.Services;
using SharedModels;
using SharedModels.Interfaces;
using System;
using System.Threading.Tasks;

namespace Tests.Calculator.Services;

public class CalculateNextServiceTest
{
    [Fact]
    public async Task CalculateNextAsync_ValidState_ReturnsUpdatedState()
    {
        var mockLogStrategy = new Mock<ILogStrategy>();
        var mockParserAndUpdater = new Mock<IFibonacciStateParserAndUpdater>();

        mockParserAndUpdater.Setup(x =>
                x.ParseAndUpdateStateAsync(It.IsAny<FibonacciState>()))
            .ReturnsAsync(new FibonacciState(Previous: "2", Current: "3", StartId: 1, DateTime.UtcNow));

        var initialState = new FibonacciState(Previous: "1", Current: "2", StartId: 1, DateTime.UtcNow);
        var updatedState = new FibonacciState(Previous: "2", Current: "3", StartId: 1, DateTime.UtcNow);

        var service = new CalculateNextService(mockLogStrategy.Object, mockParserAndUpdater.Object);

        var result = await service.CalculateNextAsync(initialState);

        Assert.Equal(updatedState.Previous, result.Previous);
        Assert.Equal(updatedState.Current, result.Current);
        Assert.Equal(updatedState.StartId, result.StartId);

        mockLogStrategy.Verify(x => x.LogAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task CalculateNextAsync_ParsingError_LogsAndThrowsException()
    {
        var mockLogStrategy = new Mock<ILogStrategy>();
        var mockParserAndUpdater = new Mock<IFibonacciStateParserAndUpdater>();

        mockParserAndUpdater.Setup(x =>
                x.ParseAndUpdateStateAsync(It.IsAny<FibonacciState>()))
            .ThrowsAsync(new FormatException());

        var initialState = new FibonacciState(Previous: "1", Current: "invalid", StartId: 1, DateTime.UtcNow);

        var service = new CalculateNextService(mockLogStrategy.Object, mockParserAndUpdater.Object);

        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await service.CalculateNextAsync(initialState);
        });
        
        mockLogStrategy.Verify(x => x.LogAsync(It.IsAny<string>()), Times.Once);
    }
}
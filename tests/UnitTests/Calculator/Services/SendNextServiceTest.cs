using Calculator.Services;
using Calculator.Services.Interfaces;
using EasyNetQ;
using SharedModels;
using SharedModels.Interfaces;
using System;
using System.Threading.Tasks;

namespace Tests.Calculator.Services;

public class SendNextServiceTest
{
    [Fact]
    public async Task SendNextAsync_ExceptionLogsError()
    {
        var mockBus = new Mock<IBus>();
        var mockLogStrategy = new Mock<ILogStrategy>();
        var mockCalculateNextService = new Mock<ICalculateNextService>();

        var service = new SendNextService(mockBus.Object, mockLogStrategy.Object, mockCalculateNextService.Object);

        var state = new FibonacciState(Previous: "1", Current: "2", StartId: 1, DateTime.UtcNow);

        var errorMessage = "Test exception message";
        mockCalculateNextService.Setup(x => x.CalculateNextAsync(state))
            .ThrowsAsync(new EasyNetQException(errorMessage));

        await Assert.ThrowsAsync<EasyNetQException>(() => service.SendNextAsync(state));

        mockLogStrategy.Verify(x =>
            x.LogAsync(It.Is<string>(msg => msg.Contains(errorMessage))), Times.Exactly(1));
    }
}
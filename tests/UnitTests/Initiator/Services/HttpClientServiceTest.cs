using Initiator.Services;
using Initiator.Services.Interfaces;
using SharedModels;
using SharedModels.Interfaces;
using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tests.Initiator.Services;

public class HttpClientServiceTest
{
    [Fact]
    public async Task SendStateToCalculatorAsync_Success()
    {
        var httpClientFactoryMock = new Mock<IHttpClientFactory>();
        var logStrategyMock = new Mock<ILogStrategy>();
        var calculateNextServiceMock = new Mock<ICalculateNextService>();

        var httpClientMock = new Mock<HttpClient>();
        httpClientFactoryMock.Setup(factory =>
            factory.CreateClient(It.IsAny<string>())).Returns(httpClientMock.Object);

        var calculatorUrl = "http://test.calculator";
        var state = new FibonacciState(Previous: "1", Current: "1", StartId: 1, DateTime.UtcNow);

        var logMessageBuilder = new StringBuilder();
        logStrategyMock.Setup(log => log.LogAsync(It.IsAny<string>()))
            .Callback<string>(msg => logMessageBuilder.Append(msg))
            .Returns(Task.CompletedTask);

        var calculatedState = new FibonacciState(Previous: "1", Current: "2", StartId: 1, DateTime.UtcNow);
        calculateNextServiceMock.Setup(service => service.CalculateNextAsync(state))
            .ReturnsAsync(calculatedState);

        var httpClientService = new HttpClientService(
            httpClientFactoryMock.Object,
            calculatorUrl,
            logStrategyMock.Object,
            calculateNextServiceMock.Object
        );

        await httpClientService.SendStateToCalculatorAsync(state);
        
        Assert.Contains("Sent Fibonacci state: Previous=1, Current=2, StartId=1", logMessageBuilder.ToString());

        httpClientFactoryMock.Verify(factory => factory.CreateClient(It.IsAny<string>()), Times.Once);

        httpClientMock.Verify(client =>
            client.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
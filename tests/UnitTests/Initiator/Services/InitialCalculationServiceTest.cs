using System;
using System.Threading;
using System.Threading.Tasks;
using Initiator.Controllers;
using Initiator.Services;
using Initiator.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using SharedModels;
using SharedModels.Interfaces;

namespace Tests.Initiator.Services;

public class InitialCalculationServiceTest
{
    [Fact]
    public async Task StartAsync_ShouldCallStartCalculationsAsync()
    {
        var numberOfLaunches = 5;

        var fibonacciStartServiceMock = new Mock<IFibonacciStartService>();
        
        fibonacciStartServiceMock
            .Setup(service => service.StartCalculationsAsync(It.IsAny<int>()))
            .Returns(Task.CompletedTask);
        
        var mockLogger = new Mock<ILogStrategy>();

        var initiatorController = new InitiatorController(fibonacciStartServiceMock.Object, mockLogger.Object);

        var serviceProviderMock = new Mock<IServiceProvider>();
        var scopeMock = new Mock<IServiceScope>();
        var scopeFactoryMock = new Mock<IServiceScopeFactory>();

        serviceProviderMock
            .Setup(provider => provider.GetService(typeof(IServiceScopeFactory)))
            .Returns(scopeFactoryMock.Object);
        scopeFactoryMock
            .Setup(provider => provider.CreateScope())
            .Returns(scopeMock.Object);
        scopeMock
            .Setup(provider => provider.ServiceProvider)
            .Returns(serviceProviderMock.Object);
        serviceProviderMock
            .Setup(provider => provider.GetService(typeof(InitiatorController)))
            .Returns(initiatorController);

        var initialCalculationService = new InitialCalculationService(serviceProviderMock.Object, numberOfLaunches);

        await initialCalculationService.StartAsync(CancellationToken.None);

        fibonacciStartServiceMock.Verify(service => service.StartCalculationsAsync(numberOfLaunches), Times.Once);
    }

    [Fact]
    public void StopAsync_ShouldReturnCompletedTask()
    {
        var serviceProviderMock = new Mock<IServiceProvider>();
        var initialCalculationService = new InitialCalculationService(serviceProviderMock.Object, 5);

        var result = initialCalculationService.StopAsync(CancellationToken.None);

        Assert.Equal(Task.CompletedTask, result);
    }
}
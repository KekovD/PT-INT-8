using System;
using Initiator.Controllers;
using Initiator.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using SharedModels;

namespace Tests.Initiator.Controllers;

public class InitiatorControllerTest
{
    [Fact]
    public async Task StartCalculationsAsync_ReturnsOkResult()
    {
        var mockFibonacciStartService = new Mock<IFibonacciStartService>();
        
        mockFibonacciStartService
            .Setup(service => service.StartCalculationsAsync(It.IsAny<int>()))
            .Returns(Task.CompletedTask);

        var mockLogger = new Mock<ILogStrategy>();
        
        var controller = new InitiatorController(mockFibonacciStartService.Object, mockLogger.Object);

        int numberOfLaunches = 5;

        var result = await controller.StartCalculationsAsync(numberOfLaunches);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Calculations started", okResult.Value);

        mockFibonacciStartService.Verify(service => service.StartCalculationsAsync(numberOfLaunches), Times.Once);
    }
    
    
    [Fact]
    public async Task StartCalculationsAsync_ReturnsBadRequestOnException()
    {
        var mockFibonacciStartService = new Mock<IFibonacciStartService>();
        
        mockFibonacciStartService
            .Setup(service => service.StartCalculationsAsync(It.IsAny<int>()))
            .ThrowsAsync(new Exception("Test exception"));
        
        var mockLogger = new Mock<ILogStrategy>();

        var controller = new InitiatorController(mockFibonacciStartService.Object, mockLogger.Object);

        int numberOfLaunches = 5;

        var result = await controller.StartCalculationsAsync(numberOfLaunches);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("An error occurred while starting calculations", badRequestResult.Value);

        mockFibonacciStartService.Verify(service => service.StartCalculationsAsync(numberOfLaunches), Times.Once);
    }
}
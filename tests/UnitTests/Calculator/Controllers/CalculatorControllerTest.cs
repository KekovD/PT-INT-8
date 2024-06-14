using System;
using System.Threading.Tasks;
using Calculator.Controllers;
using Calculator.Services.Interfaces;
using EasyNetQ;
using Microsoft.AspNetCore.Mvc;
using SharedModels;
using SharedModels.Interfaces;

namespace Tests.Calculator.Controllers;

public class CalculatorControllerTest
{
    [Fact]
    public async Task ReceiveMessageAsync_Success()
    {
        var mockSendNextService = new Mock<ISendNextService>();
        var mockLogStrategy = new Mock<ILogStrategy>();
        var mockBus = new Mock<IBus>();
        var controller = new CalculatorController(mockSendNextService.Object, mockBus.Object, mockLogStrategy.Object);
        var state = new FibonacciState(Previous: "1", Current: "2", StartId: 1, DateTime.Now); 

        var result = await controller.ReceiveMessageAsync(state);

        mockSendNextService.Verify(
            service => service.SendNextAsync(state),
            Times.Once,
            "SendNextAsync should be called once");

        Assert.IsType<OkResult>(result);
    }

    [Fact]
    public async Task ReceiveMessageAsync_Exception()
    {
        var mockSendNextService = new Mock<ISendNextService>();
        var mockLogStrategy = new Mock<ILogStrategy>();
        var mockBus = new Mock<IBus>();
        
        mockSendNextService.Setup(service => service.SendNextAsync(It.IsAny<FibonacciState>()))
            .ThrowsAsync(new InvalidOperationException("Mock Exception"));
        
        var controller = new CalculatorController(mockSendNextService.Object, mockBus.Object, mockLogStrategy.Object);
        var state = new FibonacciState(Previous: "1", Current: "2", StartId: 1, DateTime.Now); 
        
        var result = await controller.ReceiveMessageAsync(state);

        mockLogStrategy.Verify(
            strategy => strategy.LogAsync(It.IsAny<string>()),
            Times.Once,
            "LogAsync should be called once");

        Assert.IsType<BadRequestObjectResult>(result);
    }
}
using Initiator.Services;
using Initiator.Services.Interfaces;
using SharedModels;
using System;
using System.Threading.Tasks;

namespace Tests.Initiator.Services
{
    public class FibonacciStartServiceTest
    {
        [Theory]
        [InlineData(5)]
        [InlineData(10)]
        public async Task StartCalculationsAsync_ValidNumberOfLaunches_SendsRequests(int numberOfLaunches)
        {
            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockMessageQueueService = new Mock<IMessageQueueService>();
            var mockLogStrategy = new Mock<ILogStrategy>();

            int startPrevious = 0;
            int startCurrent = 1;

            var fibonacciService = new FibonacciStartService(
                mockHttpClientService.Object,
                mockMessageQueueService.Object,
                startPrevious,
                startCurrent,
                mockLogStrategy.Object);

            await fibonacciService.StartCalculationsAsync(numberOfLaunches);

            mockHttpClientService.Verify(
                svc => svc.SendStateToCalculatorAsync(It.IsAny<FibonacciState>()),
                Times.Exactly(numberOfLaunches));
        }
        
        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task StartCalculationsAsync_InvalidNumberOfLaunches_ThrowsException(int numberOfLaunches)
        {
            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockMessageQueueService = new Mock<IMessageQueueService>();
            var mockLogStrategy = new Mock<ILogStrategy>();

            int startPrevious = 0;
            int startCurrent = 1;

            var fibonacciService = new FibonacciStartService(
                mockHttpClientService.Object,
                mockMessageQueueService.Object,
                startPrevious,
                startCurrent,
                mockLogStrategy.Object);

            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
                async () => await fibonacciService.StartCalculationsAsync(numberOfLaunches));
        }
    }
}

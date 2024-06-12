using System;
using System.Threading;
using System.Threading.Tasks;
using Initiator.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Initiator;

public class InitialCalculationService(IServiceProvider serviceProvider) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using IServiceScope scope = serviceProvider.CreateScope();
        FibonacciController controller = scope.ServiceProvider.GetRequiredService<FibonacciController>();
        await controller.StartCalculations(10);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
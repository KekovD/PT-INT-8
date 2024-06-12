using System;
using System.Threading;
using System.Threading.Tasks;
using EasyNetQ;
using Initiator.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Initiator;

public class InitialCalculationService(IServiceProvider serviceProvider) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();
        var controller = scope.ServiceProvider.GetRequiredService<FibonacciController>();
        await controller.StartCalculations(2);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
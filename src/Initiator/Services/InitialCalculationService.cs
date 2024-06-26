using Initiator.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Initiator.Services;

public class InitialCalculationService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly int _numberOfLaunches;
    
    public InitialCalculationService(IServiceProvider serviceProvider, int numberOfLaunches)
    {
        _serviceProvider = serviceProvider;
        _numberOfLaunches = numberOfLaunches;
    }
    
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var controller = scope.ServiceProvider.GetRequiredService<InitiatorController>();
        await controller.StartCalculationsAsync(_numberOfLaunches).ConfigureAwait(false);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
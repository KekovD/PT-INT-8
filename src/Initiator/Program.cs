using System;
using System.Threading.Tasks;
using EasyNetQ;
using Initiator.Controllers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Initiator;

public class Program
{
    public static async Task Main(string[] args)
    {
        IWebHost host = new WebHostBuilder()
            .UseKestrel()
            .ConfigureServices((context, services) =>
            {
                services.AddControllers();
                services.AddHttpClient();
                services.AddSingleton<IBus>(provider =>
                    RabbitHutch.CreateBus(Environment.GetEnvironmentVariable("RabbitMQ__ConnectionString")));
                services.AddSingleton<FibonacciController>();
                services.AddHostedService<InitialCalculationService>();
            })
            .Configure(app =>
            {
                app.UseRouting();
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });
            })
            .Build();

        await host.RunAsync();
    }
}
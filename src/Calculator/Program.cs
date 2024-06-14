using Calculator.Controllers;
using Calculator.Services;
using Calculator.Services.Interfaces;
using EasyNetQ;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;
using SharedModels;
using SharedModels.Interfaces;

namespace Calculator;

public class Program
{
    public static async Task Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        
        builder.Services.AddSingleton<IBus>(provider =>
            RabbitHutch.CreateBus(Environment.GetEnvironmentVariable("RabbitMQ__ConnectionString")));
        
        builder.Services.AddTransient<CalculatorController>();
        builder.Services.AddTransient<IFibonacciStateParserAndUpdater, FibonacciStateParserAndUpdater> ();
        builder.Services.AddTransient<ICalculateNextService, CalculateNextService>();
        builder.Services.AddTransient<ISendNextService, SendNextService>();
        builder.Services.AddTransient<ILogStrategy, ConsoleLogStrategy>();
        
        WebApplication app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });

        await app.RunAsync().ConfigureAwait(false);
    }
}
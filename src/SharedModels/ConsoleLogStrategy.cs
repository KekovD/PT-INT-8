using System;
using System.Threading.Tasks;
using SharedModels.Interfaces;

namespace SharedModels;

public class ConsoleLogStrategy : ILogStrategy
{
    public Task LogAsync(string message)
    {
        Console.WriteLine(message);
        return Task.CompletedTask;
    }
}
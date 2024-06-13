using System;
using System.Threading.Tasks;

namespace SharedModels;

public class ConsoleLogStrategy : ILogStrategy
{
    public Task LogAsync(string message)
    {
        Console.WriteLine(message);
        return Task.CompletedTask;
    }
}
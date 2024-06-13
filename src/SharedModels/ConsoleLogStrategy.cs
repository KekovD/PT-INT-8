using System;

namespace SharedModels;

public class ConsoleLogStrategy : ILogStrategy
{
    public void Log(string message)
    {
        Console.WriteLine(message);
    }
}
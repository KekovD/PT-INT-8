using Initiator.Services.Interfaces;
using System;

namespace Initiator.Services;

public class ConsoleLogStrategy : ILogStrategy
{
    public void Log(string message)
    {
        Console.WriteLine(message);
    }
}
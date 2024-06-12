using System;
using Initiator.Services.Interfaces;

namespace Initiator.Services;

public class ConsoleLogStrategy : ILogStrategy
{
    public void Log(string message)
    {
        Console.WriteLine(message);
    }
}
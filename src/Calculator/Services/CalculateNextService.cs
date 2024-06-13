using System;
using System.Numerics;
using System.Threading.Tasks;
using Calculator.Services.Interfaces;
using SharedModels;

namespace Calculator.Services;

public class CalculateNextService : ICalculateNextService
{
    public FibonacciState CalculateNext(FibonacciState state)
    {
        var previous = BigInteger.Parse(state.Previous);
        var current = BigInteger.Parse(state.Current);
        var newCurrent = BigInteger.Add(previous, current);
        
        return new FibonacciState(state.Current, newCurrent.ToString(), state.StartId, DateTime.Now);
    }
}
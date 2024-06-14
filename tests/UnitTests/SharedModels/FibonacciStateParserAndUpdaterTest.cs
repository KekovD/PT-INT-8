using SharedModels;
using System;
using System.Threading.Tasks;

namespace Tests.SharedModels;

public class FibonacciStateParserAndUpdaterTest
{
    [Fact]
    public async Task ParseAndUpdateStateAsync_ShouldUpdateValuesCorrectly()
    {
        var initialState = new FibonacciState(Previous: "0", Current: "1", StartId: 1, DateTime.UtcNow);
        var parserAndUpdater = new FibonacciStateParserAndUpdater();

        var updatedState = await parserAndUpdater.ParseAndUpdateStateAsync(initialState);

        Assert.Equal("1", updatedState.Previous);
        Assert.Equal("1", updatedState.Current); 
        Assert.Equal(initialState.StartId, updatedState.StartId);
    }
}
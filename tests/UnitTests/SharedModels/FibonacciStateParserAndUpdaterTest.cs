using System;
using System.Threading.Tasks;
using SharedModels;

namespace Tests.SharedModels;

public class FibonacciStateParserAndUpdaterTest
{
    [Fact]
    public async Task ParseAndUpdateStateAsync_ShouldUpdateValuesCorrectly()
    {
        var initialState = new FibonacciState("0", "1", 1, DateTime.Now);
        var parserAndUpdater = new FibonacciStateParserAndUpdater();

        var updatedState = await parserAndUpdater.ParseAndUpdateStateAsync(initialState);

        Assert.Equal("1", updatedState.Previous);
        Assert.Equal("1", updatedState.Current); 
        Assert.Equal(initialState.StartId, updatedState.StartId);
    }
}
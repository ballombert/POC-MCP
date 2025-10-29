using System.ComponentModel;
using System.Reflection;
using ModelContextProtocol.Server;

namespace SampleMcpServer.Tests.Tools;

public class RandomNumberToolsTests
{
    private readonly RandomNumberTools _tools;

    public RandomNumberToolsTests()
    {
        _tools = new RandomNumberTools();
    }

    [Fact]
    public void GetRandomNumber_WithDefaultParameters_ReturnsNumberInExpectedRange()
    {
        // Act
        var result = _tools.GetRandomNumber();

        // Assert
        Assert.True(result >= 0, $"Expected result >= 0, but got {result}");
        Assert.True(result < 100, $"Expected result < 100, but got {result}");
    }

    [Theory]
    [InlineData(1, 10)]
    [InlineData(50, 100)]
    [InlineData(0, 1)]
    [InlineData(-10, 10)]
    public void GetRandomNumber_WithCustomRange_ReturnsNumberInRange(int min, int max)
    {
        // Act
        var result = _tools.GetRandomNumber(min, max);

        // Assert
        Assert.True(result >= min, $"Expected result >= {min}, but got {result}");
        Assert.True(result < max, $"Expected result < {max}, but got {result}");
    }

    [Fact]
    public void GetRandomNumber_CalledMultipleTimes_ReturnsVariedResults()
    {
        // Arrange
        var results = new HashSet<int>();
        
        // Act - Call multiple times to check for variation
        for (int i = 0; i < 100; i++)
        {
            results.Add(_tools.GetRandomNumber(0, 1000));
        }

        // Assert - Should have some variation (not all the same number)
        Assert.True(results.Count > 1, "Random number generator should produce varied results");
    }

    [Theory]
    [InlineData(10, 5)]  // min > max should throw
    public void GetRandomNumber_WithInvalidRange_ThrowsArgumentOutOfRangeException(int min, int max)
    {
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => _tools.GetRandomNumber(min, max));
    }

    [Theory]
    [InlineData(5, 5)]  // min == max should return min
    public void GetRandomNumber_WithEqualMinMax_ReturnsMin(int min, int max)
    {
        // Act
        var result = _tools.GetRandomNumber(min, max);

        // Assert
        Assert.Equal(min, result);
    }

    [Fact]
    public void GetRandomNumber_HasMcpServerToolAttribute()
    {
        // Arrange
        var method = typeof(RandomNumberTools).GetMethod(nameof(RandomNumberTools.GetRandomNumber));

        // Assert
        Assert.NotNull(method);
        var attribute = method.GetCustomAttribute<McpServerToolAttribute>();
        Assert.NotNull(attribute);
    }

    [Fact]
    public void GetRandomNumber_HasDescriptionAttribute()
    {
        // Arrange
        var method = typeof(RandomNumberTools).GetMethod(nameof(RandomNumberTools.GetRandomNumber));

        // Assert
        Assert.NotNull(method);
        var description = method.GetCustomAttribute<DescriptionAttribute>();
        Assert.NotNull(description);
        Assert.NotNull(description.Description);
        Assert.Contains("random number", description.Description.ToLowerInvariant());
    }

    [Fact]
    public void GetRandomNumber_ParametersHaveDescriptionAttributes()
    {
        // Arrange
        var method = typeof(RandomNumberTools).GetMethod(nameof(RandomNumberTools.GetRandomNumber));
        var parameters = method!.GetParameters();

        // Assert
        Assert.Equal(2, parameters.Length);
        
        // Check min parameter
        var minParam = parameters.First(p => p.Name == "min");
        var minDescription = minParam.GetCustomAttribute<DescriptionAttribute>();
        Assert.NotNull(minDescription);
        Assert.Contains("Minimum", minDescription.Description);

        // Check max parameter
        var maxParam = parameters.First(p => p.Name == "max");
        var maxDescription = maxParam.GetCustomAttribute<DescriptionAttribute>();
        Assert.NotNull(maxDescription);
        Assert.Contains("Maximum", maxDescription.Description);
    }

    [Fact]
    public void GetRandomNumber_ParametersHaveCorrectDefaultValues()
    {
        // Arrange
        var method = typeof(RandomNumberTools).GetMethod(nameof(RandomNumberTools.GetRandomNumber));
        var parameters = method!.GetParameters();

        // Assert
        var minParam = parameters.First(p => p.Name == "min");
        Assert.True(minParam.HasDefaultValue);
        Assert.Equal(0, minParam.DefaultValue);

        var maxParam = parameters.First(p => p.Name == "max");
        Assert.True(maxParam.HasDefaultValue);
        Assert.Equal(100, maxParam.DefaultValue);
    }

    [Fact]
    public void RandomNumberTools_IsInternal()
    {
        // Assert
        Assert.False(typeof(RandomNumberTools).IsPublic);
        Assert.False(typeof(RandomNumberTools).IsNestedAssembly);
    }

    [Fact]
    public void GetRandomNumber_ReturnsInt32()
    {
        // Arrange
        var method = typeof(RandomNumberTools).GetMethod(nameof(RandomNumberTools.GetRandomNumber));

        // Assert
        Assert.Equal(typeof(int), method!.ReturnType);
    }
}
# API Reference

This document provides detailed information about the MCP tools and methods available in the POC-MCP server.

## Available Tools

### RandomNumberTools

Tools for generating random numbers with various parameters.

#### `get_random_number`

Generates a random number between specified minimum and maximum values.

**Signature:**
```csharp
public int GetRandomNumber(int min = 0, int max = 100)
```

**Parameters:**
- `min` (int, optional): Minimum value (inclusive). Default: 0
- `max` (int, optional): Maximum value (exclusive). Default: 100

**Returns:** 
- `int`: A random number between min (inclusive) and max (exclusive)

**Examples:**

*Basic usage:*
```
"Give me a random number"
→ Uses default range [0, 100)
```

*Custom range:*
```
"Generate a random number between 1 and 10"
→ Uses range [1, 10)
```

*Large range:*
```
"Get a random number between 1000 and 9999"
→ Uses range [1000, 9999)
```

**Error Conditions:**
- If `min >= max`, behavior is undefined (may throw exception)
- Parameters must be valid integers

## Creating Custom Tools

### Basic Tool Structure

```csharp
using System.ComponentModel;
using ModelContextProtocol.Server;

internal class MyCustomTools
{
    [McpServerTool]
    [Description("Clear, concise description of what this tool does")]
    public ReturnType MyToolMethod(
        [Description("Description of this parameter")] ParameterType param1,
        [Description("Optional parameter description")] ParameterType param2 = defaultValue)
    {
        // Implementation here
        return result;
    }
}
```

### Supported Parameter Types

- **Primitive types**: `int`, `string`, `bool`, `double`, `float`, `long`
- **Nullable types**: `int?`, `string?`, etc.
- **Collections**: `List<T>`, `T[]`, `IEnumerable<T>`
- **Complex objects**: Custom classes (JSON deserialization)
- **Optional parameters**: Use default values

### Supported Return Types

- **Primitive types**: Serialized directly
- **Strings**: Returned as text content
- **Objects**: JSON serialized
- **Collections**: JSON array serialization
- **Void**: No content returned (success indication only)

### Best Practices

#### 1. Tool Naming
- Use descriptive method names (converted to snake_case for MCP)
- Example: `GetWeatherForecast` → `get_weather_forecast`

#### 2. Parameter Design
- Provide sensible defaults for optional parameters
- Use clear, descriptive parameter names
- Add comprehensive `[Description]` attributes

#### 3. Error Handling
```csharp
[McpServerTool]
[Description("Tool that might fail")]
public string MyTool(string input)
{
    if (string.IsNullOrEmpty(input))
    {
        throw new ArgumentException("Input cannot be null or empty");
    }
    
    try
    {
        // Tool logic here
        return "Success result";
    }
    catch (Exception ex)
    {
        // Will be converted to MCP error response
        throw new InvalidOperationException($"Tool failed: {ex.Message}");
    }
}
```

#### 4. Dependency Injection
```csharp
internal class DatabaseTools
{
    private readonly IDbService _dbService;
    private readonly ILogger<DatabaseTools> _logger;
    
    public DatabaseTools(IDbService dbService, ILogger<DatabaseTools> logger)
    {
        _dbService = dbService;
        _logger = logger;
    }
    
    [McpServerTool]
    [Description("Queries database for user information")]
    public async Task<User> GetUser([Description("User ID")] int userId)
    {
        _logger.LogInformation("Getting user {UserId}", userId);
        return await _dbService.GetUserAsync(userId);
    }
}
```

## Advanced Scenarios

### Async Tools
```csharp
[McpServerTool]
[Description("Performs asynchronous operation")]
public async Task<string> AsyncTool([Description("Input data")] string data)
{
    await Task.Delay(100); // Simulate async work
    return $"Processed: {data}";
}
```

### Complex Object Parameters
```csharp
public class SearchCriteria
{
    public string Query { get; set; }
    public int MaxResults { get; set; } = 10;
    public DateTime? StartDate { get; set; }
}

[McpServerTool]
[Description("Searches with complex criteria")]
public List<SearchResult> Search(
    [Description("Search criteria object")] SearchCriteria criteria)
{
    // Implementation
    return results;
}
```

### File Operations
```csharp
[McpServerTool]
[Description("Reads text file content")]
public string ReadFile([Description("File path")] string filePath)
{
    if (!File.Exists(filePath))
    {
        throw new FileNotFoundException($"File not found: {filePath}");
    }
    
    return File.ReadAllText(filePath);
}
```

## Registration and Configuration

### Tool Registration
In `Program.cs`:
```csharp
builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithTools<RandomNumberTools>()    // Built-in tools
    .WithTools<MyCustomTools>()        // Your custom tools
    .WithTools<DatabaseTools>();       // More tools
```

### Service Dependencies
```csharp
// Register services that tools depend on
builder.Services.AddSingleton<IDbService, SqlDbService>();
builder.Services.AddScoped<IEmailService, SmtpEmailService>();
```

### Configuration
```csharp
// Access configuration in tools
internal class ConfigurableTools
{
    private readonly IConfiguration _config;
    
    public ConfigurableTools(IConfiguration config)
    {
        _config = config;
    }
    
    [McpServerTool]
    public string GetConfigValue([Description("Config key")] string key)
    {
        return _config[key] ?? "Not found";
    }
}
```

## Testing Tools

### Unit Testing
```csharp
[Test]
public void GetRandomNumber_WithValidRange_ReturnsNumberInRange()
{
    // Arrange
    var tools = new RandomNumberTools();
    
    // Act
    var result = tools.GetRandomNumber(1, 10);
    
    // Assert
    Assert.That(result, Is.GreaterThanOrEqualTo(1));
    Assert.That(result, Is.LessThan(10));
}
```

### Integration Testing
See [Examples](examples/) directory for complete integration test examples.

## Troubleshooting

### Common Issues

**Tool not discovered:**
- Ensure method has `[McpServerTool]` attribute
- Verify class is registered with `.WithTools<T>()`
- Check method is public

**Parameter binding fails:**
- Verify parameter types are JSON-serializable
- Check parameter names match expected JSON keys
- Ensure required parameters don't have default values

**Runtime errors:**
- Check server logs (stderr output)
- Verify dependencies are properly injected
- Test tools individually before MCP integration

For more examples and implementation patterns, see the [Examples](examples/) directory.
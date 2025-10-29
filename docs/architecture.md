# Architecture

This document describes the technical architecture and design decisions for the POC-MCP .NET server.

## High-Level Architecture

```
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│   AI Assistant  │    │   MCP Server    │    │   Tool Classes  │
│  (Copilot, etc) │◄──►│   (stdio/JSON)  │◄──►│   (.NET Types)  │
└─────────────────┘    └─────────────────┘    └─────────────────┘
        │                        │                        │
        │                        │                        │
   JSON-RPC 2.0            Host Builder              Method Calls
   over stdio              & DI Container            & Reflection
```

## Core Components

### 1. Host Application (`Program.cs`)

- **Framework**: Microsoft.Extensions.Hosting
- **Purpose**: Application lifecycle management and dependency injection
- **Configuration**: Sets up logging, MCP services, and tool registration

```csharp
var builder = Host.CreateApplicationBuilder(args);

// Logging to stderr (stdout reserved for MCP protocol)
builder.Logging.AddConsole(o => o.LogToStandardErrorThreshold = LogLevel.Trace);

// MCP services registration
builder.Services
    .AddMcpServer()                    // Core MCP functionality
    .WithStdioServerTransport()        // stdio JSON-RPC transport
    .WithTools<RandomNumberTools>();   // Tool discovery and registration
```

### 2. Transport Layer

- **Protocol**: JSON-RPC 2.0 over stdin/stdout
- **Implementation**: `WithStdioServerTransport()`
- **Communication**: Bidirectional message passing with AI clients
- **Format**: Structured JSON messages following MCP specification

### 3. Tool System

#### Tool Discovery
- **Reflection-based**: Scans registered classes for `[McpServerTool]` methods
- **Automatic registration**: Tools added via `.WithTools<T>()`
- **Metadata extraction**: Uses `[Description]` attributes for AI discoverability

#### Tool Execution
- **Method invocation**: Direct .NET method calls via reflection
- **Parameter binding**: JSON to .NET type conversion
- **Return handling**: .NET return values serialized to JSON

### 4. Tool Classes (`Tools/` directory)

```csharp
internal class RandomNumberTools
{
    [McpServerTool]  // Marks method as MCP tool
    [Description("Generates a random number between min and max")]
    public int GetRandomNumber(
        [Description("Minimum value (inclusive)")] int min = 0,
        [Description("Maximum value (exclusive)")] int max = 100)
    {
        return Random.Shared.Next(min, max);
    }
}
```

## Design Patterns

### Dependency Injection
- **Container**: Microsoft.Extensions.DependencyInjection
- **Scope**: Singleton for tool classes (stateless recommended)
- **Services**: Can inject additional services into tool constructors

### Attribute-Based Configuration
- **Tools**: `[McpServerTool]` for method discovery
- **Documentation**: `[Description]` for AI-readable descriptions
- **Parameters**: `[Description]` on parameters for usage guidance

### Self-Contained Distribution
- **Build**: Single-file executable per platform
- **Runtime**: No .NET runtime required on target machine
- **Platforms**: Multi-target for Windows, macOS, Linux (multiple architectures)

## Message Flow

### 1. Client Request
```json
{
  "jsonrpc": "2.0",
  "id": "123",
  "method": "tools/call",
  "params": {
    "name": "get_random_number",
    "arguments": {
      "min": 1,
      "max": 10
    }
  }
}
```

### 2. Server Processing
1. **Transport**: Receives JSON on stdin
2. **Routing**: Identifies tool method by name
3. **Binding**: Converts JSON arguments to .NET parameters
4. **Execution**: Invokes tool method
5. **Serialization**: Converts return value to JSON

### 3. Server Response
```json
{
  "jsonrpc": "2.0",
  "id": "123",
  "result": {
    "content": [
      {
        "type": "text",
        "text": "7"
      }
    ]
  }
}
```

## Security Considerations

### Execution Context
- **Process isolation**: Each MCP server runs in separate process
- **Privilege level**: Inherits permissions of host application
- **Resource access**: Limited to what .NET process can access

### Input Validation
- **Type safety**: .NET type system provides basic validation
- **Parameter validation**: Should be implemented in tool methods
- **Error handling**: Exceptions converted to MCP error responses

## Performance Characteristics

### Startup Time
- **Cold start**: ~100-200ms (self-contained executable)
- **Tool discovery**: Reflection-based, cached after first scan
- **Memory usage**: Minimal baseline (~10-20MB)

### Runtime Performance
- **Tool execution**: Direct method invocation (fast)
- **Serialization**: System.Text.Json (efficient)
- **Transport**: stdio pipes (minimal overhead)

### Scalability
- **Concurrent requests**: Single-threaded by design (MCP pattern)
- **State management**: Stateless tools recommended
- **Resource cleanup**: Automatic via .NET GC and hosting lifetime

## Extension Points

### Custom Tools
1. Create new class in `Tools/` directory
2. Add `[McpServerTool]` methods
3. Register with `.WithTools<YourClass>()`

### Custom Services
```csharp
builder.Services.AddSingleton<IMyService, MyService>();

// Then inject into tool constructors
internal class MyTools
{
    private readonly IMyService _service;
    
    public MyTools(IMyService service)
    {
        _service = service;
    }
}
```

### Custom Transport
- Implement alternative to `WithStdioServerTransport()`
- Support for WebSocket, HTTP, or other protocols
- Currently stdio is standard for MCP

## Build and Packaging

### Project Configuration
```xml
<PropertyGroup>
  <TargetFramework>net9.0</TargetFramework>
  <RuntimeIdentifiers>win-x64;osx-arm64;linux-x64;...</RuntimeIdentifiers>
  <SelfContained>true</SelfContained>
  <PublishSingleFile>true</PublishSingleFile>
  <PackAsTool>true</PackAsTool>
  <PackageType>McpServer</PackageType>
</PropertyGroup>
```

### Distribution Strategy
- **NuGet packaging**: Standard .NET distribution mechanism
- **Platform-specific builds**: Separate executables per OS/architecture
- **IDE integration**: Automatic download and execution via `dnx` command

For implementation details, see the [API Reference](api-reference.md) and [Examples](examples/).
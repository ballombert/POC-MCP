# Copilot Instructions for POC-MCP

This project is a **.NET MCP (Model Context Protocol) Server** implementation using the official C# SDK.

## Project Overview

This is a .NET 9.0 console application that implements an MCP server using Microsoft's ModelContextProtocol C# SDK. The server exposes tools that AI clients (like Copilot) can invoke through the MCP protocol.

### Architecture Overview

- **Framework**: .NET 9.0 console application with Microsoft.Extensions.Hosting
- **Transport**: stdio-based communication (JSON-RPC over stdin/stdout)
- **SDK**: Uses `ModelContextProtocol` NuGet package (v0.4.0-preview.1)
- **Distribution**: Self-contained, single-file executable distributed via NuGet
- **Structure**: `src/SampleMcpServer/` contains the main server implementation

## Development Guidelines

### MCP Server Implementation Patterns
- **Tool Classes**: Create classes in `Tools/` folder with methods decorated with `[McpServerTool]`
- **Parameter Documentation**: Use `[Description]` attributes on both methods and parameters for AI discoverability
- **Service Registration**: Use `builder.Services.AddMcpServer().WithStdioServerTransport().WithTools<YourToolClass>()`
- **Logging**: All logs go to stderr (stdout reserved for MCP protocol messages)

### Key .NET Patterns Used
```csharp
// Tool definition pattern
internal class RandomNumberTools
{
    [McpServerTool]
    [Description("Tool description for AI")]
    public ReturnType MethodName(
        [Description("Parameter description")] ParameterType param = defaultValue)
    {
        // Implementation
    }
}
```

### Build and Distribution
- **Development**: Use `dotnet run` for local testing
- **Cross-platform builds**: Configured for win-x64, win-arm64, osx-arm64, linux-x64, linux-arm64, linux-musl-x64
- **NuGet packaging**: `dotnet pack -c Release` creates distributable package
- **Self-contained**: No .NET runtime required on target machines

### Testing Strategy
- **Local Testing**: Configure IDE with stdio transport using `dotnet run` command
- **VS Code**: Create `.vscode/mcp.json` with server configuration
- **Visual Studio**: Create `.mcp.json` in solution directory
- **Integration Testing**: Test via Copilot Chat (e.g., "Give me 3 random numbers" tests `get_random_number` tool)

### Configuration Files
- **`.mcp/server.json`**: MCP server metadata and NuGet package configuration
- **`SampleMcpServer.csproj`**: Multi-platform build settings and NuGet packaging metadata
- **`Program.cs`**: Host builder setup with MCP services and tool registration

## Common Patterns (To Be Updated)

As this codebase develops, update this section with:
- Specific MCP server/client patterns used in this project
- Configuration management approaches
- Error handling strategies
- Integration patterns with external AI tools

## Key Files (To Be Updated)

Document important files as they are created:
- MCP server implementation files
- Configuration files for MCP setup
- Client integration code
- Protocol definition files

---

*This file should be updated as the POC evolves to reflect actual implementation patterns and architectural decisions.*
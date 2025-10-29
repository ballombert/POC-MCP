# Getting Started

This guide will help you set up and run the POC-MCP .NET server locally.

## Prerequisites

- **.NET 9.0 SDK** - [Download here](https://dotnet.microsoft.com/download/dotnet/9.0)
  - The project includes a `global.json` file specifying the required SDK version
  - Minimum version: 9.0.100 (with rollForward to latestMinor)
- **Compatible IDE** - VS Code or Visual Studio with MCP support
- **Git** - For cloning the repository

## Quick Setup

### 1. Clone and Build

```bash
```bash
# Clone the repository
git clone https://github.com/ballombert/POC-MCP.git
cd POC-MCP

# Verify .NET SDK version (should be 9.0.100 or compatible)
dotnet --version

# Build the solution
dotnet build

# Verify the build
dotnet run --project src/SampleMcpServer --help
```

### 2. Configure Your IDE

#### VS Code Setup

Create `.vscode/mcp.json` in your workspace:

```json
{
  "servers": {
    "POC-MCP": {
      "type": "stdio",
      "command": "dotnet",
      "args": [
        "run",
        "--project",
        "/absolute/path/to/POC-MCP/src/SampleMcpServer"
      ]
    }
  }
}
```

#### Visual Studio Setup

Create `.mcp.json` in your solution directory:

```json
{
  "servers": {
    "POC-MCP": {
      "type": "stdio",
      "command": "dotnet",
      "args": [
        "run",
        "--project",
        "src/SampleMcpServer"
      ]
    }
  }
}
```

### 3. Test the Server

1. **Start your IDE** with MCP configuration
2. **Open Copilot Chat** or compatible AI assistant
3. **Test the tools**:
   - "Give me a random number"
   - "Generate 3 random numbers between 1 and 10"
   - "Get a random number between 50 and 100"

You should see the MCP server being invoked automatically.

## Development Workflow

### Adding New Tools

1. **Create a new tool class** in `src/SampleMcpServer/Tools/`:

```csharp
internal class MyCustomTools
{
    [McpServerTool]
    [Description("Description of what this tool does")]
    public string MyTool(
        [Description("Parameter description")] string input)
    {
        // Implementation here
        return $"Result: {input}";
    }
}
```

2. **Register the tool** in `Program.cs`:

```csharp
builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithTools<RandomNumberTools>()
    .WithTools<MyCustomTools>();  // Add this line
```

3. **Test the new tool** by rebuilding and asking the AI assistant to use it.

### Debugging

- **Logs go to stderr** - Check your terminal/console for server logs
- **Protocol messages** use stdout - Don't add console writes to stdout
- **Use breakpoints** - Attach debugger with `dotnet run` in your IDE

## Next Steps

- Read the [Architecture](architecture.md) documentation
- Explore [API Reference](api-reference.md) for available tools
- Check [Deployment](deployment.md) for packaging and distribution
- Browse [Examples](examples/) for more complex scenarios

## Troubleshooting

### Common Issues

**Server not starting:**
- Verify .NET 9.0 SDK is installed: `dotnet --version`
- Check project builds successfully: `dotnet build`

**AI assistant not finding tools:**
- Verify MCP configuration file path is correct
- Restart your IDE after configuration changes
- Check server logs for errors

**Permission issues:**
- Ensure dotnet has execute permissions
- On macOS/Linux, you may need: `chmod +x /path/to/dotnet`

For more help, see the main [README](../README.md) or check the server-specific [documentation](../src/SampleMcpServer/README.md).
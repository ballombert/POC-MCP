# POC-MCP - .NET Model Context Protocol Server

A proof of concept implementation of an MCP (Model Context Protocol) server using .NET 9.0 and the official Microsoft ModelContextProtocol SDK.

## Overview

This project demonstrates how to create and distribute MCP servers using C# and .NET. The server exposes tools that AI clients (like GitHub Copilot, Claude, etc.) can invoke through the standardized MCP protocol.

### What is MCP?

Model Context Protocol (MCP) is an open standard that enables AI assistants to securely connect to external data sources and tools. It provides a standardized way for AI models to interact with various resources through a common interface.

## Project Structure

```
POC-MCP/
├── .github/
│   └── copilot-instructions.md    # AI coding agent guidelines
├── docs/                          # Comprehensive documentation
│   ├── README.md                  # Documentation overview
│   ├── getting-started.md         # Setup and quick start guide
│   ├── architecture.md            # Technical architecture details
│   ├── api-reference.md           # MCP tools and API documentation
│   ├── deployment.md              # Build, versioning, and deployment guide
│   ├── adr/                       # Architecture Decision Records
│   │   ├── README.md              # ADR index and guidelines
│   │   ├── 001-dotnet-9-mcp-server.md
│   │   ├── 002-stdio-transport.md
│   │   ├── 003-attribute-based-tools.md
│   │   └── 004-self-contained-distribution.md
│   └── examples/                  # Code examples and patterns
├── scripts/
│   └── version.sh                 # Version management script
├── src/
│   └── SampleMcpServer/           # Main MCP server implementation
│       ├── .mcp/
│       │   └── server.json        # MCP server metadata
│       ├── Tools/
│       │   └── RandomNumberTools.cs # Example MCP tools
│       ├── Program.cs             # Application entry point
│       ├── SampleMcpServer.csproj # Project configuration
│       └── README.md              # Server-specific documentation
├── Directory.Build.props          # Centralized build properties and versioning
├── global.json                   # .NET SDK version specification
├── POC-MCP.sln                   # Visual Studio solution file
└── README.md                      # This file
```

## Features

- ✅ **Cross-platform**: Builds for Windows, macOS, and Linux (multiple architectures)
- ✅ **Self-contained**: No .NET runtime required on target machines
- ✅ **NuGet distribution**: Packaged and distributed via NuGet
- ✅ **stdio transport**: Uses JSON-RPC over stdin/stdout for communication
- ✅ **Tool discovery**: AI assistants can automatically discover available tools
- ✅ **IDE integration**: Works with VS Code and Visual Studio

## Quick Start

### Prerequisites

- .NET 9.0 SDK
- Compatible IDE (VS Code or Visual Studio with MCP support)

### Development

1. **Clone and navigate to the project:**
   ```bash
   git clone https://github.com/ballombert/POC-MCP.git
   cd POC-MCP
   ```

2. **Build the solution:**
   ```bash
   dotnet build
   ```

3. **Run the MCP server:**
   ```bash
   dotnet run --project src/SampleMcpServer
   ```

4. **Test with AI assistant:**
   - Configure your IDE with the MCP server (see server README for details)
   - Ask Copilot Chat: "Give me 3 random numbers"
   - The server should be invoked automatically

### Building and Distribution

### Building and Distribution

1. **Build for all platforms:**
   ```bash
   dotnet pack -c Release
   ```
   Or use the version script:
   ```bash
   ./scripts/version.sh pack
   ```

2. **Manage versions:**
   ```bash
   # Show current version
   ./scripts/version.sh show
   
   # Bump version
   ./scripts/version.sh bump patch
   
   # Set specific version
   ./scripts/version.sh set 1.0.0-rc1
   ```

2. **Publish to NuGet:**
   ```bash
   dotnet nuget push bin/Release/*.nupkg --api-key <your-key> --source https://api.nuget.org/v3/index.json
   ```

## Example Tools

The sample server includes a `RandomNumberTools` class demonstrating:

- Tool method decoration with `[McpServerTool]`
- Parameter documentation with `[Description]` attributes
- Default parameter values
- Return value handling

```csharp
[McpServerTool]
[Description("Generates a random number between the specified minimum and maximum values.")]
public int GetRandomNumber(
    [Description("Minimum value (inclusive)")] int min = 0,
    [Description("Maximum value (exclusive)")] int max = 100)
{
    return Random.Shared.Next(min, max);
}
```

## Architecture

- **Host**: Microsoft.Extensions.Hosting for application lifecycle
- **Transport**: stdio-based JSON-RPC communication
- **SDK**: Official ModelContextProtocol NuGet package
- **Logging**: Configured to stderr (stdout reserved for protocol)
- **Packaging**: Self-contained, single-file executable

## Documentation

- **[Complete Documentation](docs/)** - Comprehensive guides and references
- **[Getting Started](docs/getting-started.md)** - Setup and quick start guide
- **[Architecture](docs/architecture.md)** - Technical architecture details
- **[API Reference](docs/api-reference.md)** - MCP tools and methods documentation
- **[Deployment Guide](docs/deployment.md)** - Build, versioning, and distribution
- **[Architecture Decisions (ADR)](docs/adr/)** - Important architectural decisions and rationale
- **[Examples](docs/examples/)** - Code examples and usage patterns
- **[Server Implementation](src/SampleMcpServer/README.md)** - Server-specific details
- **[AI Coding Guidelines](.github/copilot-instructions.md)** - For AI development agents
- **[Official MCP Documentation](https://modelcontextprotocol.io/)** - External resources
- **[Protocol Specification](https://spec.modelcontextprotocol.io/)** - MCP protocol details

## Contributing

This is a proof of concept project. When contributing:

1. Follow the patterns established in the existing codebase
2. Update documentation when adding new features
3. Test with actual AI assistants before submitting
4. Consider cross-platform compatibility

## License

[Add your license information here]

---

For detailed implementation guidance and development patterns, see [.github/copilot-instructions.md](.github/copilot-instructions.md).
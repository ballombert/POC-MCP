# Deployment and Versioning Guide

This guide covers building, versioning, packaging, and distributing the POC-MCP server.

## Version Management

The project uses centralized version management through `Directory.Build.props` and provides a convenient script for version operations.

### Version Structure

- **Format**: `Major.Minor.Patch[-Suffix]`
- **Example**: `0.1.0-beta`, `1.2.3`, `2.0.0-rc1`
- **Assembly Version**: Uses `Major.Minor.0.0` for compatibility
- **File Version**: Uses `Major.Minor.Patch`

### Version Script Usage

The `scripts/version.sh` script provides convenient version management:

```bash
# Show current version
./scripts/version.sh show

# Bump version components
./scripts/version.sh bump patch    # 0.1.0 → 0.1.1
./scripts/version.sh bump minor    # 0.1.0 → 0.2.0
./scripts/version.sh bump major    # 0.1.0 → 1.0.0

# Set specific version
./scripts/version.sh set 1.0.0-rc1

# Manage version suffix
./scripts/version.sh suffix beta   # Add/change suffix
./scripts/version.sh release       # Remove suffix (release version)

# Build and pack
./scripts/version.sh pack
```

### Manual Version Management

Edit `Directory.Build.props` directly:

```xml
<PropertyGroup>
  <VersionMajor>0</VersionMajor>
  <VersionMinor>1</VersionMinor>
  <VersionPatch>0</VersionPatch>
  <VersionSuffix>beta</VersionSuffix>
</PropertyGroup>
```

## Build Process

### Development Build

```bash
# Clean and build
dotnet clean
dotnet build

# Build specific configuration
dotnet build --configuration Release
```

### Release Build

```bash
# Full release build with all platforms
dotnet clean
dotnet build --configuration Release
dotnet pack --configuration Release
```

### Platform-Specific Builds

For self-contained executables (manual publish):

```bash
# Windows
dotnet publish src/SampleMcpServer -c Release -r win-x64 -p:IsPublishing=true

# macOS (Apple Silicon)
dotnet publish src/SampleMcpServer -c Release -r osx-arm64 -p:IsPublishing=true

# Linux
dotnet publish src/SampleMcpServer -c Release -r linux-x64 -p:IsPublishing=true
```

## NuGet Packaging

### Package Creation

```bash
# Create NuGet package
dotnet pack src/SampleMcpServer --configuration Release

# Package location
ls src/SampleMcpServer/bin/Release/SampleMcpServer.*.nupkg
```

### Package Contents

- **Tool executable**: Framework-dependent .NET tool
- **Metadata**: `.mcp/server.json` for MCP configuration
- **Documentation**: README.md included in package

### Package Configuration

Key settings in `SampleMcpServer.csproj`:

```xml
<PropertyGroup>
  <PackAsTool>true</PackAsTool>
  <PackageType>McpServer</PackageType>
  <PackageId>SampleMcpServer</PackageId>
  <PackageTags>AI; MCP; server; stdio</PackageTags>
</PropertyGroup>
```

## Distribution

### NuGet.org Publication

```bash
# Publish to NuGet.org (requires API key)
dotnet nuget push src/SampleMcpServer/bin/Release/SampleMcpServer.*.nupkg \
  --api-key <your-api-key> \
  --source https://api.nuget.org/v3/index.json
```

### Private NuGet Feed

```bash
# Publish to private feed
dotnet nuget push src/SampleMcpServer/bin/Release/SampleMcpServer.*.nupkg \
  --source <your-private-feed-url>
```

### Local Installation

```bash
# Install from local package
dotnet tool install --global --add-source src/SampleMcpServer/bin/Release SampleMcpServer
```

## IDE Integration

### VS Code Configuration

Create `.vscode/mcp.json`:

```json
{
  "servers": {
    "SampleMcpServer": {
      "type": "stdio",
      "command": "dnx",
      "args": [
        "SampleMcpServer",
        "--version",
        "0.1.0-beta",
        "--yes"
      ]
    }
  }
}
```

### Visual Studio Configuration

Create `.mcp.json` in solution directory:

```json
{
  "servers": {
    "SampleMcpServer": {
      "type": "stdio",
      "command": "dnx",
      "args": [
        "SampleMcpServer",
        "--version",
        "0.1.0-beta",
        "--yes"
      ]
    }
  }
}
```

### Development Configuration

For local development (source code):

```json
{
  "servers": {
    "SampleMcpServer-Dev": {
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

## CI/CD Integration

### GitHub Actions Example

```yaml
name: Build and Release

on:
  push:
    tags: ['v*']

jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      
      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          global-json-file: global.json
          
      - name: Build
        run: dotnet build --configuration Release
        
      - name: Pack
        run: dotnet pack --configuration Release --no-build
        
      - name: Publish to NuGet
        run: dotnet nuget push **/*.nupkg --api-key ${{ secrets.NUGET_API_KEY }} --source https://api.nuget.org/v3/index.json
```

### Version from Git Tags

The `Directory.Build.props` supports CI versioning:

```xml
<!-- Git versioning integration -->
<PropertyGroup Condition="'$(GITHUB_RUN_NUMBER)' != ''">
  <VersionSuffix Condition="'$(VersionSuffix)' != ''">$(VersionSuffix).$(GITHUB_RUN_NUMBER)</VersionSuffix>
  <VersionSuffix Condition="'$(VersionSuffix)' == ''">ci.$(GITHUB_RUN_NUMBER)</VersionSuffix>
</PropertyGroup>
```

## Release Workflow

### Preparation

1. **Update Version**: Use script or manual edit
2. **Test Build**: Ensure everything compiles
3. **Update Documentation**: Changelog, README updates
4. **Commit Changes**: Version bump and docs

### Release Steps

1. **Create Release Build**:
   ```bash
   ./scripts/version.sh release  # Remove suffix
   ./scripts/version.sh pack     # Build and pack
   ```

2. **Test Package**:
   ```bash
   # Test local installation
   dotnet tool install --global --add-source src/SampleMcpServer/bin/Release SampleMcpServer
   ```

3. **Publish**:
   ```bash
   # Publish to NuGet
   dotnet nuget push src/SampleMcpServer/bin/Release/SampleMcpServer.*.nupkg --api-key <key> --source https://api.nuget.org/v3/index.json
   ```

4. **Tag Release**:
   ```bash
   git tag v$(./scripts/version.sh show | grep "Current version" | cut -d' ' -f3)
   git push origin --tags
   ```

### Post-Release

1. **Bump to Next Version**:
   ```bash
   ./scripts/version.sh bump minor
   ./scripts/version.sh suffix beta
   ```

2. **Update .mcp.json Configurations**: Update version in documentation examples

## Troubleshooting

### Build Issues

- **Version conflicts**: Check `Directory.Build.props` syntax
- **Pack failures**: Ensure `PackAsTool=true` and no `SelfContained=true` during pack
- **Missing dependencies**: Run `dotnet restore`

### Package Issues

- **Tool installation fails**: Check package ID and version
- **MCP not recognized**: Verify `.mcp/server.json` is included in package
- **Runtime errors**: Ensure target framework compatibility

### IDE Integration Issues

- **Server not found**: Check package installation and version
- **Configuration errors**: Validate JSON syntax in `.mcp.json`
- **Permission issues**: Ensure `dnx` has execute permissions

For more information, see:
- [Getting Started Guide](getting-started.md)
- [Architecture Documentation](architecture.md)
- [API Reference](api-reference.md)
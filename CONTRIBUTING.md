# Contributing to POC-MCP

Thank you for your interest in contributing to POC-MCP! This document provides guidelines for contributing to this .NET MCP server implementation.

## Development Setup

### Prerequisites

- .NET 9.0 SDK (specified in `global.json`)
- Git
- Compatible IDE (VS Code or Visual Studio)

### Getting Started

1. **Fork and clone the repository:**
   ```bash
   git clone https://github.com/ballombert/POC-MCP.git
   cd POC-MCP
   ```

2. **Verify setup:**
   ```bash
   dotnet --version  # Should match global.json requirement
   dotnet build      # Should build successfully
   ```

3. **Test the MCP server:**
   ```bash
   dotnet run --project src/SampleMcpServer
   ```

## Development Workflow

### Branch Strategy

- **main**: Stable, deployable code
- **develop**: Integration branch for features
- **feature/xxx**: Individual feature branches
- **hotfix/xxx**: Critical fixes for production

### Making Changes

1. **Create feature branch:**
   ```bash
   git checkout -b feature/your-feature-name
   ```

2. **Make your changes** following the coding standards below

3. **Test your changes:**
   ```bash
   dotnet build
   dotnet test  # When tests are available
   ./scripts/version.sh pack  # Test packaging
   ```

4. **Commit with clear messages:**
   ```bash
   git add .
   git commit -m "feat: add new MCP tool for file operations"
   ```

5. **Push and create PR:**
   ```bash
   git push origin feature/your-feature-name
   ```

## Coding Standards

### .NET Conventions

- Follow standard C# naming conventions
- Use `PascalCase` for public members
- Use `camelCase` for private fields
- Enable nullable reference types
- Use `var` when type is obvious

### MCP-Specific Patterns

#### Tool Implementation
```csharp
internal class MyTools
{
    [McpServerTool]
    [Description("Clear, actionable description")]
    public ReturnType MethodName(
        [Description("Parameter description")] ParameterType param)
    {
        // Validate inputs
        // Implement functionality
        // Return appropriate result
    }
}
```

#### Error Handling
```csharp
[McpServerTool]
public string MyTool(string input)
{
    if (string.IsNullOrEmpty(input))
        throw new ArgumentException("Input cannot be null or empty");
    
    try
    {
        // Tool logic
        return result;
    }
    catch (Exception ex)
    {
        throw new InvalidOperationException($"Tool failed: {ex.Message}");
    }
}
```

### Documentation Standards

- **All public APIs** must have XML documentation
- **MCP tools** must have `[Description]` attributes
- **Parameters** should have descriptive `[Description]` attributes
- **Complex scenarios** should have usage examples

## Version Management

Use the version script for all version changes:

```bash
# Show current version
./scripts/version.sh show

# Bump version for your changes
./scripts/version.sh bump patch    # Bug fixes
./scripts/version.sh bump minor    # New features
./scripts/version.sh bump major    # Breaking changes

# Set development suffix
./scripts/version.sh suffix dev
```

## Adding New MCP Tools

### 1. Create Tool Class

Create a new file in `src/SampleMcpServer/Tools/`:

```csharp
using System.ComponentModel;
using ModelContextProtocol.Server;

internal class YourNewTools
{
    [McpServerTool]
    [Description("Describe what this tool does")]
    public string YourMethod([Description("Input description")] string input)
    {
        // Implementation
        return result;
    }
}
```

### 2. Register Tool

Update `Program.cs`:

```csharp
builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithTools<RandomNumberTools>()
    .WithTools<YourNewTools>();  // Add this line
```

### 3. Test Tool

1. Build and run the server
2. Configure in your IDE
3. Test with Copilot Chat
4. Verify expected behavior

### 4. Document Tool

Add documentation to `docs/api-reference.md`:

```markdown
#### `your_method_name`

Description of the tool.

**Parameters:**
- `input` (string): Description

**Returns:**
- `string`: Description

**Example:**
```
"Ask the AI to use your tool"
```

## Testing

### Manual Testing

1. **Build verification:**
   ```bash
   dotnet build --configuration Release
   ```

2. **Package testing:**
   ```bash
   ./scripts/version.sh pack
   ```

3. **Integration testing:**
   - Configure MCP server in IDE
   - Test with AI assistant
   - Verify all tools work as expected

### Future: Automated Testing

When automated tests are added:
- Write unit tests for new tools
- Ensure integration tests pass
- Maintain test coverage

## Pull Request Guidelines

### PR Checklist

- [ ] Code follows established patterns
- [ ] All tools have proper `[Description]` attributes
- [ ] Documentation updated (if applicable)
- [ ] Version bumped appropriately
- [ ] Manual testing completed
- [ ] PR description explains changes

### PR Description Template

```markdown
## Summary
Brief description of changes

## Type of Change
- [ ] Bug fix
- [ ] New feature
- [ ] Breaking change
- [ ] Documentation update

## Testing
- [ ] Built successfully
- [ ] Tested with MCP client
- [ ] All existing tools still work

## Additional Notes
Any special considerations or follow-up items
```

## Architecture Decisions

For significant architectural changes:

1. **Create ADR** in `docs/adr/`
2. **Follow ADR template** (see existing ADRs)
3. **Discuss in issue** before implementation
4. **Update documentation** after acceptance

## Code of Conduct

- Be respectful and inclusive
- Focus on constructive feedback
- Help newcomers learn the codebase
- Follow the established patterns
- Ask questions when unsure

## Getting Help

- **Documentation**: Check `docs/` directory
- **Issues**: Open GitHub issue for bugs/features
- **Discussions**: Use GitHub Discussions for questions
- **Architecture**: Review ADRs in `docs/adr/`

## Release Process

Maintainers handle releases:

1. **Version management** with scripts
2. **GitHub releases** with changelog
3. **NuGet publication** for distribution
4. **Documentation updates** as needed

Thank you for contributing to POC-MCP!
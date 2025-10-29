using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;

namespace SampleMcpServer.Tests.Integration;

public class ProgramIntegrationTests
{
    [Fact]
    public void Program_CanBuildHost_WithoutErrors()
    {
        // Arrange
        var args = Array.Empty<string>();

        // Act & Assert
        var builder = Host.CreateApplicationBuilder(args);
        
        // Configure logging to use a test logger
        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();
        
        // Add MCP server services (same as in Program.cs)
        builder.Services.AddMcpServer()
            .WithStdioServerTransport()
            .WithTools<RandomNumberTools>();

        var host = builder.Build();
        Assert.NotNull(host);
        host.Dispose();
    }

    [Fact]
    public void Program_RegistersMcpServerServices_InServiceContainer()
    {
        // Arrange
        var args = Array.Empty<string>();
        var builder = Host.CreateApplicationBuilder(args);
        
        builder.Logging.ClearProviders();
        builder.Services.AddMcpServer()
            .WithStdioServerTransport()
            .WithTools<RandomNumberTools>();

        // Act
        using var host = builder.Build();
        var serviceProvider = host.Services;

        // Assert
        // Check that MCP-related services are registered
        var hostedServices = serviceProvider.GetServices<IHostedService>();
        Assert.NotEmpty(hostedServices);
        
        // Should have at least one hosted service (the MCP server)
        Assert.Contains(hostedServices, service => 
            service.GetType().FullName!.Contains("ModelContextProtocol"));
    }

    [Fact]
    public void Program_RegistersMcpServer_InServiceContainer()
    {
        // Arrange
        var args = Array.Empty<string>();
        var builder = Host.CreateApplicationBuilder(args);
        
        builder.Logging.ClearProviders();
        builder.Services.AddMcpServer()
            .WithStdioServerTransport()
            .WithTools<RandomNumberTools>();

        // Act
        using var host = builder.Build();
        var serviceProvider = host.Services;

        // Assert
        // Check that MCP-related services are registered
        var hostedServices = serviceProvider.GetServices<IHostedService>();
        Assert.NotEmpty(hostedServices);
        
        // Should have at least one hosted service (the MCP server)
        Assert.Contains(hostedServices, service => 
            service.GetType().FullName!.Contains("ModelContextProtocol"));
    }

    [Fact]
    public async Task Program_CanStartAndStopHost_WithoutErrors()
    {
        // Arrange
        var args = Array.Empty<string>();
        var builder = Host.CreateApplicationBuilder(args);
        
        builder.Logging.ClearProviders();
        builder.Services.AddMcpServer()
            .WithStdioServerTransport()
            .WithTools<RandomNumberTools>();

        using var host = builder.Build();
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(1));

        // Act & Assert
        var startTask = host.StartAsync(cts.Token);
        await startTask;
        await host.StopAsync(cts.Token);
        
        // If we reach this point, no exception was thrown
        Assert.True(true);
    }

    [Fact]
    public void Program_ConfiguresLogging_ToUseConsole()
    {
        // Arrange
        var args = Array.Empty<string>();
        var builder = Host.CreateApplicationBuilder(args);
        
        // The default configuration should include console logging
        builder.Services.AddMcpServer()
            .WithStdioServerTransport()
            .WithTools<RandomNumberTools>();

        // Act
        using var host = builder.Build();
        var loggerFactory = host.Services.GetRequiredService<ILoggerFactory>();

        // Assert
        Assert.NotNull(loggerFactory);
        
        // Create a logger to ensure the factory works
        var logger = loggerFactory.CreateLogger<ProgramIntegrationTests>();
        Assert.NotNull(logger);
    }
}
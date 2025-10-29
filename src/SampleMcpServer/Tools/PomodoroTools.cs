using System.ComponentModel;
using System.Globalization;
using System.Resources;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;
using PomodoroTimer;

namespace SampleMcpServer.Tools;

/// <summary>
/// MCP tools for Pomodoro Timer functionality.
/// Provides console-based Pomodoro sessions that can be controlled by AI assistants.
/// </summary>
internal class PomodoroTools
{
    private readonly ILogger<PomodoroTools> _logger;
    private static PomodoroSession? _currentConsoleSession;
    private static CancellationTokenSource? _sessionCancellation;

    public PomodoroTools(ILogger<PomodoroTools> logger)
    {
        _logger = logger;
    }

    [McpServerTool]
    [Description("Starts a console-based Pomodoro session")]
    public async Task<string> StartPomodoroConsole(
        [Description("Work time in minutes (default: 25)")] int workMinutes = 25,
        [Description("Break time in minutes (default: 5)")] int breakMinutes = 5,
        [Description("Language code (en, fr, es, etc.) (default: en)")] string language = "en")
    {
        if (workMinutes <= 0 || breakMinutes <= 0)
        {
            throw new ArgumentException("Work and break times must be positive");
        }

        // Stop any existing session
        StopCurrentSession();

        try
        {
            SetCulture(language);
            
            var resourceManager = new ResourceManager("PomodoroTimer.Resources.Messages", 
                typeof(Program).Assembly);

            _sessionCancellation = new CancellationTokenSource();
            _currentConsoleSession = new PomodoroSession(workMinutes, breakMinutes, resourceManager, _logger);

            // Start session in background
            _ = Task.Run(async () => 
            {
                try
                {
                    await _currentConsoleSession.StartSession(_sessionCancellation.Token);
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("Pomodoro session was cancelled");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in Pomodoro session");
                }
            }, _sessionCancellation.Token);

            _logger.LogInformation("Pomodoro console session started: {WorkMinutes}min work, {BreakMinutes}min break, Language: {Language}", 
                workMinutes, breakMinutes, language);

            return $"Pomodoro console session started: {workMinutes} min work, {breakMinutes} min break (Language: {language})";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to start Pomodoro console session");
            throw new InvalidOperationException($"Failed to start Pomodoro session: {ex.Message}");
        }
    }

    [McpServerTool]
    [Description("Stops the current Pomodoro session")]
    public string StopPomodoroSession()
    {
        if (_currentConsoleSession is null)
        {
            return "No Pomodoro session is currently running";
        }

        StopCurrentSession();
        _logger.LogInformation("Pomodoro session stopped");
        return "Pomodoro session stopped";
    }

    [McpServerTool]
    [Description("Gets the status of the current Pomodoro session")]
    public string GetPomodoroStatus()
    {
        var hasConsoleSession = _currentConsoleSession is not null;

        if (!hasConsoleSession)
        {
            return "No Pomodoro session is currently running";
        }

        return "Pomodoro session is active (Console mode)";
    }

    [McpServerTool]
    [Description("Gets available languages for Pomodoro interface")]
    public string GetAvailableLanguages()
    {
        return "Available languages: en (English), fr (French), es (Spanish), ja (Japanese), ru (Russian), zh (Chinese)";
    }

    [McpServerTool]
    [Description("Launches the Pomodoro Timer with graphical user interface")]
    public string StartPomodoroGraphical(
        [Description("Work time in minutes (default: 25)")] int workMinutes = 25,
        [Description("Break time in minutes (default: 5)")] int breakMinutes = 5,
        [Description("Language code (en, fr, es, etc.) (default: en)")] string language = "en")
    {
        if (workMinutes <= 0 || breakMinutes <= 0)
        {
            throw new ArgumentException("Work and break times must be positive");
        }

        try
        {
            // Stop any existing session
            StopCurrentSession();

            SetCulture(language);

            _logger.LogInformation("Starting Pomodoro graphical interface: {WorkMinutes}min work, {BreakMinutes}min break, Language: {Language}", 
                workMinutes, breakMinutes, language);

            // Find the solution directory and launch the PomodoroTimer project
            var currentDirectory = Directory.GetCurrentDirectory();
            string? solutionDirectory = null;
            
            // Look for the solution file going up the directory tree
            var directory = new DirectoryInfo(currentDirectory);
            while (directory != null && solutionDirectory == null)
            {
                if (directory.GetFiles("*.sln").Length > 0)
                {
                    solutionDirectory = directory.FullName;
                    break;
                }
                directory = directory.Parent;
            }
            
            if (solutionDirectory == null)
            {
                throw new InvalidOperationException("Could not find solution directory");
            }

            var pomodoroProjectPath = Path.Combine(solutionDirectory, "src", "PomodoroTimer");
            
            if (!Directory.Exists(pomodoroProjectPath))
            {
                throw new InvalidOperationException($"PomodoroTimer project not found at: {pomodoroProjectPath}");
            }

            // Launch the graphical interface using the PomodoroTimer project
            var startInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"run --project \"{pomodoroProjectPath}\" -- {workMinutes} {breakMinutes} --ui --lang={language}",
                UseShellExecute = false,
                CreateNoWindow = false,
                WorkingDirectory = solutionDirectory
            };

            var process = Process.Start(startInfo);
            
            if (process != null)
            {
                return $"Pomodoro graphical interface launched: {workMinutes} min work, {breakMinutes} min break (Language: {language}). The window should appear shortly.";
            }
            else
            {
                throw new InvalidOperationException("Failed to start the graphical interface process");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to start Pomodoro graphical interface");
            throw new InvalidOperationException($"Failed to start Pomodoro graphical interface: {ex.Message}");
        }
    }

    private void StopCurrentSession()
    {
        _sessionCancellation?.Cancel();
        _sessionCancellation?.Dispose();
        _sessionCancellation = null;
        _currentConsoleSession = null;
    }

    private static void SetCulture(string? languageCode)
    {
        CultureInfo culture;
        
        if (!string.IsNullOrEmpty(languageCode))
        {
            try
            {
                culture = new CultureInfo(languageCode);
            }
            catch (CultureNotFoundException)
            {
                // Fallback to English if language not supported
                culture = new CultureInfo("en");
            }
        }
        else
        {
            culture = new CultureInfo("en");
        }

        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = culture;
        Thread.CurrentThread.CurrentCulture = culture;
        Thread.CurrentThread.CurrentUICulture = culture;
    }
}
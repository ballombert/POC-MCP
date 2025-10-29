using System;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Themes.Fluent;
using Avalonia.Markup.Xaml;

namespace PomodoroTimer;

public class Program
{
    private static ResourceManager? _resourceManager;
    
    static async Task Main(string[] args)
    {
        // Parse arguments
        string? languageArg = null;
        bool useUI = false;
        var filteredArgs = args.Where(arg => 
        {
            if (arg.StartsWith("--lang="))
            {
                languageArg = arg.Substring(7);
                return false;
            }
            if (arg == "--ui")
            {
                useUI = true;
                return false;
            }
            return true;
        }).ToArray();

        // Set culture based on language argument or system default
        SetCulture(languageArg);
        
        _resourceManager = new ResourceManager("PomodoroTimer.Resources.Messages", typeof(Program).Assembly);

        if (filteredArgs.Length != 2)
        {
            Console.WriteLine(_resourceManager.GetString("Usage"));
            Console.WriteLine(_resourceManager.GetString("Example"));
            Console.WriteLine();
            Console.WriteLine(_resourceManager.GetString("LanguageOption"));
            Console.WriteLine(_resourceManager.GetString("LanguageInstructions"));
            Console.WriteLine();
            Console.WriteLine("Additional options:");
            Console.WriteLine("  --ui          Launch graphical user interface instead of console mode");
            return;
        }

        if (!int.TryParse(filteredArgs[0], out int workTime) || workTime <= 0)
        {
            Console.WriteLine(_resourceManager.GetString("ErrorWorkTime"));
            return;
        }

        if (!int.TryParse(filteredArgs[1], out int breakTime) || breakTime <= 0)
        {
            Console.WriteLine(_resourceManager.GetString("ErrorBreakTime"));
            return;
        }

        if (useUI)
        {
            // For UI mode, we need to initialize Avalonia properly on the main thread
            LaunchGraphicalInterface(workTime, breakTime);
        }
        else
        {
            // Launch console interface
            var pomodoro = new PomodoroSession(workTime, breakTime, _resourceManager);
            await pomodoro.StartSession();
        }
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
            // Default to English instead of system culture
            culture = new CultureInfo("en");
        }

        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = culture;
        Thread.CurrentThread.CurrentCulture = culture;
        Thread.CurrentThread.CurrentUICulture = culture;
    }

    public static void SetCulturePublic(string? languageCode)
    {
        SetCulture(languageCode);
    }

    private static void LaunchGraphicalInterface(int workTime, int breakTime)
    {
        try
        {
            Console.WriteLine("Initializing Avalonia UI...");
            
            // Store parameters for the App class to use
            App.WorkTime = workTime;
            App.BreakTime = breakTime;
            App.ResourceManager = _resourceManager!;

            Console.WriteLine("Creating AppBuilder...");
            
            // Create and run the Avalonia application
            var appBuilder = AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToTrace();
                
            Console.WriteLine("Starting application...");
            
            appBuilder.StartWithClassicDesktopLifetime(Array.Empty<string>());
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error launching UI: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            Console.WriteLine("Falling back to console mode...");
            
            // Fallback to console mode
            var pomodoro = new PomodoroSession(workTime, breakTime, _resourceManager!);
            pomodoro.StartSession().Wait();
        }
    }
}

// Simple Application class for Avalonia
public class App : Application
{
    public static int WorkTime { get; set; }
    public static int BreakTime { get; set; }
    public static ResourceManager? ResourceManager { get; set; }

    public override void Initialize()
    {
        // Basic initialization without XAML
        Styles.Add(new FluentTheme());
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            try
            {
                var cancellationTokenSource = new CancellationTokenSource();
                var pomodoroWindow = new PomodoroWindow(WorkTime, BreakTime, ResourceManager!, cancellationTokenSource.Token);
                
                desktop.MainWindow = pomodoroWindow;
                
                // Ensure app shuts down when window closes
                pomodoroWindow.Closed += (s, e) => desktop.Shutdown();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating window: {ex.Message}");
                desktop.Shutdown();
            }
        }

        base.OnFrameworkInitializationCompleted();
    }
}

public class PomodoroSession
{
    private readonly int _workTime;
    private readonly int _breakTime;
    private readonly ResourceManager _resourceManager;
    private readonly ILogger? _logger;

    public PomodoroSession(int workTime, int breakTime, ResourceManager resourceManager, ILogger? logger = null)
    {
        _workTime = workTime;
        _breakTime = breakTime;
        _resourceManager = resourceManager;
        _logger = logger;
    }

    public async Task StartSession(CancellationToken cancellationToken = default)
    {
        LogOrWrite(string.Format(_resourceManager.GetString("SessionHeader")!, _workTime, _breakTime));
        LogOrWrite(_resourceManager.GetString("Instructions") + "\n");

        int sessionNumber = 1;

        while (!cancellationToken.IsCancellationRequested)
        {
            LogOrWrite(string.Format(_resourceManager.GetString("SessionWork")!, sessionNumber, _workTime));
            await ExecuteTimer(_workTime * 60, _resourceManager.GetString("WorkPhase")!, cancellationToken);

            if (cancellationToken.IsCancellationRequested) break;

            LogOrWrite("\n" + _resourceManager.GetString("WorkCompleted"));
            if (_logger == null) Console.Beep(); // Only beep in standalone mode

            LogOrWrite(string.Format(_resourceManager.GetString("BreakTime")!, _breakTime));
            await ExecuteTimer(_breakTime * 60, _resourceManager.GetString("BreakPhase")!, cancellationToken);

            if (cancellationToken.IsCancellationRequested) break;

            LogOrWrite("\n" + _resourceManager.GetString("BreakCompleted"));
            if (_logger == null) Console.Beep(); // Only beep in standalone mode

            sessionNumber++;
            
            if (_logger != null)
            {
                // Auto-continue in MCP context
                LogOrWrite("\n" + _resourceManager.GetString("ContinueInstructions"));
                await Task.Delay(2000, cancellationToken);
            }
            else
            {
                // Wait for key press in standalone mode
                LogOrWrite("\n" + _resourceManager.GetString("ContinueInstructions"));
                Console.ReadKey();
                Console.Clear();
            }
        }
    }

    private async Task ExecuteTimer(int totalSeconds, string phase, CancellationToken cancellationToken)
    {
        for (int i = totalSeconds; i > 0 && !cancellationToken.IsCancellationRequested; i--)
        {
            int minutes = i / 60;
            int seconds = i % 60;

            // Effacer la ligne précédente et afficher le nouveau temps
            string remaining = _resourceManager.GetString("Remaining")!;
            string timerDisplay = $"\r{phase}: {minutes:D2}:{seconds:D2} {remaining}{(i > 1 ? "" : "")}";
            
            if (_logger != null)
            {
                // Dans le contexte MCP, utiliser le logger (va vers stderr)
                _logger.LogInformation($"{phase}: {minutes:D2}:{seconds:D2} {remaining}");
            }
            else
            {
                // Dans le contexte standalone, utiliser la console
                Console.Write(timerDisplay);
            }

            await Task.Delay(1000, cancellationToken);
        }

        if (!cancellationToken.IsCancellationRequested)
        {
            string finished = _resourceManager.GetString("Finished")!;
            if (_logger != null)
            {
                _logger.LogInformation($"{phase}: 00:00 - {finished}");
            }
            else
            {
                Console.Write($"\r{phase}: 00:00 - {finished}     ");
            }
        }
    }

    private void LogOrWrite(string message)
    {
        if (_logger != null)
        {
            // Dans le contexte MCP, utiliser le logger (va vers stderr)
            _logger.LogInformation(message);
        }
        else
        {
            // Dans le contexte standalone, utiliser la console
            Console.WriteLine(message);
        }
    }
}
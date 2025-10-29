using System.Resources;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;

namespace PomodoroTimer;

public class PomodoroWindow : Window
{
    private readonly int _workTime;
    private readonly int _breakTime;
    private readonly ResourceManager _resourceManager;
    private readonly CancellationToken _cancellationToken;
    
    private DispatcherTimer _timer;
    private int _remainingSeconds;
    private int _totalSeconds;
    private bool _isWorkPhase = true;
    private int _sessionNumber = 1;

    private TextBlock _phaseLabel;
    private TextBlock _timeLabel;
    private TextBlock _sessionLabel;
    private Button _stopButton;
    private Button _pauseButton;
    private ProgressBar _progressBar;
    private bool _isPaused = false;

    public PomodoroWindow(int workTime, int breakTime, ResourceManager resourceManager, CancellationToken cancellationToken)
    {
        _workTime = workTime;
        _breakTime = breakTime;
        _resourceManager = resourceManager;
        _cancellationToken = cancellationToken;
        
        InitializeComponent();
        StartSession();
    }

    private void InitializeComponent()
    {
        Title = "Pomodoro Timer";
        Width = 450;
        Height = 350;
        WindowStartupLocation = WindowStartupLocation.CenterScreen;
        Topmost = true;
        CanResize = false;
        Background = new SolidColorBrush(Color.FromRgb(240, 248, 255));

        // Main grid
        var mainGrid = new Grid
        {
            Margin = new Thickness(20),
            RowDefinitions = new RowDefinitions("Auto,Auto,Auto,Auto,Auto,Auto"),
            ColumnDefinitions = new ColumnDefinitions("*")
        };

        // Session label
        _sessionLabel = new TextBlock
        {
            FontSize = 14,
            FontWeight = FontWeight.Bold,
            TextAlignment = TextAlignment.Center,
            Foreground = new SolidColorBrush(Color.FromRgb(72, 61, 139)),
            Margin = new Thickness(0, 0, 0, 10)
        };
        Grid.SetRow(_sessionLabel, 0);

        // Phase label
        _phaseLabel = new TextBlock
        {
            FontSize = 24,
            FontWeight = FontWeight.Bold,
            TextAlignment = TextAlignment.Center,
            Margin = new Thickness(0, 0, 0, 10)
        };
        Grid.SetRow(_phaseLabel, 1);

        // Time label
        _timeLabel = new TextBlock
        {
            FontSize = 48,
            FontWeight = FontWeight.Bold,
            TextAlignment = TextAlignment.Center,
            Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 139)),
            Margin = new Thickness(0, 0, 0, 20)
        };
        Grid.SetRow(_timeLabel, 2);

        // Progress bar
        _progressBar = new ProgressBar
        {
            Height = 25,
            Margin = new Thickness(0, 0, 0, 20)
        };
        Grid.SetRow(_progressBar, 3);

        // Buttons panel
        var buttonPanel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Center,
            Spacing = 20
        };

        _pauseButton = new Button
        {
            Content = "Pause",
            Width = 80,
            Height = 35,
            FontSize = 12
        };
        _pauseButton.Click += PauseButton_Click;

        _stopButton = new Button
        {
            Content = "Stop",
            Width = 80,
            Height = 35,
            FontSize = 12
        };
        _stopButton.Click += (s, e) => Close();

        buttonPanel.Children.Add(_pauseButton);
        buttonPanel.Children.Add(_stopButton);

        Grid.SetRow(buttonPanel, 4);

        // Timer
        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(1)
        };
        _timer.Tick += Timer_Tick;

        mainGrid.Children.Add(_sessionLabel);
        mainGrid.Children.Add(_phaseLabel);
        mainGrid.Children.Add(_timeLabel);
        mainGrid.Children.Add(_progressBar);
        mainGrid.Children.Add(buttonPanel);

        Content = mainGrid;

        // Handle cancellation token
        _cancellationToken.Register(() =>
        {
            Dispatcher.UIThread.InvokeAsync(() => Close());
        });
    }

    private void PauseButton_Click(object sender, RoutedEventArgs e)
    {
        if (_isPaused)
        {
            _timer.Start();
            _pauseButton.Content = "Pause";
            _isPaused = false;
        }
        else
        {
            _timer.Stop();
            _pauseButton.Content = "Resume";
            _isPaused = true;
        }
    }

    private void StartSession()
    {
        _isWorkPhase = true;
        _sessionNumber = 1;
        StartPhase();
    }

    private void StartPhase()
    {
        if (_isWorkPhase)
        {
            _remainingSeconds = _workTime * 60;
            _totalSeconds = _workTime * 60;
            _phaseLabel.Text = _resourceManager.GetString("WorkPhase") ?? "Work Phase";
            _phaseLabel.Foreground = new SolidColorBrush(Color.FromRgb(0, 100, 0));
            _sessionLabel.Text = string.Format(_resourceManager.GetString("SessionWork") ?? "Session {0}: Work for {1} minutes", _sessionNumber, _workTime);
            Background = new SolidColorBrush(Color.FromRgb(240, 255, 240)); // Light green
        }
        else
        {
            _remainingSeconds = _breakTime * 60;
            _totalSeconds = _breakTime * 60;
            _phaseLabel.Text = _resourceManager.GetString("BreakPhase") ?? "Break Phase";
            _phaseLabel.Foreground = new SolidColorBrush(Color.FromRgb(255, 140, 0));
            _sessionLabel.Text = string.Format(_resourceManager.GetString("BreakTime") ?? "Break time: {0} minutes", _breakTime);
            Background = new SolidColorBrush(Color.FromRgb(255, 248, 240)); // Light orange
        }

        _progressBar.Maximum = _totalSeconds;
        _progressBar.Value = _remainingSeconds;
        UpdateTimeDisplay();
        
        _isPaused = false;
        _pauseButton.Content = "Pause";
        _timer.Start();
    }

    private void Timer_Tick(object sender, EventArgs e)
    {
        _remainingSeconds--;
        _progressBar.Value = Math.Max(0, _remainingSeconds);
        UpdateTimeDisplay();

        if (_remainingSeconds <= 0)
        {
            _timer.Stop();
            PhaseComplete();
        }
    }

    private void UpdateTimeDisplay()
    {
        int minutes = Math.Max(0, _remainingSeconds) / 60;
        int seconds = Math.Max(0, _remainingSeconds) % 60;
        _timeLabel.Text = $"{minutes:D2}:{seconds:D2}";
        
        // Change color as time runs out
        if (_remainingSeconds <= 60) // Last minute
        {
            _timeLabel.Foreground = new SolidColorBrush(Colors.Red);
        }
        else if (_remainingSeconds <= 300) // Last 5 minutes
        {
            _timeLabel.Foreground = new SolidColorBrush(Colors.Orange);
        }
        else
        {
            _timeLabel.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 139));
        }
    }

    private async void PhaseComplete()
    {
        // Flash the window
        FlashWindow();

        string message;
        string title = "Pomodoro Timer";
        
        if (_isWorkPhase)
        {
            message = _resourceManager.GetString("WorkCompleted") ?? "Work session completed! Time for a break.";
            _isWorkPhase = false;
        }
        else
        {
            message = _resourceManager.GetString("BreakCompleted") ?? "Break completed! Ready for the next work session.";
            _isWorkPhase = true;
            _sessionNumber++;
        }

        // Show notification dialog
        var result = await ShowMessageBox(message + "\n\nContinue to next phase?", title);

        if (result)
        {
            StartPhase();
        }
        else
        {
            Close();
        }
    }

    private async Task<bool> ShowMessageBox(string message, string title)
    {
        var messageBox = new Window
        {
            Title = title,
            Width = 400,
            Height = 200,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            CanResize = false
        };

        var grid = new Grid
        {
            Margin = new Thickness(20),
            RowDefinitions = new RowDefinitions("*,Auto"),
            ColumnDefinitions = new ColumnDefinitions("*")
        };

        var messageText = new TextBlock
        {
            Text = message,
            TextWrapping = TextWrapping.Wrap,
            TextAlignment = TextAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };
        Grid.SetRow(messageText, 0);

        var buttonPanel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Center,
            Spacing = 10,
            Margin = new Thickness(0, 20, 0, 0)
        };

        bool result = false;

        var yesButton = new Button
        {
            Content = "Yes",
            Width = 60,
            Height = 30
        };
        yesButton.Click += (s, e) => { result = true; messageBox.Close(); };

        var noButton = new Button
        {
            Content = "No",
            Width = 60,
            Height = 30
        };
        noButton.Click += (s, e) => { result = false; messageBox.Close(); };

        buttonPanel.Children.Add(yesButton);
        buttonPanel.Children.Add(noButton);

        Grid.SetRow(buttonPanel, 1);

        grid.Children.Add(messageText);
        grid.Children.Add(buttonPanel);
        messageBox.Content = grid;

        await messageBox.ShowDialog(this);
        return result;
    }

    private void FlashWindow()
    {
        WindowState = WindowState.Normal;
        Activate();
        Topmost = false;
        Topmost = true;
    }

    protected override void OnClosing(WindowClosingEventArgs e)
    {
        _timer?.Stop();
        base.OnClosing(e);
    }
}
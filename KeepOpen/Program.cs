// destroy our console window
using KeepOpen;
using System.Runtime.InteropServices;
using System.Diagnostics;

[DllImport("kernel32.dll")]
static extern IntPtr GetConsoleWindow();
[DllImport("user32.dll")]
static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

bool showConsole = false;

// Load configuration
var config = KeepOpen.Config.Load();
Console.WriteLine($"Loaded {config.Programs.Count} programs from configuration.");

foreach (var arg in args)
{
    if (arg == "--help" || arg == "-h")
    {
        Console.WriteLine("KeepOpen - A simple utility to keep a process or session active.");
        Console.WriteLine();
        Console.WriteLine("Usage:");
        Console.WriteLine("  KeepOpen [options]");
        Console.WriteLine();
        Console.WriteLine("Options:");
        Console.WriteLine("  -c, --console    Keep the console window visible (default is hidden).");
        Console.WriteLine("  -h, --help       Show this help message.");
        return;
    }
    
    if (arg == "--console" || arg == "-c")
    {
        showConsole = true;
    }
}

if (!showConsole)
{
    nint hWnd = GetConsoleWindow();
    ShowWindow(hWnd, 0); // 0 = SW_HIDE
}

while (true)
{
    foreach (AppConfig app in config.Programs)
    {
        string? targetName = app.ProcessName?.Trim();
        if (string.IsNullOrEmpty(targetName))
        {
            Console.WriteLine("ProcessName for app is null, please update your config!");
            continue;
        }

        if (!Processes.IsProcessRunning(targetName))
        {
            Console.WriteLine($"Target '{targetName}' not alive, prompting to start");
            
            if (Processes.PromptToLaunch(targetName))
            {
                Console.WriteLine($"Launching '{targetName}'...");
#pragma warning disable CA1416 // Validate platform compatibility
                Processes.LaunchDetached(app.Path!, app.Arguments ?? string.Empty);
#pragma warning restore CA1416 // Validate platform compatibility
            }
            else
            {
                Console.WriteLine($"User declined to launch '{targetName}'.");
            }
        }
    }

    Thread.Sleep(config.IterationTimeSeconds * 1000);
}
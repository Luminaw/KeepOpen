# KeepOpen

KeepOpen is a lightweight Windows utility designed to monitor specific processes and ensure they remain active. If a monitored process is found to be no longer running, KeepOpen will prompt the user with a custom message box to relaunch it.

## Features

- **Process Monitoring**: Regularly checks if configured processes are currently running.
- **Prompted Recovery**: Shows a non-intrusive dialog asking the user if they'd like to restart a closed application.
- **Configurable**: Easily manage the list of monitored programs and the check frequency via a JSON file.
- **Stealth Mode**: By default, the console window is hidden to keep your workspace clean.

## Usage

Run the executable to start monitoring.

### Command Line Options

- `-c, --console`: Keep the console window visible (useful for debugging).
- `-h, --help`: Show the help message.

## Configuration

Settings are stored in `appsettings.json` in the application directory.

```json
{
  "IterationTimeSeconds": 120,
  "Programs": [
    {
      "Path": "C:\\Windows\\System32\\notepad.exe",
      "ProcessName": "notepad",
      "Arguments": ""
    }
  ]
}
```

- **IterationTimeSeconds**: How often (in seconds) to check the processes.
- **Programs**: A list of objects containing:
  - `Path`: Full path to the executable.
  - `ProcessName`: The name of the process as it appears in the Task Manager (without `.exe`).
  - `Arguments`: (Optional) Command line arguments to pass to the application on launch.

## Requirements

- **Operating System**: Windows
- **Runtime**: .NET 10.0 Desktop Runtime or higher

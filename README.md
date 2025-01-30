# Pty4Net
Cross-platform pseudo terminal (PTY) implementation in .NET.

# Requirements

- .NET 8

# Features

- .NET 8 cross-platform library that implements a pseudo-terminal.
- Works on Windows, Linux and MacOS.
- On Windows, the library provides two implementations depending on the version:
  - For Windows 10 version 1803 or greater, the library uses [ConPty](https://devblogs.microsoft.com/commandline/windows-command-line-introducing-the-windows-pseudo-console-conpty/).
  - For earlier versions, [WinPty](https://github.com/rprichard/winpty) is used.

# Usage

In its simplest form, to create a pseudo-terminal you just need a single line of code:

```csharp
IPseudoTerminal pty = PseudoTerminalProvider.CreatePseudoTerminal();
```

This will create an instance of a terminal with the default options for the current platform, but it's also possible to customize the options with a few more lines of code:

```csharp
PseudoTerminalOptions options = PseudoTerminalOptions.CreateDefault();
options.Environment["MY_ENV_VAR"] = "SomeValue";

IPseudoTerminal pty = PseudoTerminalProvider.CreatePseudoTerminal(options);
```

Then, you can read and write data from the terminal with `ReadAsync()` and `WriteAsync()` methods, respectively. Here's a simple example that runs the commands `echo hello world` and `exit` and saves all the terminal output to a file:

```csharp
bool exited = false;
// output FileStream is used to save the terminal output.
using FileStream output = File.OpenWrite("output.txt");
using IPseudoTerminal terminal = PseudoTerminalProvider.CreatePseudoTerminal();
terminal.ProcessExited += (sender, e) => exited = true;
CancellationTokenSource cancellationSource = new CancellationTokenSource();
Task.Run(async () =>
{
    byte[] data = new byte[4096];
    while (!cancellationSource.IsCancellationRequested)
    {
        // read terminal output.
        var bytesReceived = await terminal.ReadAsync(data, 0, data.Length);
        if (bytesReceived > 0)
        {
            // save terminal output data to file.
            output.Write(data, 0, bytesReceived);
        }
        await Task.Delay(5);
    }
}, cancellationSource.Token);

// send 'echo' command.
byte[] enter = Encoding.Default.GetBytes(Environment.NewLine);
byte[] echoCmd = Encoding.Default.GetBytes("echo hello world");
terminal.WriteAsync(echoCmd, 0, echoCmd.Length).Wait();
terminal.WriteAsync(enter, 0, enter.Length).Wait();

// send 'exit' command.
byte[] exitCmd = Encoding.Default.GetBytes("exit");
terminal.WriteAsync(exitCmd, 0, exitCmd.Length).Wait();
terminal.WriteAsync(enter, 0, enter.Length).Wait();

// wait for terminal to exit.
Task.Run(async () =>
{
    while (!exited)
    {
        await Task.Delay(5);
    }
    output.Flush();
    output.Close();
    cancellationSource.Cancel();
}).Wait(5000);
```

# Building

To build this project you need .NET 8 SDK. Assuming all the tools can be found on the `PATH`, simply go to the project directory and run the following commands:

```console
$ dotnet restore
$ dotnet build
```
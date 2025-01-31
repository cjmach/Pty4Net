using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Pty4Net.Tests;

public class PseudoTerminalTests
{
    private const string OutputFileName = "pty-output.txt";
    private readonly string OutputFile = Path.Combine(Path.GetDirectoryName(typeof(PseudoTerminalTests).Assembly.Location), OutputFileName);

    [OneTimeSetUp]
    public void Init()
    {
        if (File.Exists(OutputFile))
        {
            File.Delete(OutputFile);
        }
    }

    [Test]
    public void TestOptions() {
        PseudoTerminalOptions options = PseudoTerminalOptions.CreateDefault();

        string expectedCmd;
        string[] expectedArgs;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
            expectedCmd = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "cmd.exe");
            expectedArgs = [];
        } else {
            expectedCmd = "/bin/bash";
            expectedArgs = ["--login"];
        }
        Assert.That(options.Command, Is.EqualTo(expectedCmd));
        Assert.That(options.Arguments, Is.EqualTo(expectedArgs));
    }

    [Test]
    public void TestTerminal()
    {
        bool exited = false;
        bool ok = false;

        // generate a random message to be echoed to the terminal.
        int random = RandomNumberGenerator.GetInt32(int.MaxValue);
        string outputToMatch = $"Testing {random}";

        PseudoTerminalOptions options = PseudoTerminalOptions.CreateDefault();
        options.Columns = 80;
        options.Rows = 40;

        // output FileStream is used to save the terminal output.
        using (FileStream output = File.OpenWrite(OutputFile))
        {
            using IPseudoTerminal terminal = PseudoTerminalProvider.CreatePseudoTerminal(options);
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

            Thread.Sleep(500);

            // send 'echo' command.
            byte[] enter = Encoding.Default.GetBytes(Environment.NewLine);
            byte[] echoCmd = Encoding.Default.GetBytes($"echo {outputToMatch}");
            terminal.WriteAsync(echoCmd, 0, echoCmd.Length).Wait();
            terminal.WriteAsync(enter, 0, enter.Length).Wait();

            Thread.Sleep(500);

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

            
            string line;
            // read output, line by line.
            using StreamReader reader = File.OpenText(OutputFile);
            while ((line = reader.ReadLine()) != null)
            {
                if (outputToMatch.StartsWith(line))
                {
                    ok = true;
                    break;
                }
            }
        }

        Assert.IsTrue(ok, $"Line '{outputToMatch}' not found in the terminal output. Current content: {Environment.NewLine}{File.ReadAllText(OutputFile)}");
    }
}
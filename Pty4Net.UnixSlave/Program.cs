using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Pty4Net.UnixSlave;

using static NativeMethods;

public class Program {
    static int Main(string[] args) {
        string shell = null;
        string cwd = null;

        List<string> shellArgs = new List<string>();
        try
        {
            for (int i = 0; i < args.Length; i++)
            {
                string arg = args[i];
                switch (arg)
                {
                    case "-d":
                    case "--directory":
                        cwd = args[++i];
                        break;

                    case "-s":
                    case "--shell":
                        shell = args[++i];
                        break;

                    default:
                        shellArgs.Add(arg);
                        break;
                }
            }
        } catch (IndexOutOfRangeException) {
            Console.Error.WriteLine("[ERROR] Invalid command line options.");
            Console.Error.WriteLine("Usage: Pty4Net.UnixSlave --directory PATH --shell SHELL [SHELL args...]");
            return 1;
        }
        if (string.IsNullOrEmpty(shell)) {
            Console.Error.WriteLine("[ERROR] Missing required option -s/--shell.");
            return 1;
        }
        if (string.IsNullOrEmpty(cwd)) {
            Console.Error.WriteLine("[ERROR] Missing required option -d/--directory.");
            return 1;
        }

        if (SetSid() < 0) {
            throw new InvalidOperationException("Failed to call setsid(2).");
        }
        if (Ioctl(0, TIOCSCTTY, IntPtr.Zero) < 0) {
            throw new InvalidOperationException("Failed to call ioctl(2).");
        }
        Chdir(args[1]);

        List<string> envVars = new List<string>();
        IDictionary env = Environment.GetEnvironmentVariables();

        foreach (string variable in env.Keys)
        {
            envVars.Add($"{variable}={env[variable]}");
        }
        envVars.Add(null);

        shellArgs.Add(null);

        Execve(shell, shellArgs.ToArray(), envVars.ToArray());

        Console.Error.WriteLine($"[ERROR] Failed to execute {shell} via execve(2). Errno: {Marshal.GetLastSystemError()}.");
        return 1;
    }
}

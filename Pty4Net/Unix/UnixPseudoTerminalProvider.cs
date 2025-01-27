using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace Pty4Net.Unix
{
    public class UnixPseudoTerminalProvider : IPseudoTerminalProvider
    {
        public IPseudoTerminal Create(int columns, int rows, string initialDirectory, string environment, string command, params string[] arguments)
        {
            var fdm = NativeMethods.open("/dev/ptmx", NativeMethods.O_RDWR | NativeMethods.O_NOCTTY);

            var res = NativeMethods.grantpt(fdm);
            res = NativeMethods.unlockpt(fdm);

            var namePtr = NativeMethods.ptsname(fdm);
            var name = Marshal.PtrToStringAnsi(namePtr);
            var fds = NativeMethods.open(name, (int)NativeMethods.O_RDWR);

            var fileActions = Marshal.AllocHGlobal(1024);
            NativeMethods.posix_spawn_file_actions_init(fileActions);
            res = NativeMethods.posix_spawn_file_actions_adddup2(fileActions, (int)fds, 0);
            res = NativeMethods.posix_spawn_file_actions_adddup2(fileActions, (int)fds, 1);
            res = NativeMethods.posix_spawn_file_actions_adddup2(fileActions, (int)fds, 2);
            res = NativeMethods.posix_spawn_file_actions_addclose(fileActions, (int)fdm);
            res = NativeMethods.posix_spawn_file_actions_addclose(fileActions, (int)fds);


            var attributes = Marshal.AllocHGlobal(1024);
            res = NativeMethods.posix_spawnattr_init(attributes);

            var envVars = new List<string>();
            var env = Environment.GetEnvironmentVariables();

            foreach(var variable in env.Keys)
            {
                if(variable.ToString() != "TERM")
                {
                    envVars.Add($"{variable}={env[variable]}");
                }
            }

            envVars.Add("TERM=xterm-256color");
            envVars.Add(null);

            string path = typeof(UnixSlave.Program).Assembly.Location;
            var argsArray = new List<string> { "dotnet", path, "-d", initialDirectory, "-s", command };
            argsArray.AddRange(arguments);
            argsArray.Add(null);

            res = NativeMethods.posix_spawnp(out var pid, "dotnet", fileActions, attributes, argsArray.ToArray(), envVars.ToArray());

            var stdin = NativeMethods.dup(fdm);
            var process = Process.GetProcessById((int)pid);
            return new UnixPseudoTerminal(process, fds, stdin, new FileStream(new SafeFileHandle(new IntPtr(stdin), true), FileAccess.Write), new FileStream(new SafeFileHandle(new IntPtr(fdm), true), FileAccess.Read));
        }
    }
}

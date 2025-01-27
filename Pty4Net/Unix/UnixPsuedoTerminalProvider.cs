﻿using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace AvalonStudio.Terminals.Unix
{
    public class UnixPsuedoTerminalProvider : IPsuedoTerminalProvider
    {
        public IPsuedoTerminal Create(int columns, int rows, string initialDirectory, string environment, string command, params string[] arguments)
        {
            var fdm = Native.open("/dev/ptmx", Native.O_RDWR | Native.O_NOCTTY);

            var res = Native.grantpt(fdm);
            res = Native.unlockpt(fdm);

            var namePtr = Native.ptsname(fdm);
            var name = Marshal.PtrToStringAnsi(namePtr);
            var fds = Native.open(name, (int)Native.O_RDWR);

            var fileActions = Marshal.AllocHGlobal(1024);
            Native.posix_spawn_file_actions_init(fileActions);
            res = Native.posix_spawn_file_actions_adddup2(fileActions, (int)fds, 0);
            res = Native.posix_spawn_file_actions_adddup2(fileActions, (int)fds, 1);
            res = Native.posix_spawn_file_actions_adddup2(fileActions, (int)fds, 2);
            res = Native.posix_spawn_file_actions_addclose(fileActions, (int)fdm);
            res = Native.posix_spawn_file_actions_addclose(fileActions, (int)fds);


            var attributes = Marshal.AllocHGlobal(1024);
            res = Native.posix_spawnattr_init(attributes);

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

            var path = System.Reflection.Assembly.GetEntryAssembly().Location;
            var argsArray = new List<string> { "dotnet", path, "--trampoline", initialDirectory, command };
            argsArray.AddRange(arguments);
            argsArray.Add(null);

            res = Native.posix_spawnp(out var pid, "dotnet", fileActions, attributes, argsArray.ToArray(), envVars.ToArray());

            var stdin = Native.dup(fdm);
            var process = Process.GetProcessById((int)pid);
            return new UnixPsuedoTerminal(process, fds, stdin, new FileStream(new SafeFileHandle(new IntPtr(stdin), true), FileAccess.Write), new FileStream(new SafeFileHandle(new IntPtr(fdm), true), FileAccess.Read));
        }
    }
}

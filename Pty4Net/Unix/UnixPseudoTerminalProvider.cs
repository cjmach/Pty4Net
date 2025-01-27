using Microsoft.Win32.SafeHandles;
using System;
using System.Collections;
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
            IntPtr fileActions = IntPtr.Zero;
            IntPtr attributes = IntPtr.Zero;

            int master = NativeMethods.open("/dev/ptmx", NativeMethods.O_RDWR | NativeMethods.O_NOCTTY);

            int res = NativeMethods.grantpt(master);
            res = NativeMethods.unlockpt(master);

            IntPtr namePtr = NativeMethods.ptsname(master);
            string name = Marshal.PtrToStringAnsi(namePtr);
            int slave = NativeMethods.open(name, NativeMethods.O_RDWR);

            try
            {
                fileActions = Marshal.AllocHGlobal(1024);
                NativeMethods.posix_spawn_file_actions_init(fileActions);
                res = NativeMethods.posix_spawn_file_actions_adddup2(fileActions, slave, 0);
                res = NativeMethods.posix_spawn_file_actions_adddup2(fileActions, slave, 1);
                res = NativeMethods.posix_spawn_file_actions_adddup2(fileActions, slave, 2);
                res = NativeMethods.posix_spawn_file_actions_addclose(fileActions, master);
                res = NativeMethods.posix_spawn_file_actions_addclose(fileActions, slave);


                attributes = Marshal.AllocHGlobal(1024);
                res = NativeMethods.posix_spawnattr_init(attributes);

                List<string> envVars = new List<string>();
                IDictionary env = Environment.GetEnvironmentVariables();

                foreach (string variable in env.Keys)
                {
                    if (variable != "TERM")
                    {
                        envVars.Add($"{variable}={env[variable]}");
                    }
                }

                envVars.Add("TERM=xterm-256color");
                envVars.Add(null);

                string path = typeof(UnixSlave.Program).Assembly.Location;
                List<string> argsArray = new List<string> { "dotnet", path, "-d", initialDirectory, "-s", command };
                argsArray.AddRange(arguments);
                argsArray.Add(null);

                res = NativeMethods.posix_spawnp(out IntPtr pid, "dotnet", fileActions, attributes, argsArray.ToArray(), envVars.ToArray());

                int stdin = NativeMethods.dup(master);
                Process process = Process.GetProcessById(pid.ToInt32());
                return new UnixPseudoTerminal(process, 
                                              slave, 
                                              stdin, 
                                              new FileStream(new SafeFileHandle(new IntPtr(stdin), true), FileAccess.Write), 
                                              new FileStream(new SafeFileHandle(new IntPtr(master), true), FileAccess.Read));
            }
            finally
            {
                if (fileActions != IntPtr.Zero)
                {
                    NativeMethods.posix_spawn_file_actions_destroy(fileActions);
                    Marshal.FreeHGlobal(fileActions);
                }
                if (attributes != IntPtr.Zero)
                {
                    NativeMethods.posix_spawnattr_destroy(attributes);
                    Marshal.FreeHGlobal(attributes);
                }
            }
        }
    }
}

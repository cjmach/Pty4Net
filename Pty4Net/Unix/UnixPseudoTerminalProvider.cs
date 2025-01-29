using Microsoft.Win32.SafeHandles;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Pty4Net.Unix
{
    /// <summary>
    /// 
    /// </summary>
    internal class UnixPseudoTerminalProvider : IPseudoTerminalProvider
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public IPseudoTerminal CreatePseudoTerminal(PseudoTerminalOptions options)
        {
            IntPtr fileActions = IntPtr.Zero;
            IntPtr attributes = IntPtr.Zero;

            int master = NativeMethods.open("/dev/ptmx", NativeMethods.O_RDWR | NativeMethods.O_NOCTTY);
            if (master < 0)
            {
                throw new InvalidOperationException("Call to open(2) failed. Error: " + Marshal.GetLastSystemError());
            }

            int res = NativeMethods.grantpt(master);
            if (res < 0)
            {
                throw new InvalidOperationException("Call to grantpt(3) failed. Error: " + Marshal.GetLastSystemError());
            }
            res = NativeMethods.unlockpt(master);
            if (res < 0)
            {
                throw new InvalidOperationException("Call to unlockpt(3) failed. Error: " + Marshal.GetLastSystemError());
            }

            IntPtr namePtr = NativeMethods.ptsname(master);
            if (namePtr == IntPtr.Zero)
            {
                throw new InvalidOperationException("Call to ptsname(3) failed. Error: " + Marshal.GetLastSystemError());
            }
            string name = Marshal.PtrToStringAnsi(namePtr);
            int slave = NativeMethods.open(name, NativeMethods.O_RDWR);
            if (slave < 0)
            {
                throw new InvalidOperationException("Call to open(2) failed. Error: " + Marshal.GetLastSystemError());
            }

            try
            {
                fileActions = Marshal.AllocHGlobal(1024);
                res = NativeMethods.posix_spawn_file_actions_init(fileActions);
                if (res != 0)
                {
                    throw new InvalidOperationException("Call to posix_spawn_file_actions_list(3) failed. Error: " + res);
                }
                res = NativeMethods.posix_spawn_file_actions_adddup2(fileActions, slave, 0);
                if (res != 0)
                {
                    throw new InvalidOperationException("Call to posix_spawn_file_actions_adddup2(3) failed. Error: " + res);
                }
                res = NativeMethods.posix_spawn_file_actions_adddup2(fileActions, slave, 1);
                if (res != 0)
                {
                    throw new InvalidOperationException("Call to posix_spawn_file_actions_adddup2(3) failed. Error: " + res);
                }
                res = NativeMethods.posix_spawn_file_actions_adddup2(fileActions, slave, 2);
                if (res != 0)
                {
                    throw new InvalidOperationException("Call to posix_spawn_file_actions_adddup2(3) failed. Error: " + res);
                }
                res = NativeMethods.posix_spawn_file_actions_addclose(fileActions, master);
                if (res != 0)
                {
                    throw new InvalidOperationException("Call to posix_spawn_file_actions_addclose(3) failed. Error: " + res);
                }
                res = NativeMethods.posix_spawn_file_actions_addclose(fileActions, slave);
                if (res != 0)
                {
                    throw new InvalidOperationException("Call to posix_spawn_file_actions_addclose(3) failed. Error: " + res);
                }


                attributes = Marshal.AllocHGlobal(1024);
                res = NativeMethods.posix_spawnattr_init(attributes);
                if (res != 0)
                {
                    throw new InvalidOperationException("Call to posix_spawnattr_init(3) failed. Error: " + res);
                }

                string[] envVars = options.EnvironmentList;

                string path = typeof(UnixSlave.Program).Assembly.Location;
                List<string> argsArray = new List<string> { "dotnet", path, "-d", options.InitialDirectory, "-s", options.Command };
                argsArray.AddRange(options.Arguments);
                argsArray.Add(null);

                res = NativeMethods.posix_spawnp(out IntPtr pid, "dotnet", fileActions, attributes, argsArray.ToArray(), envVars);
                if (res != 0)
                {
                    throw new InvalidOperationException("Call to posix_spawnp(3) failed. Error: " + res);
                }

                Process process = Process.GetProcessById(pid.ToInt32());
                Waitpid(process.Id);
                return new UnixPseudoTerminal(process,
                                              master,
                                              new FileStream(new SafeFileHandle(new IntPtr(master), true), FileAccess.Write),
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

        private void Waitpid(int pid)
        {
            Task.Run(() =>
            {
                NativeMethods.waitpid(pid, IntPtr.Zero, 0);
            });
        }
    }
}

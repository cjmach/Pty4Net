using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Runtime.InteropServices;
using static winpty.WinPty;

namespace Pty4Net.Win32
{
    /// <summary>
    /// 
    /// </summary>
    internal class WinPtyTerminalProvider : IPseudoTerminalProvider
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public IPseudoTerminal CreatePseudoTerminal(PseudoTerminalOptions options)
        {
            var cfg = winpty_config_new(WINPTY_FLAG_COLOR_ESCAPES, out IntPtr err);
            winpty_config_set_initial_size(cfg, options.Columns, options.Rows);

            var handle = winpty_open(cfg, out err);

            if (err != IntPtr.Zero)
            {
                Console.WriteLine(winpty_error_code(err));
                return null;
            }

            string exe = options.Command;
            string args = string.Join(" ", options.Arguments);
            string cwd = options.InitialDirectory;

            var spawnCfg = winpty_spawn_config_new(WINPTY_SPAWN_FLAG_AUTO_SHUTDOWN, exe, args, cwd, options.Environment, out err);
            if (err != IntPtr.Zero)
            {
                Console.WriteLine(winpty_error_code(err));
                return null;
            }

            var stdin = CreatePipe(winpty_conin_name(handle), PipeDirection.Out);
            var stdout = CreatePipe(winpty_conout_name(handle), PipeDirection.In);

            if (!winpty_spawn(handle, spawnCfg, out IntPtr process, out IntPtr thread, out int procError, out err))
            {
                Console.WriteLine(winpty_error_code(err));
                return null;
            }

            var id = GetProcessId(process);

            var terminalProcess = Process.GetProcessById(id);

            return new WinPtyTerminal(terminalProcess, handle, cfg, spawnCfg, err, stdin, stdout);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        [DllImport("kernel32", SetLastError = true)]
        static extern int GetProcessId(IntPtr handle);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pipeName"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        private Stream CreatePipe(string pipeName, PipeDirection direction)
        {
            string serverName = ".";

            if (pipeName.StartsWith("\\"))
            {
                int slash3 = pipeName.IndexOf('\\', 2);

                if (slash3 != -1)
                {
                    serverName = pipeName.Substring(2, slash3 - 2);
                }

                int slash4 = pipeName.IndexOf('\\', slash3 + 1);

                if (slash4 != -1)
                {
                    pipeName = pipeName.Substring(slash4 + 1);
                }
            }

            var pipe = new NamedPipeClientStream(serverName, pipeName, direction);

            pipe.Connect();

            return pipe;
        }
    }
}

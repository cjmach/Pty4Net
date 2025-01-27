using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Pty4Net.Unix
{
    public class UnixPseudoTerminal : IPseudoTerminal
    {
        private int _handle;
        private int _cfg;
        private Stream _stdin = null;
        private Stream _stdout = null;
        private Process _process;
        private bool _isDisposed = false;

        public UnixPseudoTerminal(Process process, int handle, int cfg, Stream stdin, Stream stdout)
        {
            _process = process;

            _handle = handle;
            _stdin = stdin;
            _stdout = stdout;

            _cfg = cfg;
        }

        public static void Trampoline(string[] args)
        {
            if (args.Length > 2 && args[0] == "--trampoline")
            {
                NativeMethods.setsid();
                NativeMethods.ioctl(0, NativeMethods.TIOCSCTTY, IntPtr.Zero);
                NativeMethods.chdir(args[1]);

                var envVars = new List<string>();
                var env = Environment.GetEnvironmentVariables();

                foreach (var variable in env.Keys)
                {
                    if (variable.ToString() != "TERM")
                    {
                        envVars.Add($"{variable}={env[variable]}");
                    }
                }

                envVars.Add("TERM=xterm-256color");
                envVars.Add(null);

                var argsArray = args.Skip(3).ToList();
                argsArray.Add(null);

                NativeMethods.execve(args[2], argsArray.ToArray(), envVars.ToArray());
            }
            else
            {
                return;
            }
        }

        public void Dispose()
        {
            if (!_isDisposed)
            {
                _isDisposed = true;
                _stdin?.Dispose();
                _stdout?.Dispose();

                // TODO close file descriptors and terminate processes?
            }
        }

        public async Task<int> ReadAsync(byte[] buffer, int offset, int count)
        {
            return await _stdout.ReadAsync(buffer, offset, count);
        }

        public async Task WriteAsync(byte[] buffer, int offset, int count)
        {
            if (buffer.Length == 1 && buffer[0] == 10)
            {
                buffer[0] = 13;
            }

            await Task.Run(() =>
            {
                var buf = Marshal.AllocHGlobal(count);
                Marshal.Copy(buffer, offset, buf, count);
                NativeMethods.write(_cfg, buf, count);

                Marshal.FreeHGlobal(buf);
            });
        }

        public void SetSize(int columns, int rows)
        {
            NativeMethods.WinSize size = new NativeMethods.WinSize();
            int ret;
            size.ws_row = (ushort)(rows > 0 ? rows : 24);
            size.ws_col = (ushort)(columns > 0 ? columns : 80);

            var ptr = NativeMethods.StructToPtr(size);

            ret = NativeMethods.ioctl(_cfg, NativeMethods.TIOCSWINSZ, ptr);

            Marshal.FreeHGlobal(ptr);

            var error = Marshal.GetLastWin32Error();
        }

        public Process Process => _process;
    }
}

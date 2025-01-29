using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Pty4Net.Unix
{
    internal class UnixPseudoTerminal : BasePseudoTerminal
    {
        private int _cfg;
        private Stream _stdin = null;
        private Stream _stdout = null;
        private bool _isDisposed = false;

        internal UnixPseudoTerminal(Process process, int cfg, Stream stdin, Stream stdout) : base(process)
        {
            _stdin = stdin;
            _stdout = stdout;

            _cfg = cfg;
        }

        public override void Dispose()
        {
            if (!_isDisposed)
            {
                _isDisposed = true;
                _stdin?.Dispose();
                _stdout?.Dispose();

                // TODO close file descriptors and terminate processes?
            }
        }

        public override async Task<int> ReadAsync(byte[] buffer, int offset, int count)
        {
            return await _stdout.ReadAsync(buffer, offset, count);
        }

        public override async Task WriteAsync(byte[] buffer, int offset, int count)
        {
            await Task.Run(() =>
            {
                NativeMethods.write(_cfg, buffer, count);
            });
        }

        public override void SetSize(int columns, int rows)
        {
            if (columns > 0 && rows > 0)
            {
                NativeMethods.WinSize size = new NativeMethods.WinSize(columns, rows);
                int ret = NativeMethods.ioctl(_cfg, NativeMethods.TIOCSWINSZ, ref size);
                if (ret < 0) 
                {
                    throw new InvalidOperationException("Call to ioctl(2) failed. Error: " + Marshal.GetLastWin32Error());
                }
            }
        }
    }
}

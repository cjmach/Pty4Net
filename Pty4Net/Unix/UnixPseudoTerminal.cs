using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Pty4Net.Unix
{
    internal class UnixPseudoTerminal : BasePseudoTerminal
    {
        private int _handle;
        private int _cfg;
        private Stream _stdin = null;
        private Stream _stdout = null;
        private bool _isDisposed = false;

        internal UnixPseudoTerminal(Process process, int handle, int cfg, Stream stdin, Stream stdout) : base(process)
        {
            _handle = handle;
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
                IntPtr buf = Marshal.AllocHGlobal(count);
                try
                {
                    Marshal.Copy(buffer, offset, buf, count);
                    NativeMethods.write(_cfg, buf, count);
                }
                finally
                {
                    Marshal.FreeHGlobal(buf);
                }
            });
        }

        public override void SetSize(int columns, int rows)
        {
            NativeMethods.WinSize size = new NativeMethods.WinSize();
            int ret;
            size.ws_row = (ushort)(rows > 0 ? rows : 24);
            size.ws_col = (ushort)(columns > 0 ? columns : 80);

            IntPtr ptr = NativeMethods.StructToPtr(size);
            try
            {
                ret = NativeMethods.ioctl(_cfg, NativeMethods.TIOCSWINSZ, ptr);
                var error = Marshal.GetLastWin32Error();
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
        }
    }
}

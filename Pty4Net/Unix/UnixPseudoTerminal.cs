using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Pty4Net.Unix
{
    /// <summary>
    /// Pseudo-terminal implementation for Unix systems.
    /// </summary>
    internal class UnixPseudoTerminal : BasePseudoTerminal
    {
        /// <summary>
        /// 
        /// </summary>
        private readonly int cfg;
        /// <summary>
        /// 
        /// </summary>
        private readonly Stream stdin;
        /// <summary>
        /// 
        /// </summary>
        private readonly Stream stdout;
        /// <summary>
        /// 
        /// </summary>
        private bool isDisposed;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="process">The terminal process.</param>
        /// <param name="cfg">The file descriptor of the terminal.</param>
        /// <param name="stdin">Stream which will be used to send data to terminal's stdin.</param>
        /// <param name="stdout">Stream which will be used to read data from terminal's stdout.</param>
        internal UnixPseudoTerminal(Process process, int cfg, Stream stdin, Stream stdout) : base(process)
        {
            this.stdin = stdin;
            this.stdout = stdout;

            this.cfg = cfg;
        }

        /// <summary>
        /// Disposes the streams.
        /// </summary>
        public override void Dispose()
        {
            if (!isDisposed)
            {
                isDisposed = true;
                stdin?.Dispose();
                stdout?.Dispose();

                // TODO close file descriptors and terminate processes?
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public override async Task<int> ReadAsync(byte[] buffer, int offset, int count)
        {
            return await stdout.ReadAsync(buffer, offset, count);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public override async Task WriteAsync(byte[] buffer, int offset, int count)
        {
            await stdin.WriteAsync(buffer, offset, count);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="columns"></param>
        /// <param name="rows"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public override void SetSize(int columns, int rows)
        {
            if (columns > 0 && rows > 0)
            {
                NativeMethods.WinSize size = new NativeMethods.WinSize(columns, rows);
                int ret = NativeMethods.ioctl(cfg, NativeMethods.TIOCSWINSZ, ref size);
                if (ret < 0) 
                {
                    throw new InvalidOperationException("Call to ioctl(2) failed. Error: " + Marshal.GetLastWin32Error());
                }
            }
        }
    }
}

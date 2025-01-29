using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using static winpty.WinPty;

namespace Pty4Net.Win32
{
    /// <summary>
    /// WinPty Pseudo-terminal implementation for Windows systems.
    /// </summary>
    /// <see href="https://github.com/rprichard/winpty"/>
    internal class WinPtyTerminal : BasePseudoTerminal
    {
        /// <summary>
        /// 
        /// </summary>
        private IntPtr handle = IntPtr.Zero;
        /// <summary>
        /// 
        /// </summary>
        private IntPtr err = IntPtr.Zero;
        /// <summary>
        /// 
        /// </summary>
        private IntPtr cfg = IntPtr.Zero;
        /// <summary>
        /// 
        /// </summary>
        private IntPtr spawnCfg = IntPtr.Zero;
        /// <summary>
        /// 
        /// </summary>
        private Stream stdin;
        /// <summary>
        /// 
        /// </summary>
        private Stream stdout;
        /// <summary>
        /// 
        /// </summary>
        private bool isDisposed;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="process"></param>
        /// <param name="handle"></param>
        /// <param name="cfg"></param>
        /// <param name="spawnCfg"></param>
        /// <param name="err"></param>
        /// <param name="stdin"></param>
        /// <param name="stdout"></param>
        internal WinPtyTerminal(Process process, IntPtr handle, IntPtr cfg, IntPtr spawnCfg, IntPtr err, Stream stdin, Stream stdout) : base(process)
        {
            this.handle = handle;
            this.stdin = stdin;
            this.stdout = stdout;

            this.cfg = cfg;
            this.spawnCfg = spawnCfg;
            this.err = err;
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Dispose()
        {
            if (!isDisposed)
            {
                isDisposed = true;
                stdin?.Dispose();
                stdout?.Dispose();
                winpty_config_free(cfg);
                winpty_spawn_config_free(spawnCfg);
                winpty_error_free(err);
                winpty_free(handle);
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
            if (buffer.Length == 1 && buffer[0] == (byte) '\n')
            {
                buffer[0] = (byte) '\r';
            }

            await stdin.WriteAsync(buffer, offset, count);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="columns"></param>
        /// <param name="rows"></param>
        public override void SetSize(int columns, int rows)
        {
            if (cfg != IntPtr.Zero && columns >= 1 && rows >= 1)
            {
                winpty_set_size(handle, columns, rows, out err);
            }
        }
    }
}

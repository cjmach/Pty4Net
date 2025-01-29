using System.IO;
using System.IO.Pipes;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Pty4Net.Win32
{
    /// <summary>
    /// ConPty Pseudo-terminal implementation for Windows systems.
    /// </summary>
    /// <see href="https://devblogs.microsoft.com/commandline/windows-command-line-introducing-the-windows-pseudo-console-conpty/" />
    internal class ConPtyTerminal : BasePseudoTerminal
    {
        /// <summary>
        /// 
        /// </summary>
        private readonly ProcessInformation processInfo;
        /// <summary>
        /// 
        /// </summary>
        private readonly PseudoConsole console;
        /// <summary>
        /// 
        /// </summary>
        private readonly Pipe input;
        /// <summary>
        /// 
        /// </summary>
        private readonly Pipe output;
        /// <summary>
        /// 
        /// </summary>
        private readonly Stream reader;
        /// <summary>
        /// 
        /// </summary>
        private readonly Stream writer;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="processInfo"></param>
        /// <param name="console"></param>
        /// <param name="input"></param>
        /// <param name="output"></param>
        internal ConPtyTerminal(ProcessInformation processInfo, PseudoConsole console, Pipe input, Pipe output) : base(processInfo.Process)
        {
            this.processInfo = processInfo;
            this.console = console;
            this.input = input;
            this.output = output;
            this.reader = new AnonymousPipeClientStream(PipeDirection.In, output.ReadSide);
            this.writer = new AnonymousPipeClientStream(PipeDirection.Out, input.WriteSide);
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Dispose()
        {
            processInfo.Dispose();
            console.Dispose();
            reader.Dispose();
            writer.Dispose();
            output.Dispose();
            input.Dispose();
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
            return await reader.ReadAsync(buffer, offset, count);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="columns"></param>
        /// <param name="rows"></param>
        public override void SetSize(int columns, int rows)
        {
            if (columns >= 1 && rows >= 1)
            {
                NativeMethods.COORD size = new NativeMethods.COORD(columns, rows);
                int result = NativeMethods.ResizePseudoConsole(console.Handle, size);
                if (result != 0)
                {
                    Marshal.ThrowExceptionForHR(result);
                }
            }
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
            if (buffer.Length == 1 && buffer[0] == (byte)'\n')
            {
                buffer[0] = (byte)'\r';
            }
            await writer.WriteAsync(buffer, offset, count);
        }
    }
}
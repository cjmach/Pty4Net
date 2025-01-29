using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Pty4Net.Win32
{
    internal class ConPtyTerminal : BasePseudoTerminal
    {
        private readonly ProcessInformation processInfo;
        private readonly PseudoConsole console;
        private readonly Pipe input;
        private readonly Pipe output;
        private readonly Stream reader;
        private readonly Stream writer;

        internal ConPtyTerminal(ProcessInformation processInfo, PseudoConsole console, Pipe input, Pipe output) : base(processInfo.Process)
        {
            this.processInfo = processInfo;
            this.console = console;
            this.input = input;
            this.output = output;
            this.reader = new AnonymousPipeClientStream(PipeDirection.In, output.ReadSide);
            this.writer = new AnonymousPipeClientStream(PipeDirection.Out, input.WriteSide);
        }

        public override void Dispose()
        {
            processInfo.Dispose();
            console.Dispose();
            reader.Dispose();
            writer.Dispose();
            output.Dispose();
            input.Dispose();
        }

        public override async Task<int> ReadAsync(byte[] buffer, int offset, int count)
        {
            return await reader.ReadAsync(buffer, offset, count);
        }

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
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Pty4Net.Win32 {
    internal class ConPtyTerminal : IPseudoTerminal
    {
        private readonly ProcessInformation processInfo;
        private readonly PseudoConsole console;
        private readonly Pipe input;
        private readonly Pipe output;
        private readonly Stream reader;
        private readonly Stream writer;

        public Process Process => processInfo.Process;

        internal ConPtyTerminal(ProcessInformation processInfo, PseudoConsole console, Pipe input, Pipe output) {
            this.processInfo = processInfo;
            this.console = console;
            this.input = input;
            this.output = output;
            this.reader = new AnonymousPipeClientStream(PipeDirection.In, output.ReadSide);
            this.writer = new AnonymousPipeClientStream(PipeDirection.Out, input.WriteSide);
        }

        public void Dispose()
        {
            processInfo.Dispose();
            console.Dispose();
            reader.Dispose();
            writer.Dispose();
            output.Dispose();
            input.Dispose();
        }

        public async Task<int> ReadAsync(byte[] buffer, int offset, int count)
        {
            return await reader.ReadAsync(buffer, offset, count);
        }

        public void SetSize(int columns, int rows)
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

        public async Task WriteAsync(byte[] buffer, int offset, int count)
        {
            if (buffer.Length == 1 && buffer[0] == (byte) '\n') {
                buffer[0] = (byte) '\r';
            }
            await writer.WriteAsync(buffer, offset, count);
        }
    }
}
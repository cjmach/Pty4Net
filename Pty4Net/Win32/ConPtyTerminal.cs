using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;

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

        public ConPtyTerminal(ProcessInformation processInfo, PseudoConsole console, Pipe input, Pipe output) {
            this.processInfo = processInfo;
            this.console = console;
            this.input = input;
            this.output = output;
            this.reader = new AnonymousPipeClientStream(PipeDirection.In, input.WriteSide);
            this.writer = new AnonymousPipeClientStream(PipeDirection.Out, output.ReadSide);
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

        public Task<int> ReadAsync(byte[] buffer, int offset, int count)
        {
            return reader.ReadAsync(buffer, offset, count);
        }

        public void SetSize(int columns, int rows)
        {
            NativeMethods.COORD size = new NativeMethods.COORD(columns, rows);
            int result = NativeMethods.ResizePseudoConsole(console.Handle, size);
            if (result != 0) {
                Marshal.ThrowExceptionForHR(result);
            }
        }

        public Task WriteAsync(byte[] buffer, int offset, int count)
        {
            return writer.WriteAsync(buffer, offset, count);
        }
    }
}
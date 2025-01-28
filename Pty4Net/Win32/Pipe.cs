using System;
using Microsoft.Win32.SafeHandles;

namespace Pty4Net.Win32 {

    using static NativeMethods;

    /// <summary>
    /// 
    /// </summary>
    /// <see href="https://docs.microsoft.com/en-us/windows/console/creating-a-pseudoconsole-session"/>
    internal sealed class Pipe : IDisposable
    {
        public SafePipeHandle ReadSide { get; }
        public SafePipeHandle WriteSide { get; }

        public Pipe()
        {
            if (!CreatePipe(out SafePipeHandle readSide, out SafePipeHandle writeSide, IntPtr.Zero, 0))
            {
                throw new InvalidOperationException("failed to create pipe");
            }
            ReadSide = readSide;
            WriteSide = writeSide;
        }

        ~Pipe() {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            ReadSide?.Dispose();
            WriteSide?.Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }}
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
        /// <summary>
        /// 
        /// </summary>
        public SafePipeHandle ReadSide { get; }
        /// <summary>
        /// 
        /// </summary>
        public SafePipeHandle WriteSide { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public Pipe()
        {
            if (!CreatePipe(out SafePipeHandle readSide, out SafePipeHandle writeSide, IntPtr.Zero, 0))
            {
                throw new InvalidOperationException("failed to create pipe");
            }
            ReadSide = readSide;
            WriteSide = writeSide;
        }

        /// <summary>
        /// 
        /// </summary>
        ~Pipe() {
            Dispose(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        private void Dispose(bool disposing)
        {
            ReadSide?.Dispose();
            WriteSide?.Dispose();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }}
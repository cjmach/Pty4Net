using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace Pty4Net.Win32 {

    using static NativeMethods;

    /// <summary>
    /// 
    /// </summary>
    internal sealed class PseudoConsole : IDisposable {
        /// <summary>
        /// 
        /// </summary>
        internal IntPtr Handle { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputReadSide"></param>
        /// <param name="outputWriteSide"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        internal PseudoConsole(SafePipeHandle inputReadSide, SafePipeHandle outputWriteSide, int width, int height) {
            int result = CreatePseudoConsole(new COORD(width, height),
                                                   inputReadSide,
                                                   outputWriteSide,
                                                   0, 
                                                   out IntPtr hPC);
            if(result != 0)
            {
                Marshal.ThrowExceptionForHR(result);
            }
            Handle = hPC;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            ClosePseudoConsole(Handle);
        }
    }
}
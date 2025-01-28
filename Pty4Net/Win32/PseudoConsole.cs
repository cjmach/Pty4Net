using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace Pty4Net.Win32 {

    using static NativeMethods;

    internal sealed class PseudoConsole : IDisposable {
        internal IntPtr Handle { get; }

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

        public void Dispose()
        {
            ClosePseudoConsole(Handle);
        }
    }
}
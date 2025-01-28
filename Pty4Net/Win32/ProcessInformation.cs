using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Pty4Net.Win32 {
    internal class ProcessInformation : IDisposable
    {
        private readonly NativeMethods.STARTUPINFOEX startupInfo;
        private readonly NativeMethods.PROCESS_INFORMATION processInfo;
        private bool disposed;

        internal Process Process { get; }

        internal ProcessInformation(NativeMethods.STARTUPINFOEX startupInfo, NativeMethods.PROCESS_INFORMATION processInfo) {
            this.startupInfo = startupInfo;
            this.processInfo = processInfo;
            Process = Process.GetProcessById(processInfo.dwProcessId);
        }

        ~ProcessInformation() {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            if (!disposed)
            {
                disposed = true;
                startupInfo.Dispose();
                processInfo.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
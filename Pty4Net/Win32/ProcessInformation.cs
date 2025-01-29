using System;
using System.Diagnostics;

namespace Pty4Net.Win32 {
    
    using static NativeMethods;

    internal class ProcessInformation : IDisposable
    {
        private readonly STARTUPINFOEX startupInfo;
        private readonly PROCESS_INFORMATION processInfo;
        private bool disposed;

        internal Process Process { get; }

        internal ProcessInformation(STARTUPINFOEX startupInfo, PROCESS_INFORMATION processInfo) {
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
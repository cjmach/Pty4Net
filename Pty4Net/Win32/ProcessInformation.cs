using System;
using System.Diagnostics;

namespace Pty4Net.Win32 {
    
    using static NativeMethods;

    /// <summary>
    /// 
    /// </summary>
    internal class ProcessInformation : IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        private readonly STARTUPINFOEX startupInfo;
        /// <summary>
        /// 
        /// </summary>
        private readonly PROCESS_INFORMATION processInfo;
        /// <summary>
        /// 
        /// </summary>
        private bool disposed;

        /// <summary>
        /// 
        /// </summary>
        internal Process Process { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startupInfo"></param>
        /// <param name="processInfo"></param>
        internal ProcessInformation(STARTUPINFOEX startupInfo, PROCESS_INFORMATION processInfo) {
            this.startupInfo = startupInfo;
            this.processInfo = processInfo;
            Process = Process.GetProcessById(processInfo.dwProcessId);
        }

        /// <summary>
        /// 
        /// </summary>
        ~ProcessInformation() {
            Dispose(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        private void Dispose(bool disposing)
        {
            if (!disposed)
            {
                disposed = true;
                startupInfo.Dispose();
                processInfo.Dispose();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
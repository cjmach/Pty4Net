using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

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
        private readonly IntPtr environment;
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
        internal ProcessInformation(STARTUPINFOEX startupInfo, PROCESS_INFORMATION processInfo, IntPtr environment) {
            this.startupInfo = startupInfo;
            this.processInfo = processInfo;
            this.environment = environment;
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
                if (environment != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(environment);
                }
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
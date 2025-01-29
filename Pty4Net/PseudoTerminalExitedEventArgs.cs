using System;
using System.Diagnostics;

namespace Pty4Net
{
    /// <summary>
    /// 
    /// </summary>
    public class ProcessExitedEventArgs : EventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        public Process Process { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="process"></param>
        internal ProcessExitedEventArgs(Process process)
        {
            Process = process;
        }
    }
}
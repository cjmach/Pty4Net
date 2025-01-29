using System;
using System.Diagnostics;

namespace Pty4Net
{
    /// <summary>
    /// 
    /// </summary>
    public class PseudoTerminalExitedEventArgs : EventArgs
    {
        /// <summary>
        /// The terminal's process.
        /// </summary>
        public Process Process { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="process"></param>
        internal PseudoTerminalExitedEventArgs(Process process)
        {
            Process = process;
        }
    }
}
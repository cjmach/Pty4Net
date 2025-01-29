using System;
using System.Diagnostics;

namespace Pty4Net
{
    public class ProcessExitedEventArgs : EventArgs
    {
        public Process Process { get; }

        internal ProcessExitedEventArgs(Process process)
        {
            Process = process;
        }
    }
}
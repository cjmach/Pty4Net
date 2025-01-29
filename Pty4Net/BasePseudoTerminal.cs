using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Pty4Net
{

    public abstract class BasePseudoTerminal : IPseudoTerminal
    {
        protected Process Process { get; }

        public event EventHandler<ProcessExitedEventArgs> ProcessExited;

        protected BasePseudoTerminal(Process process)
        {
            Process = process;
            Process.EnableRaisingEvents = true;
            Process.Exited += (sender, args) => OnProcessExited();
        }

        public abstract void SetSize(int columns, int rows);

        public abstract Task WriteAsync(byte[] buffer, int offset, int count);

        public abstract Task<int> ReadAsync(byte[] buffer, int offset, int count);

        public abstract void Dispose();

        protected virtual void OnProcessExited() {
            ProcessExited?.Invoke(this, new ProcessExitedEventArgs(Process));
        }
    }
}
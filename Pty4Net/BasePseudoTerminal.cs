using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Pty4Net
{

    /// <summary>
    /// 
    /// </summary>
    public abstract class BasePseudoTerminal : IPseudoTerminal
    {
        /// <summary>
        /// 
        /// </summary>
        protected Process Process { get; }

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<PseudoTerminalExitedEventArgs> ProcessExited;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="process"></param>
        protected BasePseudoTerminal(Process process)
        {
            Process = process;
            Process.EnableRaisingEvents = true;
            Process.Exited += (sender, args) => OnProcessExited();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="columns"></param>
        /// <param name="rows"></param>
        public abstract void SetSize(int columns, int rows);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public abstract Task WriteAsync(byte[] buffer, int offset, int count);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public abstract Task<int> ReadAsync(byte[] buffer, int offset, int count);

        /// <summary>
        /// 
        /// </summary>
        public abstract void Dispose();

        /// <summary>
        /// 
        /// </summary>
        protected virtual void OnProcessExited() {
            ProcessExited?.Invoke(this, new PseudoTerminalExitedEventArgs(Process));
        }
    }
}
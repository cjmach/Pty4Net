using System;
using System.Threading.Tasks;

namespace Pty4Net
{
    /// <summary>
    /// 
    /// </summary>
    public interface IPseudoTerminal : IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<ProcessExitedEventArgs> ProcessExited;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="columns"></param>
        /// <param name="rows"></param>
        void SetSize(int columns, int rows);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        Task WriteAsync(byte[] buffer, int offset, int count);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        Task<int> ReadAsync(byte[] buffer, int offset, int count);
    }
}

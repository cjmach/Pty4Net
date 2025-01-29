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
        /// Triggered when the terminal's process exits.
        /// </summary>
        public event EventHandler<ProcessExitedEventArgs> ProcessExited;

        /// <summary>
        /// Set terminal window size.
        /// </summary>
        /// <param name="columns">Width of terminal window.</param>
        /// <param name="rows">Height of terminal window.</param>
        /// <exception cref="InvalidOperationException">If operation fails.</exception>
        void SetSize(int columns, int rows);

        /// <summary>
        /// Writes up to <paramref name="count"/> bytes from the <paramref name="buffer"/> 
        /// starting at <paramref name="offset"/> to the terminal.
        /// </summary>
        /// <param name="buffer">The buffer with the bytes to write.</param>
        /// <param name="offset">The zero-based byte offset in buffer at which to begin copying bytes to the terminal.</param>
        /// <param name="count">The number of bytes to write.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        Task WriteAsync(byte[] buffer, int offset, int count);

        /// <summary>
        /// Reads up to <paramref name="count"/> bytes from the terminal into 
        /// the <paramref name="buffer"/> starting at <paramref name="offset"/>.
        /// </summary>
        /// <param name="buffer">The buffer used to store the bytes read from the terminal.</param>
        /// <param name="offset">The zero-based byte offset in buffer at which to begin storing the data read from the terminal.</param>
        /// <param name="count">The number of bytes to be read from the terminal.</param>
        /// <returns>A task that represents the asynchronous read operation. Contains the total number of bytes read into the buffer after completion.</returns>
        Task<int> ReadAsync(byte[] buffer, int offset, int count);
    }
}

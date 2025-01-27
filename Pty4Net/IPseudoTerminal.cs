using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Pty4Net
{
    public interface IPseudoTerminal : IDisposable
    {
        void SetSize(int columns, int rows);

        Task WriteAsync(byte[] buffer, int offset, int count);

        Task<int> ReadAsync(byte[] buffer, int offset, int count);

        Process Process { get; }
    }
}

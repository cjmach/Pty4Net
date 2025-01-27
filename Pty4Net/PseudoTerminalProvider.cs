using System;
using System.Runtime.InteropServices;
using Pty4Net.Unix;
using Pty4Net.Win32;

namespace Pty4Net
{
    public static class PseudoTerminalProvider {
        private static readonly Lazy<IPseudoTerminalProvider> Provider;

        static PseudoTerminalProvider()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Provider = new Lazy<IPseudoTerminalProvider>(() => new WinPtyTerminalProvider());
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Provider = new Lazy<IPseudoTerminalProvider>(() => new UnixPseudoTerminalProvider());
            }
            else
            {
                throw new PlatformNotSupportedException();
            }
        }

        public static IPseudoTerminal Create(int columns, int rows, string initialDirectory, string environment, string command, params string[] arguments) {
            return Provider.Value.Create(columns, rows, initialDirectory, environment, command, arguments);
        }
    }
}
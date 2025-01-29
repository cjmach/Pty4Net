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
                Provider = new Lazy<IPseudoTerminalProvider>(() => new Win32TerminalProvider());
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

        public static IPseudoTerminal Create() {
            PseudoTerminalOptions options = PseudoTerminalOptions.CreateDefault();
            return Create(options);
        }

        public static IPseudoTerminal Create(PseudoTerminalOptions options) {
            return Provider.Value.Create(options);
        }
    }
}
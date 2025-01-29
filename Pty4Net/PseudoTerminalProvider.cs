using System;
using System.Runtime.InteropServices;
using Pty4Net.Unix;
using Pty4Net.Win32;

namespace Pty4Net
{
    /// <summary>
    /// 
    /// </summary>
    public static class PseudoTerminalProvider {
        /// <summary>
        /// 
        /// </summary>
        private static readonly Lazy<IPseudoTerminalProvider> Provider;

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="PlatformNotSupportedException"></exception>
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static IPseudoTerminal CreatePseudoTerminal() {
            PseudoTerminalOptions options = PseudoTerminalOptions.CreateDefault();
            return CreatePseudoTerminal(options);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IPseudoTerminal CreatePseudoTerminal(PseudoTerminalOptions options) {
            return Provider.Value.CreatePseudoTerminal(options);
        }
    }
}
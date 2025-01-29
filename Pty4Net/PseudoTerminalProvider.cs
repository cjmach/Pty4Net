using System;
using System.Runtime.InteropServices;
using Pty4Net.Unix;
using Pty4Net.Win32;

namespace Pty4Net
{
    /// <summary>
    /// A provider of <see cref="IPseudoTerminal"/> instances.
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
        /// Creates a new <see cref="IPseudoTerminal"/> instance with the default options
        /// for the current system.
        /// </summary>
        /// <returns>A reference to a new <see cref="IPseudoTerminal"/> instance.</returns>
        public static IPseudoTerminal CreatePseudoTerminal() {
            PseudoTerminalOptions options = PseudoTerminalOptions.CreateDefault();
            return CreatePseudoTerminal(options);
        }

        /// <summary>
        /// Creates a new <see cref="IPseudoTerminal"/> instance.
        /// </summary>
        /// <param name="options">The options allow to customize how the instance is created.</param>
        /// <returns>A reference to a new <see cref="IPseudoTerminal"/> instance.</returns>
        public static IPseudoTerminal CreatePseudoTerminal(PseudoTerminalOptions options) {
            options.Validate();
            return Provider.Value.CreatePseudoTerminal(options);
        }
    }
}
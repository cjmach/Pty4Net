using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Pty4Net {
    /// <summary>
    /// Options for creating a new pseudo-terminal.
    /// </summary>
    public sealed class PseudoTerminalOptions {
        /// <summary>
        /// Gets or sets the width of the pseudo-terminal window.
        /// </summary>
        public int Columns { get; set; } = Console.WindowWidth;
        /// <summary>
        /// Gets or sets the height of the pseudo-terminal window.
        /// </summary>
        public int Rows { get; set; } = Console.WindowHeight;
        /// <summary>
        /// Gets or sets the initial working directory of the pseudo-terminal. Default is the user's profile directory.
        /// </summary>
        public string InitialDirectory { get; set; } = System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile);
        /// <summary>
        /// 
        /// </summary>
        public string Environment { get; set; } = null;
        /// <summary>
        /// Gets or sets the absolute path of the program to start in the pseudo-terminal.
        /// </summary>
        public string Command { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the command-line arguments passed to the program specified in <see cref="PseudoTerminalOptions.Command"/>.
        /// </summary>
        public string[] Arguments { get; set; } = Array.Empty<string>();

        /// <summary>
        /// Creates a new instance configured with the default options for
        /// the current system.
        /// </summary>
        /// <returns>A new instance.</returns>
        public static PseudoTerminalOptions CreateDefault() {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                return ForWindows();
            }
            return ForUnix();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static PseudoTerminalOptions ForUnix() {
            return new PseudoTerminalOptions
            {
                Command = "/bin/bash",
                Arguments = new string[] { "--login" }
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static PseudoTerminalOptions ForWindows() {
            return new PseudoTerminalOptions
            {
                Command = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.System), "cmd.exe")
            };
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Pty4Net
{
    /// <summary>
    /// Options for creating a new pseudo-terminal.
    /// </summary>
    public sealed class PseudoTerminalOptions
    {
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
        /// Gets or sets the environment variables of the pseudo-terminal process.
        /// </summary>
        public IDictionary Environment { get; set; } = System.Environment.GetEnvironmentVariables();

        internal string[] EnvironmentList
        {
            get
            {
                int i = 0;
                string[] list = new string[Environment.Count + 1];
                foreach (string variable in Environment.Keys)
                {
                    list[i++] = $"{variable}={Environment[variable]}";
                }
                return list;
            }
        }

        internal string EnvironmentString 
        {
            get
            {
                StringBuilder result = new StringBuilder();
                foreach (string variable in Environment.Keys) {
                    result.Append($"{variable}={Environment[variable]}").Append('\0');
                }
                result.Append('\0');
                return result.ToString();
            }
        }
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
        public static PseudoTerminalOptions CreateDefault()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return ForWindows();
            }
            return ForUnix();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static PseudoTerminalOptions ForUnix()
        {
            PseudoTerminalOptions options = new PseudoTerminalOptions
            {
                Command = "/bin/bash",
                Arguments = new string[] { "--login" }
            };
            options.Environment["TERM"] = "xterm256-color";
            return options;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static PseudoTerminalOptions ForWindows()
        {
            return new PseudoTerminalOptions
            {
                Command = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.System), "cmd.exe")
            };
        }
    }
}
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Pty4Net {
    /// <summary>
    /// 
    /// </summary>
    public sealed class PseudoTerminalOptions {
        /// <summary>
        /// 
        /// </summary>
        public int Columns { get; set; } = Console.WindowHeight;
        /// <summary>
        /// 
        /// </summary>
        public int Rows { get; set; } = Console.WindowHeight;
        /// <summary>
        /// 
        /// </summary>
        public string InitialDirectory { get; set; } = System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile);
        /// <summary>
        /// 
        /// </summary>
        public string Environment { get; set; } = null;
        /// <summary>
        /// 
        /// </summary>
        public string Command { get; set; } = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string[] Arguments { get; set; } = Array.Empty<string>();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
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
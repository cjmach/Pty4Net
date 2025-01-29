using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Pty4Net {
    public sealed class PseudoTerminalOptions {
        public int Columns { get; set; } = Console.WindowHeight;
        public int Rows { get; set; } = Console.WindowHeight;
        public string InitialDirectory { get; set; } = System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile);
        public string Environment { get; set; } = null;
        public string Command { get; set; } = string.Empty;
        public string[] Arguments { get; set; } = Array.Empty<string>();

        public static PseudoTerminalOptions CreateDefault() {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                return ForWindows();
            }
            return ForUnix();
        }

        private static PseudoTerminalOptions ForUnix() {
            return new PseudoTerminalOptions
            {
                Command = "/bin/bash",
                Arguments = new string[] { "--login" }
            };
        }

        private static PseudoTerminalOptions ForWindows() {
            return new PseudoTerminalOptions
            {
                Command = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.System), "cmd.exe")
            };
        }
    }
}
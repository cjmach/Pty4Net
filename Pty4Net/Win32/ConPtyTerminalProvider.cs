using System;
using System.Runtime.InteropServices;

namespace Pty4Net.Win32 {
    internal class ConPtyTerminalProvider : IPseudoTerminalProvider
    {
        public IPseudoTerminal Create(int columns, int rows, string initialDirectory, string environment, string command, params string[] arguments)
        {
            Pipe input = new Pipe();
            Pipe output = new Pipe();
            PseudoConsole console = new PseudoConsole(input.ReadSide, output.WriteSide, columns, rows);

            NativeMethods.STARTUPINFOEX startupInfo = new NativeMethods.STARTUPINFOEX();
            startupInfo.Configure(console.Handle);

            bool result = NativeMethods.CreateProcess(null,
                                                      command,
                                                      null,
                                                      null,
                                                      false,
                                                      NativeMethods.EXTENDED_STARTUPINFO_PRESENT | NativeMethods.CREATE_UNICODE_ENVIRONMENT,
                                                      IntPtr.Zero,
                                                      initialDirectory,
                                                      ref startupInfo,
                                                      out NativeMethods.PROCESS_INFORMATION pInfo);
            if (!result) {
                throw new InvalidOperationException("Could not start terminal process. Error: " + Marshal.GetLastWin32Error());
            }
            return new ConPtyTerminal(new ProcessInformation(startupInfo, pInfo), console, input, output);
        }
    }
}
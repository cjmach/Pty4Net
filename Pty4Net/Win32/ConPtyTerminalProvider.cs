using System;
using System.Runtime.InteropServices;

namespace Pty4Net.Win32 {
    internal class ConPtyTerminalProvider : IPseudoTerminalProvider
    {
        public IPseudoTerminal Create(PseudoTerminalOptions options)
        {
            Pipe input = new Pipe();
            Pipe output = new Pipe();
            PseudoConsole console = new PseudoConsole(input.ReadSide, output.WriteSide, options.Columns, options.Rows);

            NativeMethods.STARTUPINFOEX startupInfo = new NativeMethods.STARTUPINFOEX();
            startupInfo.Configure(console.Handle);

            bool result = NativeMethods.CreateProcess(null,
                                                      options.Command,
                                                      null,
                                                      null,
                                                      false,
                                                      NativeMethods.EXTENDED_STARTUPINFO_PRESENT | NativeMethods.CREATE_UNICODE_ENVIRONMENT,
                                                      IntPtr.Zero,
                                                      options.InitialDirectory,
                                                      ref startupInfo,
                                                      out NativeMethods.PROCESS_INFORMATION pInfo);
            if (!result) {
                throw new InvalidOperationException("Could not start terminal process. Error: " + Marshal.GetLastWin32Error());
            }
            return new ConPtyTerminal(new ProcessInformation(startupInfo, pInfo), console, input, output);
        }
    }
}
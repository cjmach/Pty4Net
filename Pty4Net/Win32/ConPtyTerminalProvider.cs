using System;
using System.Runtime.InteropServices;

namespace Pty4Net.Win32 {

    using static NativeMethods;

    /// <summary>
    /// 
    /// </summary>
    internal class ConPtyTerminalProvider : IPseudoTerminalProvider
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public IPseudoTerminal CreatePseudoTerminal(PseudoTerminalOptions options)
        {
            Pipe input = new Pipe();
            Pipe output = new Pipe();
            PseudoConsole console = new PseudoConsole(input.ReadSide, output.WriteSide, options.Columns, options.Rows);

            STARTUPINFOEX startupInfo = new STARTUPINFOEX();
            startupInfo.Configure(console.Handle);

            string env = options.EnvironmentString;
            IntPtr lpEnvironment = Marshal.StringToHGlobalUni(env);
            
            bool result = CreateProcess(null,
                                        options.Command,
                                        null,
                                        null,
                                        false,
                                        EXTENDED_STARTUPINFO_PRESENT | CREATE_UNICODE_ENVIRONMENT,
                                        lpEnvironment,
                                        options.InitialDirectory,
                                        ref startupInfo,
                                        out PROCESS_INFORMATION pInfo);
            if (!result)
            {
                throw new InvalidOperationException("Could not start terminal process. Error: " + Marshal.GetLastWin32Error());
            }
            return new ConPtyTerminal(new ProcessInformation(startupInfo, pInfo, lpEnvironment), console, input, output);
        }
    }
}
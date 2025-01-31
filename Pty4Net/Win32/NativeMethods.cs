using System;
using System.Runtime.InteropServices;
using System.Security;
using Microsoft.Win32.SafeHandles;

namespace Pty4Net.Win32 {

    /// <summary>
    /// 
    /// </summary>
    internal class NativeMethods {
        /// <summary>
        /// 
        /// </summary>
        internal const int STD_OUTPUT_HANDLE = -11;
        /// <summary>
        /// 
        /// </summary>
        internal const uint ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004;
        /// <summary>
        /// 
        /// </summary>
        internal const uint DISABLE_NEWLINE_AUTO_RETURN = 0x0008;
        /// <summary>
        /// 
        /// </summary>
        internal const int CREATE_UNICODE_ENVIRONMENT = 0x00000400;
        /// <summary>
        /// 
        /// </summary>
        internal const int EXTENDED_STARTUPINFO_PRESENT = 0x00080000;
        /// <summary>
        /// 
        /// </summary>
        internal const int STARTF_USESTDHANDLES = 0x00000100;
        /// <summary>
        /// 
        /// </summary>
        internal static readonly IntPtr PROC_THREAD_ATTRIBUTE_PSEUDOCONSOLE = new IntPtr(0x00020016);

        /// <summary>
        /// 
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct COORD
        {
            /// <summary>
            /// 
            /// </summary>
            public ushort X;
            /// <summary>
            /// 
            /// </summary>
            public ushort Y;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            internal COORD(int x, int y) {
                this.X = (ushort) x;
                this.Y = (ushort) y;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct PROCESS_INFORMATION : IDisposable
        {
            /// <summary>
            /// 
            /// </summary>
            public IntPtr hProcess;
            /// <summary>
            /// 
            /// </summary>
            public IntPtr hThread;
            /// <summary>
            /// 
            /// </summary>
            public int dwProcessId;
            /// <summary>
            /// 
            /// </summary>
            public int dwThreadId;

            /// <summary>
            /// 
            /// </summary>
            public void Dispose()
            {
                if (hProcess != IntPtr.Zero)
                {
                    CloseHandle(hProcess);
                    hProcess = IntPtr.Zero;
                }
                if (hThread != IntPtr.Zero)
                {
                    CloseHandle(hThread);
                    hThread = IntPtr.Zero;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct STARTUPINFO
        {
            /// <summary>
            /// 
            /// </summary>
            public int cb;
            /// <summary>
            /// 
            /// </summary>
            public string lpReserved;
            /// <summary>
            /// 
            /// </summary>
            public string lpDesktop;
            /// <summary>
            /// 
            /// </summary>
            public string lpTitle;
            /// <summary>
            /// 
            /// </summary>
            public int dwX;
            /// <summary>
            /// 
            /// </summary>
            public int dwY;
            /// <summary>
            /// 
            /// </summary>
            public int dwXSize;
            /// <summary>
            /// 
            /// </summary>
            public int dwYSize;
            /// <summary>
            /// 
            /// </summary>
            public int dwXCountChars;
            /// <summary>
            /// 
            /// </summary>
            public int dwYCountChars;
            /// <summary>
            /// 
            /// </summary>
            public int dwFillAttribute;
            /// <summary>
            /// 
            /// </summary>
            public int dwFlags;
            /// <summary>
            /// 
            /// </summary>
            public short wShowWindow;
            /// <summary>
            /// 
            /// </summary>
            public short cbReserved2;
            /// <summary>
            /// 
            /// </summary>
            public IntPtr lpReserved2;
            /// <summary>
            /// 
            /// </summary>
            public IntPtr hStdInput;
            /// <summary>
            /// 
            /// </summary>
            public IntPtr hStdOutput;
            /// <summary>
            /// 
            /// </summary>
            public IntPtr hStdError;
        }

        /// <summary>
        /// 
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct STARTUPINFOEX : IDisposable
        {
            /// <summary>
            /// 
            /// </summary>
            public STARTUPINFO StartupInfo;
            /// <summary>
            /// 
            /// </summary>
            public IntPtr lpAttributeList;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="consoleHandle"></param>
            /// <exception cref="InvalidOperationException"></exception>
            internal void Configure(IntPtr consoleHandle)
            {
                // see: https://docs.microsoft.com/en-us/windows/console/creating-a-pseudoconsole-session#preparing-for-creation-of-the-child-process
                this.StartupInfo.cb = Marshal.SizeOf<STARTUPINFOEX>();
                this.StartupInfo.dwFlags = STARTF_USESTDHANDLES;

                IntPtr lpSize = IntPtr.Zero;

                bool result = InitializeProcThreadAttributeList(IntPtr.Zero, 1, 0, ref lpSize);
                if (result || lpSize == IntPtr.Zero)
                {
                    throw new InvalidOperationException("Could not determine the number of bytes for the attribute list. Error: " + Marshal.GetLastWin32Error());
                }

                this.lpAttributeList = Marshal.AllocHGlobal(lpSize);

                result = InitializeProcThreadAttributeList(this.lpAttributeList, 1, 0, ref lpSize);
                if (!result)
                {
                    throw new InvalidOperationException("Could not set up attribute list. Error: " + Marshal.GetLastWin32Error());
                }

                result = UpdateProcThreadAttribute(
                    this.lpAttributeList,
                    0,
                    PROC_THREAD_ATTRIBUTE_PSEUDOCONSOLE,
                    consoleHandle,
                    (IntPtr)IntPtr.Size,
                    IntPtr.Zero,
                    IntPtr.Zero);

                if (!result)
                {
                    throw new InvalidOperationException("Could not set process thread attribute. Error: " + Marshal.GetLastWin32Error());
                }
            }

            /// <summary>
            /// 
            /// </summary>
            public void Dispose()
            {
                if (this.lpAttributeList != IntPtr.Zero)
                {
                    DeleteProcThreadAttributeList(this.lpAttributeList);
                    Marshal.FreeHGlobal(this.lpAttributeList);
                    this.lpAttributeList = IntPtr.Zero;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal class SECURITY_ATTRIBUTES
        {
            /// <summary>
            /// 
            /// </summary>
            public int nLength = Marshal.SizeOf<SECURITY_ATTRIBUTES>();
            /// <summary>
            /// 
            /// </summary>
            public IntPtr lpSecurityDescriptor;
            /// <summary>
            /// 
            /// </summary>
            [MarshalAs(UnmanagedType.Bool)]
            public bool bInheritHandle;
        }

        /// <summary>
        /// Used in unit tests.
        /// </summary>
        internal static void EnableSequenceProcessing()
        {
            SafeFileHandle stdoutHandle = GetStdHandle(STD_OUTPUT_HANDLE);
            if (!GetConsoleMode(stdoutHandle, out uint consoleMode))
            {
                throw new InvalidOperationException("Failed to call GetConsoleMode(). Error: " + Marshal.GetLastWin32Error());
            }

            consoleMode |= ENABLE_VIRTUAL_TERMINAL_PROCESSING | DISABLE_NEWLINE_AUTO_RETURN;
            if (!SetConsoleMode(stdoutHandle, consoleMode))
            {
                throw new InvalidOperationException("Failed to call SetConsoleMode(). Error: " + Marshal.GetLastWin32Error());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nStdHandle"></param>
        /// <returns></returns>
        [DllImport("kernel32", SetLastError = true)]
        internal static extern SafeFileHandle GetStdHandle(int nStdHandle);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hConsoleHandle"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("kernel32", SetLastError = true)]
        internal static extern bool SetConsoleMode(SafeFileHandle hConsoleHandle, uint mode);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("kernel32", SetLastError = true)]
        internal static extern bool GetConsoleMode(SafeFileHandle handle, out uint mode);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hObject"></param>
        /// <returns></returns>
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("kernel32", SetLastError = true)]
        internal static extern bool CloseHandle(IntPtr hObject);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hReadPipe"></param>
        /// <param name="hWritePipe"></param>
        /// <param name="pipeAttributes"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        [DllImport("kernel32", SetLastError = true)]
        [SecurityCritical]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool CreatePipe(out SafePipeHandle hReadPipe,
                                               out SafePipeHandle hWritePipe,
                                               IntPtr pipeAttributes,
                                               int size);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="size"></param>
        /// <param name="hInput"></param>
        /// <param name="hOutput"></param>
        /// <param name="dwFlags"></param>
        /// <param name="phPC"></param>
        /// <returns></returns>
        [DllImport("kernel32", SetLastError = true)]
        internal static extern int CreatePseudoConsole(COORD size, SafePipeHandle hInput, SafePipeHandle hOutput, uint dwFlags, out IntPtr phPC);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hPC"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        [DllImport("kernel32", SetLastError = true)]
        internal static extern int ResizePseudoConsole(IntPtr hPC, COORD size);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hPC"></param>
        /// <returns></returns>
        [DllImport("kernel32", SetLastError = true)]
        internal static extern int ClosePseudoConsole(IntPtr hPC);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lpApplicationName"></param>
        /// <param name="lpCommandLine"></param>
        /// <param name="lpProcessAttributes"></param>
        /// <param name="lpThreadAttributes"></param>
        /// <param name="bInheritHandles"></param>
        /// <param name="dwCreationFlags"></param>
        /// <param name="lpEnvironment"></param>
        /// <param name="lpCurrentDirectory"></param>
        /// <param name="lpStartupInfo"></param>
        /// <param name="lpProcessInformation"></param>
        /// <returns></returns>
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern bool CreateProcess(string lpApplicationName,
                                                  string lpCommandLine,
                                                  SECURITY_ATTRIBUTES lpProcessAttributes,
                                                  SECURITY_ATTRIBUTES lpThreadAttributes,
                                                  bool bInheritHandles,
                                                  uint dwCreationFlags,
                                                  IntPtr lpEnvironment,
                                                  string lpCurrentDirectory,
                                                  [In] ref STARTUPINFOEX lpStartupInfo,
                                                  out PROCESS_INFORMATION lpProcessInformation);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="lpAttributeList"></param>
        /// <returns></returns>
        [DllImport("kernel32", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool DeleteProcThreadAttributeList(IntPtr lpAttributeList);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lpAttributeList"></param>
        /// <param name="dwAttributeCount"></param>
        /// <param name="dwFlags"></param>
        /// <param name="lpSize"></param>
        /// <returns></returns>
        [DllImport("kernel32", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool InitializeProcThreadAttributeList(IntPtr lpAttributeList, 
                                                                      int dwAttributeCount, 
                                                                      int dwFlags, 
                                                                      ref IntPtr lpSize);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lpAttributeList"></param>
        /// <param name="dwFlags"></param>
        /// <param name="Attribute"></param>
        /// <param name="lpValue"></param>
        /// <param name="cbSize"></param>
        /// <param name="lpPreviousValue"></param>
        /// <param name="lpReturnSize"></param>
        /// <returns></returns>
        [DllImport("kernel32", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool UpdateProcThreadAttribute(IntPtr lpAttributeList,
                                                              uint dwFlags,
                                                              IntPtr Attribute,
                                                              IntPtr lpValue,
                                                              IntPtr cbSize,
                                                              IntPtr lpPreviousValue,
                                                              IntPtr lpReturnSize);
    }
}
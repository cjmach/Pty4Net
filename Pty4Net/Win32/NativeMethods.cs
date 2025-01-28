using System;
using System.Runtime.InteropServices;
using System.Security;
using Microsoft.Win32.SafeHandles;

namespace Pty4Net.Win32 {

    internal class NativeMethods {
        internal const int CREATE_UNICODE_ENVIRONMENT = 0x00000400;
        internal const int EXTENDED_STARTUPINFO_PRESENT = 0x00080000;
        internal const int STARTF_USESTDHANDLES = 0x00000100;
        internal static readonly IntPtr PROC_THREAD_ATTRIBUTE_PSEUDOCONSOLE = new IntPtr(0x00020016);

        [StructLayout(LayoutKind.Sequential)]
        internal struct COORD
        {
            public ushort X;
            public ushort Y;

            internal COORD(int x, int y) {
                this.X = (ushort) x;
                this.Y = (ushort) y;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct PROCESS_INFORMATION : IDisposable
        {
            public IntPtr hProcess;
            public IntPtr hThread;
            public int dwProcessId;
            public int dwThreadId;

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

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct STARTUPINFO
        {
            public int cb;
            public string lpReserved;
            public string lpDesktop;
            public string lpTitle;
            public int dwX;
            public int dwY;
            public int dwXSize;
            public int dwYSize;
            public int dwXCountChars;
            public int dwYCountChars;
            public int dwFillAttribute;
            public int dwFlags;
            public short wShowWindow;
            public short cbReserved2;
            public IntPtr lpReserved2;
            public IntPtr hStdInput;
            public IntPtr hStdOutput;
            public IntPtr hStdError;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct STARTUPINFOEX : IDisposable
        {
            public STARTUPINFO StartupInfo;
            public IntPtr lpAttributeList;

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

        [StructLayout(LayoutKind.Sequential)]
        internal class SECURITY_ATTRIBUTES
        {
            public int nLength = Marshal.SizeOf<SECURITY_ATTRIBUTES>();
            public IntPtr lpSecurityDescriptor;
            [MarshalAs(UnmanagedType.Bool)]
            public bool bInheritHandle;
        }

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("kernel32", SetLastError = true)]
        internal static extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernel32", SetLastError = true)]
        [SecurityCritical]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool CreatePipe(out SafePipeHandle hReadPipe,
                                               out SafePipeHandle hWritePipe,
                                               IntPtr pipeAttributes,
                                               int size);
        
        [DllImport("kernel32", SetLastError = true)]
        internal static extern int CreatePseudoConsole(COORD size, SafePipeHandle hInput, SafePipeHandle hOutput, uint dwFlags, out IntPtr phPC);

        [DllImport("kernel32", SetLastError = true)]
        internal static extern int ResizePseudoConsole(IntPtr hPC, COORD size);

        [DllImport("kernel32", SetLastError = true)]
        internal static extern int ClosePseudoConsole(IntPtr hPC);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
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
        
        [DllImport("kernel32", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool DeleteProcThreadAttributeList(IntPtr lpAttributeList);

        [DllImport("kernel32", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool InitializeProcThreadAttributeList(IntPtr lpAttributeList, 
                                                                      int dwAttributeCount, 
                                                                      int dwFlags, 
                                                                      ref IntPtr lpSize);

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
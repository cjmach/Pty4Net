using System;
using System.Runtime.InteropServices;

namespace Pty4Net.Unix
{
    internal sealed class NativeDelegates
    {
        private static readonly IntPtr LibraryHandle;

        static NativeDelegates()
        {
            string libName;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                libName = "c";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                libName = "System";
            }
            else
            {
                throw new PlatformNotSupportedException();
            }
            LibraryHandle = NativeLibrary.Load(libName, typeof(NativeDelegates).Assembly, DllImportSearchPath.LegacyBehavior);
        }

        private NativeDelegates() {}

        internal static T GetProc<T>()
        {
            string name = typeof(T).Name.ToLower();
            return GetProc<T>(name);
        }

        internal static T GetProc<T>(string function)
        {
            IntPtr handle = NativeLibrary.GetExport(LibraryHandle, function);
            return Marshal.GetDelegateForFunctionPointer<T>(handle);
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl, SetLastError = true)]
        internal delegate int Setsid();
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, SetLastError = true)]
        internal delegate int Ioctl(int fd, UInt64 ctl, [In] ref NativeMethods.WinSize arg);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, SetLastError = true)]
        internal delegate int Close(int fd);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, SetLastError = true)]
        internal delegate int Open([MarshalAs(UnmanagedType.LPStr)] string file, int flags);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, SetLastError = true)]
        internal delegate int Chdir([MarshalAs(UnmanagedType.LPStr)] string path);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, SetLastError = true)]
        internal delegate IntPtr Ptsname(int fd);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, SetLastError = true)]
        internal delegate int Grantpt(int fd);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, SetLastError = true)]
        internal delegate int Unlockpt(int fd);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, SetLastError = true)]
        internal delegate int Execve([MarshalAs(UnmanagedType.LPStr)]string path, [MarshalAs(UnmanagedType.LPArray)]string[] argv, [MarshalAs(UnmanagedType.LPArray)]string[] envp);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, SetLastError = true)]
        internal delegate int Waitpid(int pid, IntPtr status, int options);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, SetLastError = true)]
        internal delegate int Write(int fd, byte[] buffer, int length);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, SetLastError = true)]
        internal delegate int posix_spawn_file_actions_adddup2(IntPtr file_actions, int fildes, int newfildes);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, SetLastError = true)]
        internal delegate int posix_spawn_file_actions_addclose(IntPtr file_actions, int fildes);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, SetLastError = true)]
        internal delegate int posix_spawn_file_actions_init(IntPtr file_actions);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, SetLastError = true)]
        internal delegate int posix_spawn_file_actions_destroy(IntPtr file_actions);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, SetLastError = true)]
        internal delegate int posix_spawnattr_init(IntPtr attributes);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, SetLastError = true)]
        internal delegate int posix_spawnattr_destroy(IntPtr attributes);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, SetLastError = true)]
        internal delegate int posix_spawnp(out IntPtr pid, string path, IntPtr fileActions, IntPtr attrib, string[] argv, string[] envp);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, SetLastError = true)]
        internal delegate int Dup(int fd);
    }

    internal static class NativeMethods
    {
        internal const int O_RDONLY = 0x0000;
        internal const int O_WRONLY = 0x0001;
        internal const int O_RDWR = 0x0002;
        internal const int O_ACCMODE = 0x0003;

        internal const int O_CREAT = 0x0100; /* second byte, away from DOS bits */
        internal const int O_EXCL = 0x0200;
        internal const int O_NOCTTY = 0x0400;
        internal const int O_TRUNC = 0x0800;
        internal const int O_APPEND = 0x1000;
        internal const int O_NONBLOCK = 0x2000;
        internal static readonly ulong TIOCSWINSZ = RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? 0x80087467 : 0x5414;

        internal const int _SC_OPEN_MAX = 5;

        internal const int EAGAIN = 11;  /* Try again */

        internal const int EINTR = 4; /* Interrupted system call */

        internal const int ENOENT = 2;

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct WinSize
        {
            public ushort ws_row;   /* rows, in characters */
            public ushort ws_col;   /* columns, in characters */
            public ushort ws_xpixel;    /* horizontal size, pixels */
            public ushort ws_ypixel;    /* vertical size, pixels */

            internal WinSize(int cols, int rows) {
                this.ws_col = (ushort) cols;
                this.ws_row = (ushort) rows;
            }
        };

        internal static readonly NativeDelegates.Open open = NativeDelegates.GetProc<NativeDelegates.Open>();
        internal static readonly NativeDelegates.Write write = NativeDelegates.GetProc<NativeDelegates.Write>();
        internal static readonly NativeDelegates.Grantpt grantpt = NativeDelegates.GetProc<NativeDelegates.Grantpt>();
        internal static readonly NativeDelegates.Unlockpt unlockpt = NativeDelegates.GetProc<NativeDelegates.Unlockpt>();
        internal static readonly NativeDelegates.Ptsname ptsname = NativeDelegates.GetProc<NativeDelegates.Ptsname>();
        internal static readonly NativeDelegates.Waitpid waitpid = NativeDelegates.GetProc<NativeDelegates.Waitpid>();
        internal static readonly NativeDelegates.posix_spawn_file_actions_init posix_spawn_file_actions_init = NativeDelegates.GetProc<NativeDelegates.posix_spawn_file_actions_init>();
        internal static readonly NativeDelegates.posix_spawn_file_actions_destroy posix_spawn_file_actions_destroy = NativeDelegates.GetProc<NativeDelegates.posix_spawn_file_actions_destroy>();
        internal static readonly NativeDelegates.posix_spawn_file_actions_adddup2 posix_spawn_file_actions_adddup2 = NativeDelegates.GetProc<NativeDelegates.posix_spawn_file_actions_adddup2>();
        internal static readonly NativeDelegates.posix_spawn_file_actions_addclose posix_spawn_file_actions_addclose = NativeDelegates.GetProc<NativeDelegates.posix_spawn_file_actions_addclose>();
        internal static readonly NativeDelegates.posix_spawnattr_init posix_spawnattr_init = NativeDelegates.GetProc<NativeDelegates.posix_spawnattr_init>();
        internal static readonly NativeDelegates.posix_spawnattr_destroy posix_spawnattr_destroy = NativeDelegates.GetProc<NativeDelegates.posix_spawnattr_destroy>();
        internal static readonly NativeDelegates.posix_spawnp posix_spawnp = NativeDelegates.GetProc<NativeDelegates.posix_spawnp>();
        internal static readonly NativeDelegates.Dup dup = NativeDelegates.GetProc<NativeDelegates.Dup>();
        internal static readonly NativeDelegates.Ioctl ioctl = NativeDelegates.GetProc<NativeDelegates.Ioctl>();
    }
}

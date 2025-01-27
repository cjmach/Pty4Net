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

        internal delegate void Dup2(int oldfd, int newfd);
        internal delegate int Fork();
        internal delegate void Setsid();
        internal delegate int Ioctl(int fd, UInt64 ctl, IntPtr arg);
        internal delegate void Close(int fd);
        internal delegate int Open([MarshalAs(UnmanagedType.LPStr)] string file, int flags);
        internal delegate int Chdir([MarshalAs(UnmanagedType.LPStr)] string path);
        internal delegate IntPtr Ptsname(int fd);
        internal delegate int Grantpt(int fd);
        internal delegate int Unlockpt(int fd);
        internal delegate void Execve([MarshalAs(UnmanagedType.LPStr)]string path, [MarshalAs(UnmanagedType.LPArray)]string[] argv, [MarshalAs(UnmanagedType.LPArray)]string[] envp);
        internal delegate int Read(int fd, IntPtr buffer, int length);
        internal delegate int Write(int fd, IntPtr buffer, int length);
        internal delegate void Free(IntPtr ptr);
        internal delegate int Pipe(IntPtr[] fds);
        internal delegate int Setpgid(int pid, int pgid);
        internal delegate int posix_spawn_file_actions_adddup2(IntPtr file_actions, int fildes, int newfildes);
        internal delegate int posix_spawn_file_actions_addclose(IntPtr file_actions, int fildes);
        internal delegate int posix_spawn_file_actions_init(IntPtr file_actions);
        internal delegate int posix_spawnattr_init(IntPtr attributes);
        internal delegate int posix_spawnp(out IntPtr pid, string path, IntPtr fileActions, IntPtr attrib, string[] argv, string[] envp);
        internal delegate int Dup(int fd);
        internal delegate void _exit(int code);
        internal delegate int Getdtablesize();
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

        internal static readonly ulong TIOCSCTTY = RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? (ulong)0x20007484 : 0x540E;

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct WinSize
        {
            public ushort ws_row;   /* rows, in characters */
            public ushort ws_col;   /* columns, in characters */
            public ushort ws_xpixel;    /* horizontal size, pixels */
            public ushort ws_ypixel;    /* vertical size, pixels */
        };

        internal static NativeDelegates.Open open = NativeDelegates.GetProc<NativeDelegates.Open>();
        internal static NativeDelegates.Chdir chdir = NativeDelegates.GetProc<NativeDelegates.Chdir>();
        internal static NativeDelegates.Write write = NativeDelegates.GetProc<NativeDelegates.Write>();
        internal static NativeDelegates.Grantpt grantpt = NativeDelegates.GetProc<NativeDelegates.Grantpt>();
        internal static NativeDelegates.Unlockpt unlockpt = NativeDelegates.GetProc<NativeDelegates.Unlockpt>();
        internal static NativeDelegates.Ptsname ptsname = NativeDelegates.GetProc<NativeDelegates.Ptsname>();
        internal static NativeDelegates.posix_spawn_file_actions_init posix_spawn_file_actions_init = NativeDelegates.GetProc<NativeDelegates.posix_spawn_file_actions_init>();
        internal static NativeDelegates.posix_spawn_file_actions_adddup2 posix_spawn_file_actions_adddup2 = NativeDelegates.GetProc<NativeDelegates.posix_spawn_file_actions_adddup2>();
        internal static NativeDelegates.posix_spawn_file_actions_addclose posix_spawn_file_actions_addclose = NativeDelegates.GetProc<NativeDelegates.posix_spawn_file_actions_addclose>();
        internal static NativeDelegates.posix_spawnattr_init posix_spawnattr_init = NativeDelegates.GetProc<NativeDelegates.posix_spawnattr_init>();
        internal static NativeDelegates.posix_spawnp posix_spawnp = NativeDelegates.GetProc<NativeDelegates.posix_spawnp>();
        internal static NativeDelegates.Dup dup = NativeDelegates.GetProc<NativeDelegates.Dup>();
        internal static NativeDelegates.Setsid setsid = NativeDelegates.GetProc<NativeDelegates.Setsid>();
        internal static NativeDelegates.Ioctl ioctl = NativeDelegates.GetProc<NativeDelegates.Ioctl>();        
        internal static NativeDelegates.Execve execve = NativeDelegates.GetProc<NativeDelegates.Execve>();

        internal static IntPtr StructToPtr(object obj)
        {
            var ptr = Marshal.AllocHGlobal(Marshal.SizeOf(obj));
            Marshal.StructureToPtr(obj, ptr, false);
            return ptr;
        }


    }
}

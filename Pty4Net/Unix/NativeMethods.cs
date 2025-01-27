using System;
using System.Runtime.InteropServices;

namespace Pty4Net.Unix
{
    internal class NativeDelegates
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

        public static T GetProc<T>()
        {
            string name = typeof(T).Name.ToLower();
            return GetProc<T>(name);
        }

        public static T GetProc<T>(string function)
        {
            IntPtr handle = NativeLibrary.GetExport(LibraryHandle, function);
            return Marshal.GetDelegateForFunctionPointer<T>(handle);
        }

        public delegate void Dup2(int oldfd, int newfd);
        public delegate int Fork();
        public delegate void Setsid();
        public delegate int Ioctl(int fd, UInt64 ctl, IntPtr arg);
        public delegate void Close(int fd);
        public delegate int Open([MarshalAs(UnmanagedType.LPStr)] string file, int flags);
        public delegate int Chdir([MarshalAs(UnmanagedType.LPStr)] string path);
        public delegate IntPtr Ptsname(int fd);
        public delegate int Grantpt(int fd);
        public delegate int Unlockpt(int fd);
        public delegate void Execve([MarshalAs(UnmanagedType.LPStr)]string path, [MarshalAs(UnmanagedType.LPArray)]string[] argv, [MarshalAs(UnmanagedType.LPArray)]string[] envp);
        public delegate int Read(int fd, IntPtr buffer, int length);
        public delegate int Write(int fd, IntPtr buffer, int length);
        public delegate void Free(IntPtr ptr);
        public delegate int Pipe(IntPtr[] fds);
        public delegate int Setpgid(int pid, int pgid);
        public delegate int posix_spawn_file_actions_adddup2(IntPtr file_actions, int fildes, int newfildes);
        public delegate int posix_spawn_file_actions_addclose(IntPtr file_actions, int fildes);
        public delegate int posix_spawn_file_actions_init(IntPtr file_actions);
        public delegate int posix_spawnattr_init(IntPtr attributes);
        public delegate int posix_spawnp(out IntPtr pid, string path, IntPtr fileActions, IntPtr attrib, string[] argv, string[] envp);
        public delegate int Dup(int fd);
        public delegate void _exit(int code);
        public delegate int Getdtablesize();
    }

    internal static class NativeMethods
    {
        public const int O_RDONLY = 0x0000;
        public const int O_WRONLY = 0x0001;
        public const int O_RDWR = 0x0002;
        public const int O_ACCMODE = 0x0003;

        public const int O_CREAT = 0x0100; /* second byte, away from DOS bits */
        public const int O_EXCL = 0x0200;
        public const int O_NOCTTY = 0x0400;
        public const int O_TRUNC = 0x0800;
        public const int O_APPEND = 0x1000;
        public const int O_NONBLOCK = 0x2000;
        public static readonly ulong TIOCSWINSZ = RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? 0x80087467 : 0x5414;

        public const int _SC_OPEN_MAX = 5;

        public const int EAGAIN = 11;  /* Try again */

        public const int EINTR = 4; /* Interrupted system call */

        public const int ENOENT = 2;

        public static readonly ulong TIOCSCTTY = RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? (ulong)0x20007484 : 0x540E;

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct WinSize
        {
            public ushort ws_row;   /* rows, in characters */
            public ushort ws_col;   /* columns, in characters */
            public ushort ws_xpixel;    /* horizontal size, pixels */
            public ushort ws_ypixel;    /* vertical size, pixels */
        };

        public static NativeDelegates.Open open = NativeDelegates.GetProc<NativeDelegates.Open>();
        public static NativeDelegates.Chdir chdir = NativeDelegates.GetProc<NativeDelegates.Chdir>();
        public static NativeDelegates.Write write = NativeDelegates.GetProc<NativeDelegates.Write>();
        public static NativeDelegates.Grantpt grantpt = NativeDelegates.GetProc<NativeDelegates.Grantpt>();
        public static NativeDelegates.Unlockpt unlockpt = NativeDelegates.GetProc<NativeDelegates.Unlockpt>();
        public static NativeDelegates.Ptsname ptsname = NativeDelegates.GetProc<NativeDelegates.Ptsname>();
        public static NativeDelegates.posix_spawn_file_actions_init posix_spawn_file_actions_init = NativeDelegates.GetProc<NativeDelegates.posix_spawn_file_actions_init>();
        public static NativeDelegates.posix_spawn_file_actions_adddup2 posix_spawn_file_actions_adddup2 = NativeDelegates.GetProc<NativeDelegates.posix_spawn_file_actions_adddup2>();
        public static NativeDelegates.posix_spawn_file_actions_addclose posix_spawn_file_actions_addclose = NativeDelegates.GetProc<NativeDelegates.posix_spawn_file_actions_addclose>();
        public static NativeDelegates.posix_spawnattr_init posix_spawnattr_init = NativeDelegates.GetProc<NativeDelegates.posix_spawnattr_init>();
        public static NativeDelegates.posix_spawnp posix_spawnp = NativeDelegates.GetProc<NativeDelegates.posix_spawnp>();
        public static NativeDelegates.Dup dup = NativeDelegates.GetProc<NativeDelegates.Dup>();
        public static NativeDelegates.Setsid setsid = NativeDelegates.GetProc<NativeDelegates.Setsid>();
        public static NativeDelegates.Ioctl ioctl = NativeDelegates.GetProc<NativeDelegates.Ioctl>();        
        public static NativeDelegates.Execve execve = NativeDelegates.GetProc<NativeDelegates.Execve>();

        public static IntPtr StructToPtr(object obj)
        {
            var ptr = Marshal.AllocHGlobal(Marshal.SizeOf(obj));
            Marshal.StructureToPtr(obj, ptr, false);
            return ptr;
        }


    }
}

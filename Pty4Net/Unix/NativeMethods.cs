using System;
using System.Runtime.InteropServices;

namespace Pty4Net.Unix
{
    /// <summary>
    /// 
    /// </summary>
    internal sealed class NativeDelegates
    {
        /// <summary>
        /// 
        /// </summary>
        private static readonly IntPtr LibraryHandle;

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="PlatformNotSupportedException"></exception>
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

        /// <summary>
        /// 
        /// </summary>
        private NativeDelegates() {}

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        internal static T GetProc<T>()
        {
            string name = typeof(T).Name.ToLower();
            return GetProc<T>(name);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="function"></param>
        /// <returns></returns>
        internal static T GetProc<T>(string function)
        {
            IntPtr handle = NativeLibrary.GetExport(LibraryHandle, function);
            return Marshal.GetDelegateForFunctionPointer<T>(handle);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, SetLastError = true)]
        internal delegate int Setsid();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fd"></param>
        /// <param name="ctl"></param>
        /// <param name="arg"></param>
        /// <returns></returns>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, SetLastError = true)]
        internal delegate int Ioctl(int fd, UInt64 ctl, [In] ref NativeMethods.WinSize arg);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fd"></param>
        /// <returns></returns>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, SetLastError = true)]
        internal delegate int Close(int fd);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, SetLastError = true)]
        internal delegate int Open([MarshalAs(UnmanagedType.LPStr)] string file, int flags);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, SetLastError = true)]
        internal delegate int Chdir([MarshalAs(UnmanagedType.LPStr)] string path);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fd"></param>
        /// <returns></returns>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, SetLastError = true)]
        internal delegate IntPtr Ptsname(int fd);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fd"></param>
        /// <returns></returns>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, SetLastError = true)]
        internal delegate int Grantpt(int fd);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fd"></param>
        /// <returns></returns>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, SetLastError = true)]
        internal delegate int Unlockpt(int fd);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="argv"></param>
        /// <param name="envp"></param>
        /// <returns></returns>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, SetLastError = true)]
        internal delegate int Execve([MarshalAs(UnmanagedType.LPStr)]string path, [MarshalAs(UnmanagedType.LPArray)]string[] argv, [MarshalAs(UnmanagedType.LPArray)]string[] envp);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pid"></param>
        /// <param name="status"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, SetLastError = true)]
        internal delegate int Waitpid(int pid, IntPtr status, int options);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fd"></param>
        /// <param name="buffer"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, SetLastError = true)]
        internal delegate int Write(int fd, byte[] buffer, int length);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file_actions"></param>
        /// <param name="fildes"></param>
        /// <param name="newfildes"></param>
        /// <returns></returns>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, SetLastError = true)]
        internal delegate int posix_spawn_file_actions_adddup2(IntPtr file_actions, int fildes, int newfildes);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file_actions"></param>
        /// <param name="fildes"></param>
        /// <returns></returns>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, SetLastError = true)]
        internal delegate int posix_spawn_file_actions_addclose(IntPtr file_actions, int fildes);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file_actions"></param>
        /// <returns></returns>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, SetLastError = true)]
        internal delegate int posix_spawn_file_actions_init(IntPtr file_actions);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file_actions"></param>
        /// <returns></returns>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, SetLastError = true)]
        internal delegate int posix_spawn_file_actions_destroy(IntPtr file_actions);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="attributes"></param>
        /// <returns></returns>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, SetLastError = true)]
        internal delegate int posix_spawnattr_init(IntPtr attributes);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="attributes"></param>
        /// <returns></returns>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, SetLastError = true)]
        internal delegate int posix_spawnattr_destroy(IntPtr attributes);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pid"></param>
        /// <param name="path"></param>
        /// <param name="fileActions"></param>
        /// <param name="attrib"></param>
        /// <param name="argv"></param>
        /// <param name="envp"></param>
        /// <returns></returns>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, SetLastError = true)]
        internal delegate int posix_spawnp(out IntPtr pid, string path, IntPtr fileActions, IntPtr attrib, string[] argv, string[] envp);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fd"></param>
        /// <returns></returns>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, SetLastError = true)]
        internal delegate int Dup(int fd);
    }

    /// <summary>
    /// 
    /// </summary>
    internal static class NativeMethods
    {
        /// <summary>
        /// 
        /// </summary>
        internal const int O_RDWR = 0x0002;
        /// <summary>
        /// 
        /// </summary>
        internal const int O_NOCTTY = 0x0400;
        /// <summary>
        /// 
        /// </summary>
        internal static readonly ulong TIOCSWINSZ = RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? 0x80087467 : 0x5414;

        /// <summary>
        /// Used to get or set the terminal window size (see man ioctl_tty(2)).
        /// </summary>
        /// <see href="https://man7.org/linux/man-pages/man2/TIOCSWINSZ.2const.html"/>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct WinSize
        {
            /// <summary>
            /// Rows in characters.
            /// </summary>
            public ushort ws_row;
            /// <summary>
            /// Columns in characters.
            /// </summary>
            public ushort ws_col;
            /// <summary>
            /// Unused.
            /// </summary>
            public ushort ws_xpixel;
            /// <summary>
            /// Unused.
            /// </summary>
            public ushort ws_ypixel;

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="cols">Columns in characters.</param>
            /// <param name="rows">Rows in characters.</param>
            internal WinSize(int cols, int rows) {
                this.ws_col = (ushort) cols;
                this.ws_row = (ushort) rows;
            }
        };

        /// <summary>
        /// 
        /// </summary>
        internal static readonly NativeDelegates.Open open = NativeDelegates.GetProc<NativeDelegates.Open>();
        /// <summary>
        /// 
        /// </summary>
        internal static readonly NativeDelegates.Write write = NativeDelegates.GetProc<NativeDelegates.Write>();
        /// <summary>
        /// 
        /// </summary>
        internal static readonly NativeDelegates.Grantpt grantpt = NativeDelegates.GetProc<NativeDelegates.Grantpt>();
        /// <summary>
        /// 
        /// </summary>
        internal static readonly NativeDelegates.Unlockpt unlockpt = NativeDelegates.GetProc<NativeDelegates.Unlockpt>();
        /// <summary>
        /// 
        /// </summary>
        internal static readonly NativeDelegates.Ptsname ptsname = NativeDelegates.GetProc<NativeDelegates.Ptsname>();
        /// <summary>
        /// 
        /// </summary>
        internal static readonly NativeDelegates.Waitpid waitpid = NativeDelegates.GetProc<NativeDelegates.Waitpid>();
        /// <summary>
        /// 
        /// </summary>
        internal static readonly NativeDelegates.posix_spawn_file_actions_init posix_spawn_file_actions_init = NativeDelegates.GetProc<NativeDelegates.posix_spawn_file_actions_init>();
        /// <summary>
        /// 
        /// </summary>
        internal static readonly NativeDelegates.posix_spawn_file_actions_destroy posix_spawn_file_actions_destroy = NativeDelegates.GetProc<NativeDelegates.posix_spawn_file_actions_destroy>();
        /// <summary>
        /// 
        /// </summary>
        internal static readonly NativeDelegates.posix_spawn_file_actions_adddup2 posix_spawn_file_actions_adddup2 = NativeDelegates.GetProc<NativeDelegates.posix_spawn_file_actions_adddup2>();
        /// <summary>
        /// 
        /// </summary>
        internal static readonly NativeDelegates.posix_spawn_file_actions_addclose posix_spawn_file_actions_addclose = NativeDelegates.GetProc<NativeDelegates.posix_spawn_file_actions_addclose>();
        /// <summary>
        /// 
        /// </summary>
        internal static readonly NativeDelegates.posix_spawnattr_init posix_spawnattr_init = NativeDelegates.GetProc<NativeDelegates.posix_spawnattr_init>();
        /// <summary>
        /// 
        /// </summary>
        internal static readonly NativeDelegates.posix_spawnattr_destroy posix_spawnattr_destroy = NativeDelegates.GetProc<NativeDelegates.posix_spawnattr_destroy>();
        /// <summary>
        /// 
        /// </summary>
        internal static readonly NativeDelegates.posix_spawnp posix_spawnp = NativeDelegates.GetProc<NativeDelegates.posix_spawnp>();
        /// <summary>
        /// 
        /// </summary>
        internal static readonly NativeDelegates.Dup dup = NativeDelegates.GetProc<NativeDelegates.Dup>();
        /// <summary>
        /// 
        /// </summary>
        internal static readonly NativeDelegates.Ioctl ioctl = NativeDelegates.GetProc<NativeDelegates.Ioctl>();
    }
}

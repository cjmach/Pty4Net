using System;
using System.Runtime.InteropServices;

namespace Pty4Net.UnixSlave;

[UnmanagedFunctionPointer(CallingConvention.Cdecl, SetLastError = true)]
internal delegate int SetSid();

[UnmanagedFunctionPointer(CallingConvention.Cdecl, SetLastError = true)]
internal delegate int Ioctl(int fd, ulong ctl, IntPtr arg);

[UnmanagedFunctionPointer(CallingConvention.Cdecl, SetLastError = true)]
internal delegate int Chdir([MarshalAs(UnmanagedType.LPStr)] string path);

[UnmanagedFunctionPointer(CallingConvention.Cdecl, SetLastError = true)]
internal delegate int Execve([MarshalAs(UnmanagedType.LPStr)] string path, 
                           [MarshalAs(UnmanagedType.LPArray)] string[] argv, 
                           [MarshalAs(UnmanagedType.LPArray)] string[] envp);

internal class NativeMethods
{
    private static readonly SetSid SetSidDelegate;
    private static readonly Ioctl IoctlDelegate;
    private static readonly Chdir ChdirDelegate;
    private static readonly Execve ExecveDelegate;

    internal static readonly ulong TIOCSCTTY;

    static NativeMethods()
    {
        string libName;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) 
        {
            libName = "c";
            TIOCSCTTY = 0x540E;
        } 
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) 
        {
            libName = "System";
            TIOCSCTTY = unchecked((ulong) 0x20007484);
        }
        else 
        {
            throw new NotSupportedException("OS not supported.");
        }
        IntPtr libHandle = NativeLibrary.Load(libName, typeof(NativeMethods).Assembly, DllImportSearchPath.LegacyBehavior);

        SetSidDelegate = GetDelegate<SetSid>(libHandle, "setsid");
        IoctlDelegate = GetDelegate<Ioctl>(libHandle, "ioctl");
        ChdirDelegate = GetDelegate<Chdir>(libHandle, "chdir");
        ExecveDelegate = GetDelegate<Execve>(libHandle, "execve");
    }

    private static T GetDelegate<T>(IntPtr libHandle, string name) where T : Delegate {
        IntPtr handle = NativeLibrary.GetExport(libHandle, name);
        return Marshal.GetDelegateForFunctionPointer<T>(handle);
    }

    internal static int SetSid() {
        return SetSidDelegate.Invoke();
    }

    internal static int Ioctl(int fd, ulong ctl, IntPtr arg) {
        return IoctlDelegate.Invoke(fd, ctl, arg);
    }

    internal static int Chdir(string path) {
        return ChdirDelegate.Invoke(path);
    }

    internal static int Execve(string path, string[] argv, string[] envp) {
        return ExecveDelegate.Invoke(path, argv, envp);
    }
}
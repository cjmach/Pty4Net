using System;
using System.Runtime.InteropServices;

namespace Pty4Net.Win32
{
    internal class Win32TerminalProvider : IPseudoTerminalProvider
    {
        private static readonly bool IsConPtySupported;

        static Win32TerminalProvider()
        {
            IntPtr kernelLib = NativeLibrary.Load("kernel32", typeof(Win32TerminalProvider).Assembly, DllImportSearchPath.System32);
            try
            {
                IntPtr handle = NativeLibrary.GetExport(kernelLib, "CreatePseudoConsole");
                IsConPtySupported = handle != IntPtr.Zero; // paranoia check
            }
            catch (EntryPointNotFoundException)
            {
                IsConPtySupported = false;
            }
            finally
            {
                NativeLibrary.Free(kernelLib);
            }
        }

        public IPseudoTerminal Create(PseudoTerminalOptions options)
        {
            IPseudoTerminalProvider provider;
            if (IsConPtySupported)
            {
                provider = new ConPtyTerminalProvider();
            }
            else
            {
                provider = new WinPtyTerminalProvider();
            }
            return provider.Create(options);
        }
    }
}
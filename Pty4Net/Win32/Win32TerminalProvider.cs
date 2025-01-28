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

        public IPseudoTerminal Create(int columns, int rows, string initialDirectory, string environment, string command, params string[] arguments)
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
            return provider.Create(columns, rows, initialDirectory, environment, command, arguments);
        }
    }
}
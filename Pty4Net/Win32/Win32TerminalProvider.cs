using System;
using System.Runtime.InteropServices;

namespace Pty4Net.Win32
{
    /// <summary>
    /// 
    /// </summary>
    internal class Win32TerminalProvider : IPseudoTerminalProvider
    {
        /// <summary>
        /// 
        /// </summary>
        private static readonly bool IsConPtySupported;

        /// <summary>
        /// 
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public IPseudoTerminal CreatePseudoTerminal(PseudoTerminalOptions options)
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
            return provider.CreatePseudoTerminal(options);
        }
    }
}
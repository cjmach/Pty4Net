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
        private static readonly IPseudoTerminalProvider Provider;

        /// <summary>
        /// 
        /// </summary>
        static Win32TerminalProvider()
        {
            IntPtr kernelLib = NativeLibrary.Load("kernel32", typeof(Win32TerminalProvider).Assembly, DllImportSearchPath.System32);
            try
            {
                IntPtr handle = NativeLibrary.GetExport(kernelLib, "CreatePseudoConsole");
                if (handle != IntPtr.Zero) // paranoia check
                {
                    Provider = new ConPtyTerminalProvider();
                }
            }
            catch (EntryPointNotFoundException)
            {
                Provider = new WinPtyTerminalProvider();
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
            return Provider.CreatePseudoTerminal(options);
        }
    }
}
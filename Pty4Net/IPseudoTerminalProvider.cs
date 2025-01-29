namespace Pty4Net
{
    /// <summary>
    /// 
    /// </summary>
    public interface IPseudoTerminalProvider
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        IPseudoTerminal CreatePseudoTerminal(PseudoTerminalOptions options);
    }
}

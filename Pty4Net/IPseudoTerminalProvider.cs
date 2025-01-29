namespace Pty4Net
{
    /// <summary>
    /// A provider of <see cref="IPseudoTerminal"/> instances. Abstracts away from the 
    /// complexities involved in the creation of a platform-specific pseudo-terminal process.
    /// </summary>
    public interface IPseudoTerminalProvider
    {
        /// <summary>
        /// Creates a new <see cref="IPseudoTerminal"/> instance.
        /// </summary>
        /// <param name="options">The options allow to customize how the instance is created.</param>
        /// <returns>A reference to a new <see cref="IPseudoTerminal"/> instance.</returns>
        IPseudoTerminal CreatePseudoTerminal(PseudoTerminalOptions options);
    }
}

namespace Pty4Net
{
    public interface IPseudoTerminalProvider
    {
        IPseudoTerminal Create(PseudoTerminalOptions options);
    }
}

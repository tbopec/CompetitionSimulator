using RoboCoP.Messages;

namespace RoboCoP.Exceptions
{
    /// <summary>
    /// <see cref="Message"/> with incorrect format found.
    /// </summary>
    public class ProtocolFormatException: ProtocolException
    {
        public ProtocolFormatException(string message): base(message) {}
    }
}
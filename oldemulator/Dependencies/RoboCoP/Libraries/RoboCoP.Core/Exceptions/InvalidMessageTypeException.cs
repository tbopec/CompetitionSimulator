using System;
using RoboCoP.Messages;

namespace RoboCoP.Exceptions
{
    /// <summary>
    /// <see cref="Message"/> of incorrect type received in entity which works with messages of pre-defined type.
    /// </summary>
    public class InvalidMessageTypeException: ProtocolException
    {
        public InvalidMessageTypeException(Type expected, Type real)
            : base(string.Format("Message of type {0} received when message with type {1} were expected.",
                                 real.Name, expected.Name)) {}
    }
}
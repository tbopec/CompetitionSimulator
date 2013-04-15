using System;
using RoboCoP.Messages;

namespace RoboCoP.Exceptions
{
    /// <summary>
    /// <see cref="Message"/> of incorrect format was passed as an argument.
    /// </summary>
    public class MessageValidationFailed: ArgumentException
    {
        public MessageValidationFailed(string message): base("Message passed as an argument was invalid: " + message) {}
    }
}
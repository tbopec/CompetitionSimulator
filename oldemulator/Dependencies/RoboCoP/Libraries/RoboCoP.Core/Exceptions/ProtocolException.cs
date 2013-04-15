using System;
using System.IO;

namespace RoboCoP.Exceptions
{
    /// <summary>
    /// Base exceptions for all RoboCoP errors.
    /// </summary>
    public abstract class ProtocolException: IOException
    {
        protected ProtocolException(string message): base(message) {}
        protected ProtocolException(string message, Exception inner): base(message, inner) {}
    }
}
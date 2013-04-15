using System.IO;

namespace RoboCoP.Exceptions
{
    /// <summary>
    /// IOException was caught by the code. This exception is critical and user should be informed about it.
    /// </summary>
    public class LostIOException: IOException
    {
        public LostIOException(string discoveryString)
            : base("IOException was caught and then ignored by the code. " + discoveryString) {}

        public LostIOException(IOException exception): base("IOException was caught and re-thrown by the code.", exception) {}
    }
}
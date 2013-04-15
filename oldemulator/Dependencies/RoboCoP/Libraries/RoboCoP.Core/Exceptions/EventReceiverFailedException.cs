using System.IO;
using RoboCoP.Implementation;

namespace RoboCoP.Exceptions
{
    /// <summary>
    /// Exception which is thrown when <see cref="EventReceiver{TMessage}"/> failed to continue receiving messages.
    /// </summary>
    public class EventReceiverFailedException: IOException
    {
        public EventReceiverFailedException(IOException exception): base("EventReceiver failed because of exception", exception) {}
    }
}
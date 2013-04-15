using System.Collections.Generic;
using RoboCoP.Messages;

namespace RoboCoP.Internal
{
    /// <summary>
    /// Basic functionality for <see cref="Message"/>s which can be transferred via switch.
    /// </summary>
    public abstract class AddressableMessage: Message
    {
        protected AddressableMessage(IDictionary<string, string> header, byte[] body):
            base(header, body) {}

        /// <summary>
        /// Name of the message receiver.
        /// </summary>
        public string To
        {
            get { return Fields["to"]; }
        }

        /// <summary>
        /// Name of the message sender.
        /// </summary>
        public string From
        {
            get { return Fields["from"]; }
        }
    }
}
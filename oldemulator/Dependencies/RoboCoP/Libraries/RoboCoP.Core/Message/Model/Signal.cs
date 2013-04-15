using System;
using System.Collections.Generic;
using RoboCoP.Internal;

namespace RoboCoP.Messages
{
    /// <summary>
    /// Represents an signal RoboCoP's messages.
    /// </summary>
    public class Signal: AddressableMessage
    {
        public Signal(IDictionary<string, string> header, byte[] body): base(header, body)
        {
            if(Type != MessageType.Signal)
                throw new ArgumentException("Message is not a signal");
        }

        /// <summary>
        /// Name of the <see cref="Signal"/>
        /// </summary>
        public string Name
        {
            get { return Fields["cmd-name"]; }
        }

        /// <summary>
        /// Id of the current <see cref="Signal"/>.
        /// </summary>
        public string PackId
        {
            get { return Fields["pack-id"]; }
        }
    }
}
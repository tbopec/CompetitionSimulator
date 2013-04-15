using System;
using System.Collections.Generic;

namespace RoboCoP.Messages
{
    /// <summary>
    /// Represent a data RoboCoP's message.
    /// </summary>
    public class DataMessage: Message
    {
        public DataMessage(IDictionary<string, string> fields, byte[] body): base(fields, body)
        {
            if(Type != MessageType.Data)
                throw new ArgumentException("Message is not data message.");
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RoboCoP.Exceptions;
using RoboCoP.Helpers;
using RoboCoP.Messages;

namespace RoboCoP.Internal
{
    /// <summary>
    /// Contains methods, that be use, when  any message factory's types make a message. 
    /// </summary>
    public abstract class BaseMessageFactory
    {

        protected static IDictionary<string, string> PrepareHeaders(IDictionary<string, string> dictionary, MessageType type,
                                                                  int bodyLength)
        {
            return (dictionary ?? new Dictionary<string, string>())
                .Append("timestamp", DateTime.Now.ToString("HH:mm:ss.ffff"))
                .Append("type", type.ToString().ToLower())
                .Append("length", bodyLength.ToString());
        }

        protected static T CheckMessage<T>(T message) where T : Message
        {
            string error;
            if (!MessageValidator.CheckMessage(message, out error))
                throw new MessageValidationFailed(error);
            return message;
        }

    }

}

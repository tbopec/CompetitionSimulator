using System;
using System.Collections.Generic;
using System.Text;

namespace RoboCoP.Messages
{
    /// <summary>
    /// Representation of RoboCoP's message.
    /// </summary>
    public abstract class Message
    {
        private MessageType? messageType;

        protected Message(IDictionary<string, string> fields, byte[] body)
        {
            if(fields == null)
                throw new ArgumentNullException("fields");
            if(body == null)
                throw new ArgumentNullException("body");
            Body = body;
            Fields = fields;
        }

        /// <summary>
        /// Data stored in the <see cref="Message"/>.
        /// </summary>
        /// <remarks>Never change content of this array.</remarks>
        public byte[] Body { get; private set; }

        /// <summary>
        /// <see cref="IDictionary{TKey,TValue}"/> representation of the header of the <see cref="Message"/>.
        /// </summary>
        public IDictionary<string, string> Fields { get; private set; }

        /// <summary>
        /// Text representation of messages body in utf-8.
        /// </summary>
        public string TextBody
        {
            get { return Encoding.UTF8.GetString(Body); }
        }

        /// <summary>
        /// <see cref="MessageType"/> of the <see cref="Message"/>.
        /// </summary>
        public MessageType Type
        {
            get
            {
                if(messageType == null)
                    messageType = (MessageType) Enum.Parse(typeof(MessageType), Fields["type"], true);
                return messageType.Value;
            }
        }
    }
}
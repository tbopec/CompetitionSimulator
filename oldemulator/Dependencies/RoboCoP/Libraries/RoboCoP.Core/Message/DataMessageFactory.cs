using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using RoboCoP.Exceptions;
using RoboCoP.Internal;
using RoboCoP.Messages;

namespace RoboCoP
{

    /// <summary>
    /// Class-helper which provide lots of methods for creating <see cref="Messages.DataMessage"/>s.
    /// </summary>
    public class DataMessageFactory : BaseMessageFactory
    {

        /// <summary>
        /// Create <see cref="Messages.DataMessage"/> with <see cref="Message.TextBody"/>==<paramref name="body"/>.
        /// </summary>
        public DataMessage DataMessage(string body, IDictionary<string, string> additionalHeaders = null)
        {
            if (string.IsNullOrEmpty(body))
                throw new ArgumentNullException("body");
            return DataMessage(Encoding.UTF8.GetBytes(body), additionalHeaders);
        }

        /// <summary>
        /// Create <see cref="Messages.DataMessage"/> with <see cref="Message.Body"/>==<paramref name="body"/>.
        /// </summary>
        // ReSharper disable MemberCanBeMadeStatic.Global
        public DataMessage DataMessage(byte[] body, IDictionary<string, string> additionalHeaders = null)
        // ReSharper restore MemberCanBeMadeStatic.Global
        {
            if (body == null)
                throw new ArgumentNullException("body");
            return CheckMessage(new DataMessage(PrepareHeaders(additionalHeaders, MessageType.Data, body.Length), body));
        }


    }
}

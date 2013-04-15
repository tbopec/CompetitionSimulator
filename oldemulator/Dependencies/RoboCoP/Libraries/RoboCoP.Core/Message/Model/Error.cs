using System;
using System.Collections.Generic;
using RoboCoP.Internal;

namespace RoboCoP.Messages
{
    /// <summary>
    /// Represents an RoboCoP's error message.
    /// </summary>
    public class Error: AddressableMessage
    {
        public Error(IDictionary<string, string> header, byte[] body)
            : base(header, body)
        {
            if(Type != MessageType.Error)
                throw new ArgumentException("Message is not error");
        }

        /// <summary>
        /// Returns <see cref="Signal.PackId"/> of the <see cref="Signal"/> which caused the <see cref="Error"/>
        /// if error is targeted, null otherwise.
        /// </summary>
        public string ErrorCauseId
        {
            get
            {
                string result;
                Fields.TryGetValue("pack-id", out result);
                return result;
            }
        }

        public string ErrorType
        {
            get { return Fields["error-type"]; }
        }
    }
}
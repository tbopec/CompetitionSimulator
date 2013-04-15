using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using RoboCoP.Exceptions;
using RoboCoP.Helpers;
using RoboCoP.Internal;
using RoboCoP.Messages;

namespace RoboCoP
{
    /// <summary>
    /// Class-helper which provide lots of methods for creating <see cref="Messages.DataMessage"/>s, <see cref="Messages.Signal"/>s
    /// and <see cref="Error"/>s.
    /// </summary>
    public class MessageFactory : DataMessageFactory
    {
        private readonly string serviceName;
        private readonly int uid;
        private int packId;

        public MessageFactory(string serviceName)
        {
            if(string.IsNullOrEmpty(serviceName))
                throw new ArgumentNullException("serviceName");
            this.serviceName = serviceName;
            uid = new Random().Next(0, 999);
        }

        #region Private members

        private IDictionary<string, string> PrepareSignal(IDictionary<string, string> dictionary, string to, string signalName,
                                                          int bodyLength)
        {
            return PrepareHeaders(dictionary, MessageType.Signal, bodyLength)
                .Append("from", serviceName)
                .Append("to", to)
                .Append("cmd-name", signalName)
                .Append("pack-id", GetPackId());
        }

        private string GetPackId()
        {
            int id = Interlocked.Increment(ref packId);
            return string.Format("{0}_{1}_{2}", serviceName, id, uid);
        }

        private IDictionary<string, string> PrepareError(IDictionary<string, string> dictionary, string to, string errorType,
                                                         int bodyLength)
        {
            return PrepareHeaders(dictionary, MessageType.Error, bodyLength)
                .Append("from", serviceName)
                .Append("to", to)
                .Append("error-type", errorType);
        }


        #endregion

 
        #region Creating Errors

        /// <summary>
        /// Create targeted (such that <see cref="Messages.Signal"/> that cause it is known) <see cref="Error"/>
        /// addressed to the service which sent that causing <see cref="Messages.Signal"/>.
        /// <paramref name="errorCause"/> is a <see cref="Messages.Signal"/> that cause it.
        /// </summary>
        public Error TargetedError(Signal errorCause, string errorType, string errorBody = null,
                                   IDictionary<string, string> additionalHeaders = null)
        {
            if(errorCause == null)
                throw new ArgumentNullException("errorCause");
            if(string.IsNullOrEmpty(errorType))
                throw new ArgumentNullException("errorType");
            byte[] body = Encoding.UTF8.GetBytes(errorBody ?? "");
            return CheckMessage(
                new Error(
                    PrepareError(additionalHeaders, errorCause.From, errorType, body.Length)
                        .Append("pack-id", errorCause.PackId),
                    body));
        }

        /// <summary>
        /// Create untargeted (such that <see cref="Messages.Signal"/> that cause it is unknown) <see cref="Error"/>
        /// addressed to the mailslot with name <paramref name="to"/>.
        /// </summary>
        public Error UntargetedError(string to, string errorType, string errorBody = null,
                                     IDictionary<string, string> additionalHeaders = null)
        {
            if(string.IsNullOrEmpty(to))
                throw new ArgumentNullException("to");
            if(string.IsNullOrEmpty(errorType))
                throw new ArgumentNullException("errorType");
            byte[] body = Encoding.UTF8.GetBytes(errorBody ?? "");
            return CheckMessage(
                new Error(
                    PrepareError(additionalHeaders, to, errorType, body.Length),
                    body));
        }

        /// <summary>
        /// Create untargeted (such that <see cref="Messages.Signal"/> that cause it is unknown) <see cref="Error"/>
        /// addressed to all services in the RoboCoP's network.
        /// Useful for critical errors.
        /// </summary>
        public Error BroadcastError(string errorType, string errorBody = null,
                                    IDictionary<string, string> additionalHeaders = null)
        {
            return UntargetedError("all", errorType, errorBody, additionalHeaders);
        }

        #endregion

        #region Creating Signals

        /// <summary>
        /// Create <see cref="Messages.Signal"/> with <see cref="Messages.Signal.Name"/>==<paramref name="signalName"/>
        /// addressed to mailslot with name <paramref name="to"/>.
        /// </summary>
        public Signal Signal(string to, string signalName, string body = null,
                             IDictionary<string, string> additionalHeaders = null)
        {
            return Signal(to, signalName, Encoding.UTF8.GetBytes(body ?? ""), additionalHeaders);
        }

        /// <summary>
        /// Create <see cref="Messages.Signal"/> with <see cref="Messages.Signal.Name"/>==<paramref name="signalName"/>
        /// addressed to mailslot with name <paramref name="to"/>.
        /// </summary>
        public Signal Signal(string to, string signalName, byte[] body, IDictionary<string, string> additionalHeaders = null)
        {
            if(body == null)
                throw new ArgumentNullException("body");
            if(string.IsNullOrEmpty(to))
                throw new ArgumentNullException("to");
            if(string.IsNullOrEmpty(signalName))
                throw new ArgumentNullException("signalName");
            return CheckMessage(
                new Signal(
                    PrepareSignal(additionalHeaders, to, signalName, body.Length),
                    body));
        }

        #endregion
    }
}
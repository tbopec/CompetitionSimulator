using System;
using System.Linq;

using RoboCoP.Messages;

namespace RoboCoP.Internal
{
    /// <summary>
    /// Implementation of <see cref="ISwitchNegotiator"/> interface.
    /// </summary>
    public class SwitchNegotiator: ISwitchNegotiator
    {
        private readonly MessageFactory messageFactory;
        private readonly ISender<Signal> senderToSwitch;

        public SwitchNegotiator(ISender<Signal> senderToSwitch, MessageFactory messageFactory)
        {
            if(senderToSwitch == null)
                throw new ArgumentNullException("senderToSwitch");
            if(messageFactory == null)
                throw new ArgumentNullException("messageFactory");
            this.senderToSwitch = senderToSwitch;
            this.messageFactory = messageFactory;
        }

        #region ISwitchNegotiator Members

        ///<inheritdoc/>
        public void RegisterService()
        {
            senderToSwitch.Send(messageFactory.Signal("switch", "hello", new byte[]{})).Single();
        }

        ///<inheritdoc/>
        public void UnregistrateService()
        {
            senderToSwitch.Send(messageFactory.Signal("switch", "goodbye")).Single();
        }

        ///<inheritdoc/>
        public void SubscribeToMailslot(string mailslotName)
        {
            if(string.IsNullOrEmpty(mailslotName))
                throw new ArgumentNullException("mailslotName");
            senderToSwitch.Send(messageFactory.Signal("switch", "subscribe", mailslotName)).Single();
        }

        #endregion
    }
}
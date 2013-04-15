using System;
using System.Collections.Generic;
using RoboCoP;
using RoboCoP.Internal;
using RoboCoP.Messages;

namespace Switch.Core
{
    public class SwitchService: MailboxManager
    {
        public const string BroadcastMailbox = "all";
        public const string SwitchName = "switch";
        public readonly IDictionary<ISafeConnection, string> ServicesName = new Dictionary<ISafeConnection, string>();

        public SwitchService(IConnectionsManager connectionsManager): base(connectionsManager)
        {
            ConnectionRemoved += (message, safeConnection) => ServicesName.Remove(safeConnection);
        }

        protected override void ReceiveAddressableMessage(AddressableMessage addressableMessage, ISafeConnection safeConnection)
        {
            MessageCaught(addressableMessage, safeConnection);
            if(addressableMessage.To == SwitchName)
                ReceiveMessageToMe(addressableMessage, safeConnection);
            else
                SendMessageToMailbox(addressableMessage.To, addressableMessage);
        }

        private void ReceiveMessageToMe(AddressableMessage addressableMessage, ISafeConnection safeConnection)
        {
            if(addressableMessage is Error)
                ErrorReceived((Error) addressableMessage);
            else if(addressableMessage is Signal)
                ReceiveSignalToMe((Signal) addressableMessage, safeConnection);
            else
                throw new ArgumentOutOfRangeException();
        }

        private void ReceiveSignalToMe(Signal signal, ISafeConnection safeConnection)
        {
            bool unknownSignal = false;
            switch(signal.Name) {
            case "hello":
                lock(ServicesName)
                    ServicesName[safeConnection] = signal.From;
                SubscribeTo(signal.From, safeConnection);
                SubscribeTo(BroadcastMailbox, safeConnection);
                break;
            case "subscribe":
                SubscribeTo(signal.TextBody, safeConnection);
                break;
            default:
                unknownSignal = true;
                break;
            }
            if(unknownSignal)
                UnknownSignalReceived(signal);
            else
                KnownSignalReceived(signal);
        }

        #region Events

        public event Action<Error> ErrorReceived = delegate { };
        public event Action<Signal> KnownSignalReceived = delegate { };
        public event Action<Signal> UnknownSignalReceived = delegate { };
        public event Action<AddressableMessage, ISafeConnection> MessageCaught = delegate { };

        #endregion
    }
}
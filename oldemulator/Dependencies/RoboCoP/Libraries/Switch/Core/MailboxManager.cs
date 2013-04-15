using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using RoboCoP;
using RoboCoP.Helpers;
using RoboCoP.Implementation;
using RoboCoP.Internal;

namespace Switch.Core
{
    public abstract class MailboxManager: ServicesManager
    {
        private readonly ConcurrentDictionary<string, ConcurentLinkedList<ISafeConnection>> mailboxes
            = new ConcurrentDictionary<string, ConcurentLinkedList<ISafeConnection>>();

        public readonly ConcurrentDictionary<ISafeConnection, ISender<AddressableMessage>> senders
            = new ConcurrentDictionary<ISafeConnection, ISender<AddressableMessage>>();

        protected MailboxManager(IConnectionsManager connectionsManager): base(connectionsManager)
        {
            ConnectionAdded += AddSender;
            ConnectionRemoved += RemoveAndUnsubscribeSender;
        }

        private void AddSender(ISafeConnection safeConnection)
        {
            senders.Add(safeConnection, new SingleSender<AddressableMessage>(safeConnection.AsStable()));
        }

        private void RemoveAndUnsubscribeSender(string message, ISafeConnection connection)
        {
            mailboxes.ForEach(pair => pair.Value.Remove(connection));
            ISender<AddressableMessage> sender;
            senders.TryRemove(connection, out sender);
        }

        #region Protected interface

        protected void SubscribeTo(string mailbox, ISafeConnection safeConnection)
        {
            ICollection<ISafeConnection> value =
                mailboxes.GetOrAdd(mailbox, _ => new ConcurentLinkedList<ISafeConnection>());
            value.Add(safeConnection);
            ServiceSubscribed(mailbox, safeConnection);
        }

        protected void SendMessageToMailbox(string mailbox, AddressableMessage addressableMessage)
        {
            ConcurentLinkedList<ISafeConnection> mailboxSubscriber;
            if(mailboxes.TryGetValue(mailbox, out mailboxSubscriber) && mailboxSubscriber.Any())
                mailboxSubscriber
                    .ForEach(x => senders[x]
                                 .Send(addressableMessage)
                                 .Subscribe(u => MessageSent(addressableMessage, x)));
            else
                MessageLost(addressableMessage);
        }

        #endregion

        #region Disposing

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            mailboxes.Clear();
            senders.Clear();
        }

        #endregion

        #region Events

        public event Action<string, ISafeConnection> ServiceSubscribed = delegate { };
        public event Action<AddressableMessage> MessageLost = delegate { };
        public event Action<AddressableMessage, ISafeConnection> MessageSent = delegate { };

        #endregion
    }
}
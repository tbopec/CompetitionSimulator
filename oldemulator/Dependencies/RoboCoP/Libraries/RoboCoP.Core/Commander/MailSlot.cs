using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

using System.Runtime.Serialization;

using System.IO;

using RoboCoP.Internal;
using RoboCoP.Helpers;
using RoboCoP.Messages;

namespace RoboCoP.Implementation
{
    /// <summary>
    /// Thread-safe implementation of <see cref="IMailSlot"/>. Also contain and <see cref="ReceiveSignal(string)"/>
    /// which are useful for using of the class form the <see cref="Commander"/>.
    /// </summary>
    public class MailSlot: IMailSlot
    {

        private readonly ISender<AddressableMessage> sender;

        private readonly MessageFactory factory;

        private readonly ConcurrentDictionary<string, ConcurentLinkedList<Action<Signal>>> listeners =
            new ConcurrentDictionary<string, ConcurentLinkedList<Action<Signal>>>();

        #region Implementation of IMailSlot

        private readonly List<Action<Signal>> onSignalReceived = new List<Action<Signal>>();

        /// <inheritdoc/>
        public string Name { get; private set; }

        /// <inheritdoc/>
        public void SendSignal(string signalName)
        {
            sender.SendSync(factory.Signal(Name, signalName));
        }

        /// <inheritdoc/>
        public void SendSignal(string signalName, string data)
        {
            sender.SendSync(factory.Signal(Name, signalName, data));
        }

        /// <inheritdoc/>
        public string ReceiveSignal(string signalName)
        {
            Signal receivedSignal = null;
            var ewh = new EventWaitHandle(false, EventResetMode.ManualReset);
            Action<Signal> listener = signal => {
                                          receivedSignal = signal;
                                          ewh.Set();
                                      };
            if (listeners.ContainsKey(signalName))
                listeners[signalName].Add(listener);
            else
                listeners.Add(signalName, new ConcurentLinkedList<Action<Signal>> {listener});

            ewh.WaitOne();
            
            listeners[signalName].Remove(listener);
            
            string value = receivedSignal.TextBody;

            return value;
        }

        /// <inheritdoc/>
        public void AddSignalListener(string signalName, Action onReceive)
        {
            if(signalName == null)
                throw new ArgumentNullException("signalName");
            if (onReceive == null)
                throw new ArgumentNullException("onReceive");

            listeners.GetOrAdd(signalName, _ => new ConcurentLinkedList<Action<Signal>>()).Add(_ => onReceive());
        }

        /// <inheritdoc/>
        public void AddSignalListener(string signalName, Action<string> onReceive)
        {
            if (signalName == null)
                throw new ArgumentNullException("signalName");
            if (onReceive == null)
                throw new ArgumentNullException("onReceive");

            listeners.GetOrAdd(signalName, _ => new ConcurentLinkedList<Action<Signal>>())
                .Add(signal =>
                    {
                        string value = signal.TextBody;
                        onReceive(value);
                    });
        }

        #endregion

        public MailSlot(string name, ISender<AddressableMessage> signalSender, MessageFactory messageFactory)
        {
            if(string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            factory = messageFactory;
            Name = name;
            sender = signalSender;
        }

        /// <summary>
        /// Inform all listeners added via <see cref="AddSignalListener"/> or via <see cref="OnSignalReceived"/>
        /// that <paramref name="signal"/> was received.
        /// Returns true if someone has caught the signal.
        /// </summary>
        public bool ReceiveSignal(Signal signal)
        {
            bool catched = false;

            ConcurentLinkedList<Action<Signal>> signalListeners;
            if(listeners.TryGetValue(signal.Name, out signalListeners)) {
                signalListeners.ForEach(l => l(signal));
                catched = true;
            }

            if(onSignalReceived.Count > 0) {
                onSignalReceived.ForEach(x => x(signal));
                catched = true;
            }

            return catched;
        }

    }
}
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RoboCoP.Exceptions;
using RoboCoP.Internal;
using RoboCoP.Messages;
using RoboCoP.Helpers;

namespace RoboCoP.Implementation
{
    /// <summary>
    /// Thread-safe implementation of <see cref="ICommander"/>.
    /// This implementation is "switch-safe" - it always unregister itself from switch
    /// before shutting down.
    /// </summary>
    public class Commander: ICommander
    {
        
        private const string BroadcastMailSlotName = "all";

        private readonly ISender<AddressableMessage> sender;
        
        private readonly IDisposable eventReceiver;
        
        private readonly ConcurrentDictionary<string, MailSlot> mailSlots = new ConcurrentDictionary<string, MailSlot>();
        
        private readonly string serviceName;
        
        private readonly ISwitchNegotiator switchNegotiator;

        private readonly MessageFactory messageFactory;
        
        private bool disposed;

        public Commander(string serviceName, ISender<AddressableMessage> sender, IReceiver<AddressableMessage> receiver, ISwitchNegotiator switchNegotiator)
        {
            if(serviceName == null)
                throw new ArgumentNullException("serviceName");
            if(sender == null)
                throw new ArgumentNullException("sender");
            if(receiver == null)
                throw new ArgumentNullException("receiver");
            if(switchNegotiator == null)
                throw new ArgumentNullException("switchNegotiator");

            this.serviceName = serviceName;
            this.sender = sender;
            this.switchNegotiator = switchNegotiator;
            this.messageFactory = new MessageFactory(serviceName);

            eventReceiver = InitEventReceiver(receiver);
            InitMailSlots();
        }

        #region Initialization

        private void InitMailSlots()
        {
            var personalMailSlot = new MailSlot(serviceName, sender, messageFactory);
            mailSlots.Add(serviceName, personalMailSlot);
            mailSlots.Add(BroadcastMailSlotName, personalMailSlot);
            switchNegotiator.RegisterService();
        }

        private IDisposable InitEventReceiver(IReceiver<AddressableMessage> receiver)
        {
            return receiver.ReceiveAll(SignalOrErrorReceived, exception => { throw new LostIOException((IOException) exception); });
        }

        #endregion

        #region Receiving signals and errors from duplex

        private void SignalOrErrorReceived(AddressableMessage signalOrError)
        {
            string mailSlotName = signalOrError.To;
            MailSlot mailSlot;
            mailSlots.TryGetValue(mailSlotName, out mailSlot);
            if(signalOrError is Signal)
                SignalReceived((Signal) signalOrError, mailSlot);
            else if (signalOrError is Error)
                OnErrorReceived(signalOrError as Error);
            else
                throw new ArgumentOutOfRangeException("signalOrError");
        }

 
        private void SignalReceived(Signal signal, MailSlot mailSlot)
        {
            if((mailSlot == null || !mailSlot.ReceiveSignal(signal))&&!ReferenceEquals(OnUncatchedSignal, null))
                OnUncatchedSignal(signal);
        }

        #endregion

        #region Disposing

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="Commander"/> (registration at the switch) and optionally releases the managed resources.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if(disposed)
                return;
            disposed = true;
            switchNegotiator.UnregistrateService();
            if(disposing) {
                eventReceiver.Dispose();
                sender.Dispose();
                mailSlots.Clear();
            }
        }

        ~Commander()
        {
            Dispose(false);
        }

        #endregion

        #region Implementation of IEnumerable

        /// <inheritdoc/>
        public IEnumerator<IMailSlot> GetEnumerator()
        {
            if(disposed)
                throw new ObjectDisposedException("Commander");
            return mailSlots.Values.OfType<IMailSlot>().GetEnumerator();
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Implementation of ICommander

        /// <inheritdoc/>
        public IMailSlot this[string name]
        {
            get
            {
                if(disposed)
                    throw new ObjectDisposedException("Commander");
                return mailSlots.GetOrAdd(name, delegate {
                                                    switchNegotiator.SubscribeToMailslot(name);
                                                    return new MailSlot(name, sender, messageFactory);
                                                });
            }
        }

        /// <inheritdoc/>
        public IMailSlot PersonalMailSlot
        {
            get
            {
                if(disposed)
                    throw new ObjectDisposedException("Commander");
                return mailSlots[serviceName];
            }
        }

        /// <inheritdoc/>
        public void RaiseError(Error error)
        {
            if(disposed)
                throw new ObjectDisposedException("Commander");
            sender.Send(error);
        }

        /// <inheritdoc/>
        public event Action<Error> OnErrorReceived;

        /// <inheritdoc/>
        public event Action<Signal> OnUncatchedSignal;

        #endregion
    }
}
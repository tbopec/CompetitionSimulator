using System;
using System.Collections.Generic;
using RoboCoP.Messages;

namespace RoboCoP
{
    /// <summary>
    /// Interface for communicating with RoboCoP network via it's signal interface.
    /// Provides method for sending signals, errors and subscribing to mailslots.
    /// </summary>
    public interface ICommander: IEnumerable<IMailSlot>
    {
        /// <summary>
        /// Get the <see cref="IMailSlot"/> interface to mailslot with name <paramref name="name"/>.
        /// Subscribe to it if subscription has not been created yet.
        /// </summary>
        IMailSlot this[string name] { get; }

        /// <summary>
        /// Get the <see cref="IMailSlot"/> interface to personal (which name is equal to service's name)
        /// and broadcast (which name is 'all') mailslots for which service is subscribed by default.
        /// </summary>
        IMailSlot PersonalMailSlot { get; }

        /// <summary>
        /// Send <paramref name="error"/> which should be untargeted.
        /// </summary>
        void RaiseError(Error error);

        /// <summary>
        /// Raised each time when an <see cref="Error"/> message received from any mailslot.
        /// </summary>
        event Action<Error> OnErrorReceived;

        /// <summary>
        /// Raised each time when an <see cref="Signal"/> message received from some mailslot
        /// and was not caught by synchronous <see cref="IMailSlot.ReceiveSignal"/> method,
        /// listener added via <see cref="IMailSlot.AddSignalListener"/> and
        /// <see cref="IMailSlot.OnSignalReceived"/> listeners.
        /// </summary>
        event Action<Signal> OnUncatchedSignal;
    }
}
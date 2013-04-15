using System;
using RoboCoP.Messages;

namespace RoboCoP
{
    /// <summary>
    /// Interface for sending <see cref="Message"/>s of type <typeparamref name="TMessage"/> to RoboCoP's network.
    /// </summary>
    public interface ISender<in TMessage>: IDisposable
        where TMessage: Message
    {
        /// <summary>
        /// Initialize an asynchronous sending of the <paramref name="message"/> as an <see cref="IObservable{T}"/> operation.
        /// Start the operation only when <see cref="IObservable{T}"/> will be subscribed.
        /// Provide a push-based notification of completion or error via that <see cref="IObservable{T}"/>.
        /// </summary>
        /// <remarks>
        /// Operation won't start until someone subscribes on <see cref="IObservable{T}"/>.
        /// Each next subscription on <see cref="IObservable{T}"/> will start operation again.
        /// </remarks>
        IObservable<Unit> Send(TMessage message);
    }
}
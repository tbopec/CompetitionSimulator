﻿using System;
using RoboCoP.Messages;

namespace RoboCoP
{
    /// <summary>
    /// Interface for receiving <see cref="Message"/>s of type <typeparamref name="TMessage"/> from RoboCoP's network.
    /// </summary>
    public interface IReceiver<out TMessage>: IDisposable
        where TMessage: Message
    {
        /// <summary>
        /// Initialize an asynchronous receiving of one package of the <typeparamref name="TMessage"/> as an <see cref="IObservable{T}"/> operation.
        /// Start the operation only when <see cref="IObservable{T}"/> will be subscribed.
        /// Provide a push-based notification of receiving (including received <typeparamref name="TMessage"/>) or error via that <see cref="IObservable{T}"/>.
        /// </summary>
        /// <remarks>
        /// Operation won't start until someone subscribes on <see cref="IObservable{T}"/>.
        /// Each next subscription on <see cref="IObservable{T}"/> will start operation again.
        /// </remarks>
        IObservable<TMessage> Receive();
    }
}
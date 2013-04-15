using System;
using System.Linq;
using RoboCoP.Messages;

namespace RoboCoP
{
    public static class SenderExtension
    {
        /// <summary>
        /// Directly synchronously sends the <paramref name="message"/> via <paramref name="sender"/>.
        /// </summary>
        /// <remarks>
        /// This method wraps around <see cref="ISender{TMessage}.Send"/> and encapsulate all work with <see cref="IObservable{T}"/>.
        /// </remarks>
        public static void SendSync<TMessage>(this ISender<TMessage> sender, TMessage message) where TMessage: Message
        {
            sender.Send(message).Single();
        }

        /// <summary>
        /// Directly start async sending the <paramref name="message"/> via <paramref name="sender"/>.
        /// </summary>
        /// <remarks>
        /// This method wraps around <see cref="ISender{TMessage}.Send"/> and encapsulate all work with <see cref="IObservable{T}"/>.
        /// </remarks>
        public static void SendAsync<TMessage>(this ISender<TMessage> sender, TMessage message) where TMessage: Message
        {
            sender.Send(message).Subscribe();
        }

        /// <summary>
        /// Directly start async sending the <paramref name="message"/> via <paramref name="sender"/>.
        /// Call <paramref name="onSent"/> when done.
        /// </summary>
        /// <remarks>
        /// This method wraps around <see cref="ISender{TMessage}.Send"/> and encapsulate all work with <see cref="IObservable{T}"/>.
        /// </remarks>
        public static void SendAsync<TMessage>(this ISender<TMessage> sender, TMessage message, Action onSent)
            where TMessage: Message
        {
            sender.Send(message).Subscribe(_ => onSent());
        }
    }
}
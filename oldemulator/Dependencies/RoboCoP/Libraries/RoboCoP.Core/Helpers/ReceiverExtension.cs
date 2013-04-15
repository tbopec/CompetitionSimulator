using System;
using System.Linq;
using System.Threading.Tasks;
using RoboCoP.Implementation;
using RoboCoP.Messages;

namespace RoboCoP
{
    public static class ReceiverExtension
    {
        /// <summary>
        /// Directly synchronously receiving a <typeparamref name="TMessage"/> from <paramref name="receiver"/>.
        /// </summary>
        /// <remarks>
        /// This method wraps around <see cref="IReceiver{TMessage}.Receive"/> and encapsulate all work with <see cref="IObservable{T}"/>.
        /// </remarks>
        public static TMessage ReceiveSync<TMessage>(this IReceiver<TMessage> receiver) where TMessage: Message
        {
            return receiver.Receive().Single();
        }

        /// <summary>
        /// Directly starts async receiving a <typeparamref name="TMessage"/> from <paramref name="receiver"/>.
        /// Call the <paramref name="onReceived"/> when done.
        /// </summary>
        /// <remarks>
        /// This method wraps around <see cref="IReceiver{TMessage}.Receive"/> and encapsulate all work with <see cref="IObservable{T}"/>.
        /// </remarks>
        public static void ReceiveAsync<TMessage>(this IReceiver<TMessage> receiver, Action<TMessage> onReceived)
            where TMessage: Message
        {
            receiver.Receive().Subscribe(onReceived);
        }

        /// <summary>
        /// Directly starts async receiving a <typeparamref name="TMessage"/> from <paramref name="receiver"/>.
        /// Returns a <see cref="Task{TResult}"/> which represents async operation.
        /// </summary>
        /// <remarks>
        /// This method wraps around <see cref="IReceiver{TMessage}.Receive"/> and encapsulate all work with <see cref="IObservable{T}"/>.
        /// </remarks>
        public static Task<TMessage> ReceiveAsync<TMessage>(this IReceiver<TMessage> receiver) where TMessage: Message
        {
            var taskCompletionSource = new TaskCompletionSource<TMessage>();
            receiver.Receive().Subscribe(taskCompletionSource.SetResult, taskCompletionSource.SetException);
            return taskCompletionSource.Task;
        }

        /// <summary>
        /// Starts async receiving of all <typeparamref name="TMessage"/>s from <paramref name="receiver"/>.
        /// Call the <paramref name="onReceived"/> each time when <typeparamref name="TMessage"/> received.
        /// Call the <paramref name="onError"/> each time when error occurs.
        /// Returns the <see cref="IDisposable"/> calling <see cref="IDisposable.Dispose"/> on which will cancel receiving.
        /// </summary>
        /// <remarks>
        /// This method wraps around <see cref="IReceiver{TMessage}.Receive"/> and encapsulate all work with <see cref="IObservable{T}"/>.
        /// </remarks>
        public static IDisposable ReceiveAll<TMessage>(this IReceiver<TMessage> receiver, Action<TMessage> onReceived,
                                                       Action<Exception> onError = null) where TMessage: Message
        {
            if(onError == null)
                onError = delegate { };
            return ReceiveAllObservable(receiver).Subscribe(onReceived, onError);
        }

        /// <summary>
        /// Starts async receiving of all <typeparamref name="TMessage"/>s from <paramref name="receiver"/>.
        /// Provide event-driven interface for handling received messages via <see cref="IEventReceiver{TMessage}"/>.
        /// Call <see cref="IDisposable.Dispose"/> on returned value for canceling receiving.
        /// </summary>
        /// <remarks>
        /// This method wraps around <see cref="IReceiver{TMessage}.Receive"/> and encapsulate all work with <see cref="IObservable{T}"/>.
        /// </remarks>
        public static IEventReceiver<TMessage> ReceiveAll<TMessage>(this IReceiver<TMessage> receiver) where TMessage: Message
        {
            return new EventReceiver<TMessage>(ReceiveAllObservable(receiver));
        }

        private static IObservable<TMessage> ReceiveAllObservable<TMessage>(IReceiver<TMessage> receiver) where TMessage: Message
        {
            return Observable.Defer(receiver.Receive).Repeat().AsObservable();
        }
    }
}
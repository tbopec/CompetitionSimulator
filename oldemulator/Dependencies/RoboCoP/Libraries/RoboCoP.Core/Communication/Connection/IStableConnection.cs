using System;

namespace RoboCoP.Internal
{
    /// <summary>
    /// Protocol-independent interface for sending and receiving packages of raw data.
    /// It's methods notifies only when all operations in them are completed successfully.
    /// </summary>
    public interface IStableConnection: IDisposable
    {
        /// <summary>
        /// Max length of one piece of data which may be send via <see cref="Send"/>. Returns -1 if unlimited.
        /// </summary>
        int MaxSendLength { get; }

        /// <summary>
        /// Gets internal <see cref="IConnection"/>.
        /// </summary>
        /// <exception cref="NotSupportedException">This property is not supported.</exception>
        IConnection UnsafeConnection { get; }

        /// <summary>
        /// Initialize an asynchronous receiving of one package of raw data as an <see cref="IObservable{T}"/> operation.
        /// Start the operation only when <see cref="IObservable{T}"/> will be subscribed.
        /// Provide a push-based notification of receiving (including received data) via that <see cref="IObservable{T}"/>.
        /// In case of errors - repeat operation and notify only when succeed.
        /// </summary>
        /// <remarks>
        /// Operation won't start until someone subscribes on <see cref="IObservable{T}"/>.
        /// Each next subscription on <see cref="IObservable{T}"/> will start operation again.
        /// </remarks>
        IObservable<byte[]> Receive();

        /// <summary>
        /// Initialize an asynchronous sending of one package of raw data as an <see cref="IObservable{T}"/> operation.
        /// Do not split that package into pieces.
        /// Start the operation only when <see cref="IObservable{T}"/> will be subscribed.
        /// Provide a push-based notification of completion via that <see cref="IObservable{T}"/>.
        /// In case of errors - repeat operation and notify only when succeed.
        /// </summary>
        /// <exception cref="ArgumentException"><paramref name="data"/> is null, empty, or it's length is greater then <see cref="MaxSendLength"/>.</exception>
        /// <remarks>
        /// Operation won't start until someone subscribes on <see cref="IObservable{T}"/>.
        /// Each next subscription on <see cref="IObservable{T}"/> will start operation again.
        /// </remarks>
        IObservable<Unit> Send(byte[] data);
    }
}
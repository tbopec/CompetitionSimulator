using System;

using RoboCoP.Messages;

namespace RoboCoP
{
    /// <summary>
    /// Interface for receiving and sending values with <see cref="Signal"/>s with this mailslot.
    /// </summary>
    public interface IMailSlot
    {
        /// <summary>
        /// Gets the name of the mailslot.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Send a signal, that has a specified <paramref name="signalName"/> with empty body.
        /// </summary>
        void SendSignal(string signalName);

        /// <summary>
        /// Send a signal, that contains <paramref name="data"/>
        /// </summary>
        void SendSignal(string signalName, string data);

        /// <summary>
        /// Synchronously receive a string data,
        /// with a signal that has <see cref="Signal.Name"/>==<paramref name="signalName"/>.
        /// </summary>
        string ReceiveSignal(string signalName);

        /// <summary>
        /// Add listener <param name="onReceive"/> which is invoked each time when <see cref="Signal"/>
        /// with  <see cref="Signal.Name"/>=<paramref name="signalName"/> received.
        /// If error has ocurrs during received, he ignored.
        /// </summary>
        void AddSignalListener(string signalName, Action onReceive);

        /// <summary>
        /// Add listener <param name="onReceive"/> which is invoked each time when a string data
        /// with a signal that has  <see cref="Signal.Name"/>=<paramref name="signalName"/> received.
        /// If error has ocurrs during received, he ignored.
        /// </summary>
        void AddSignalListener(string signalName, Action<string> onReceive);
   }
}
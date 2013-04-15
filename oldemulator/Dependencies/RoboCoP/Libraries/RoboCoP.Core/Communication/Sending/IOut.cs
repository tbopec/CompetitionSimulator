using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RoboCoP
{
    /// <summary>
    /// Interface for sending a data to robocop's  network.
    /// </summary>
    public interface IOut : IDisposable
    {

        /// <summary>
        /// Sends to output the text <paramref name="data"/>.
        /// </summary>
        void SendText(string data);

        /// <summary>
        /// Start async sending the text <paramref name="data"/>.
        /// </summary>
        void SendTextAsync(string data);

        /// <summary>
        /// Start async sending the text <paramref name="data"/>.
        /// Call <paramref name="onSent"/> when done.
        /// </summary>
        void SendTextAsync(string data, Action onSent);

        /// <summary>
        /// Sends to output the binary <paramref name="data"/>.
        /// </summary>
        void SendBinary(byte[] data);

        /// <summary>
        /// Start async sending the binary <paramref name="data"/>.
        /// </summary>
        void SendBinaryAsync(byte[] data);

        /// <summary>
        /// Start async sending the binary <paramref name="data"/>.
        /// Call <paramref name="onSent"/> when done.
        /// </summary>
        void SendBinaryAsync(byte[] data, Action onSent);

    }
}

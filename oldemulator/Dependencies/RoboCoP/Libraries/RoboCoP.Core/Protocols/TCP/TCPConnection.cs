using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;

using System.IO;

using RoboCoP.Internal;

namespace RoboCoP.Protocols.TCP
{
    public class TCPConnection: IConnection
    {
        private readonly NetworkStream stream;
        private readonly int receiveBufferSize;

        public TCPConnection(TcpClient tcpClient)
        {
            if(tcpClient == null)
                throw new ArgumentNullException("tcpClient");
            RemoteEndpoint = new TCPAddress((IPEndPoint) tcpClient.Client.RemoteEndPoint);
            stream = tcpClient.GetStream();
            MaxSendLength = tcpClient.SendBufferSize;
            receiveBufferSize = tcpClient.ReceiveBufferSize;
        }

        #region IConnection Members

        public INetworkAddress RemoteEndpoint { get; private set; }

        public int MaxSendLength { get; private set; }

        public void Dispose()
        {
            stream.Dispose();
        }

        /// <inheritdoc/>
        public IObservable<byte[]> Receive()
        {
            var receivingBuffer = new byte[receiveBufferSize];
            return Observable
                .FromAsyncPattern<byte[], int, int, int>(stream.BeginRead, stream.EndRead)
                .Invoke(receivingBuffer, 0, receiveBufferSize)
                .Select(x => {
                            var result = new byte[x];
                            Array.Copy(receivingBuffer, 0, result, 0, x);
                            return result;
                        });
        }

        public IObservable<Unit> Send(byte[] data)
        {
            if (data == null || data.Length == 0 || data.Length > MaxSendLength)
                throw new ArgumentException("data");
            return Observable
                .FromAsyncPattern<byte[], int, int>(stream.BeginWrite, stream.EndWrite)
                .Invoke(data, 0, data.Length);
        }

        public void EndSend(IAsyncResult asyncResult)
        {
            stream.EndWrite(asyncResult);
        }

        #endregion
    }

}
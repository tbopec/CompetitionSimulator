using System;
using System.Net;
using System.Net.Sockets;
using RoboCoP.Implementation;

namespace RoboCoP.Protocols.TCP
{
    public class TCPConnectionManager: AbstractConnectionManager
    {
        private readonly TcpListener tcpListener;

        public TCPConnectionManager(IPEndPoint localAddress)
        {
            tcpListener = new TcpListener(localAddress);
            tcpListener.Start();
            tcpListener.BeginAcceptTcpClient(ProcessTcpClient, null);
        }

        private void ProcessTcpClient(IAsyncResult ar)
        {
            TcpClient tcpClient = tcpListener.EndAcceptTcpClient(ar);
            AddConnection(new TCPConnection(tcpClient));
            tcpListener.BeginAcceptTcpClient(ProcessTcpClient, null);
        }
    }
}
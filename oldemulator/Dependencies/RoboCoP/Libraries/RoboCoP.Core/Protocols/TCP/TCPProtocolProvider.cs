using System;
using System.IO;
using System.Net.Sockets;
using RoboCoP.Internal;

namespace RoboCoP.Protocols.TCP
{
    public class TCPProtocolProvider: ProtocolProvider<TCPAddress>
    {
        public override Func<IConnection> GetConnectionFunc(TCPAddress addressToConnectTo)
        {
            return () => {
                       var tcpClient = new TcpClient(AddressFamily.InterNetwork);
                       try {
                           tcpClient.Connect(addressToConnectTo.Address);
                       }
                       catch(SocketException e) {
                           throw new IOException("SocketException caught", e);
                       }
                       tcpClient.NoDelay = true;
                       return new TCPConnection(tcpClient);
                   };
        }

        public override IConnectionsManager GetConnectionManager(TCPAddress addressToBindTo)
        {
            return new TCPConnectionManager(addressToBindTo.Address);
        }
    }
}
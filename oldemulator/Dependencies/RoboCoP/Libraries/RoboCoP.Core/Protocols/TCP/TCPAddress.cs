using System.Net;

namespace RoboCoP.Protocols.TCP
{
    /// <summary>
    /// Class which represents an <see cref="INetworkAddress"/> in TCP networks.
    /// </summary>
    public class TCPAddress: INetworkAddress
    {
        public TCPAddress(IPEndPoint address)
        {
            Address = address;
        }

        /// <summary>
        /// <see cref="IPEndPoint"/> of the endpoint.
        /// </summary>
        public IPEndPoint Address { get; private set; }

        #region INetworkAddress Members

        public string ProtocolType
        {
            get { return "TCP"; }
        }

        public override string ToString()
        {
            return string.Format("tcp:{0}:{1}", Address.Address.ToString(), Address.Port);
        }

        #endregion
    }
}
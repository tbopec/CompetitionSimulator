using System.Net;
using AIRLab.Thornado;
using RoboCoP.Protocols.TCP;

namespace RoboCoP.Plus
{
    [PrimaryFormat]
    public class INetworkAddressIO: BasicTypeFormat<INetworkAddress>
    {
        public INetworkAddressIO()
            : base(
                obj => Formats.IPEndPoint.Write(((TCPAddress)obj).Address),
                str => new TCPAddress(AIRLab.Thornado.Formats.IPEndPoint.Parse(str)),
                "Адрес в одном из протоколов, поддерживаемом RoboCoP")
        { }
    }

    [PrimaryFormat]
    public class INetworkAddressIO1 : BasicTypeFormat<TCPAddress>
    {
        public INetworkAddressIO1()
            : base(
                obj => Formats.IPEndPoint.Write(((TCPAddress)obj).Address),
                str => new TCPAddress(AIRLab.Thornado.Formats.IPEndPoint.Parse(str)),
                "Адрес в одном из протоколов, поддерживаемом RoboCoP")
        { }
    }



}

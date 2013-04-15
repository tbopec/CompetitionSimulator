//TODO: а эти форматы вообще кажутся мне подозрительными
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using AIRLab.Mathematics;

namespace AIRLab.Thornado.TypeFormats
{

    [PrimaryFormat]
    public class AngleFormat : BasicTypeFormat<Angle>
    {
        public AngleFormat()
            : base(
                z => z.Grad + "G",
                ParseFunction,
                "Angle") { }

        static Angle ParseFunction(string arg)
        {
            if (arg.Contains('R'))
            {
                arg = arg.Replace("R", "");
                return Angle.FromRad(Formats.Double.Parse(arg));
            }
            arg = arg.Replace("G", "");
            return Angle.FromGrad(Formats.Double.Parse(arg));
        }
    }

    [PrimaryFormat]
    public class IPAddressFormat : BasicTypeFormat<IPAddress>
    {
        public IPAddressFormat()
            : base(
                z => z.ToString(),
                str => System.Net.IPAddress.Parse(str),
                "IP Address")
        { }
    }

    [PrimaryFormat]
    public class IPEndPointFormat : BasicTypeFormat<IPEndPoint>
    {
        public IPEndPointFormat()
            : base(
                z => z.ToString(),
                str =>
                {
                    string[] parts = str.Split(':');
                    if (parts.Length != 2) throw new Exception("");
                    return new IPEndPoint(Formats.IPAddress.Parse(parts[0]), int.Parse(parts[1]));
                },
                "IP End Point"
                )
        { }
    }
}


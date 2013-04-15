using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AIRLab.Thornado;
using AIRLab.Thornado.TypeFormats;

namespace AIRLab.Thornado
{
    public class Formats
    {
        public static readonly IntFormat Int = new IntFormat();
        public static readonly DoubleFormat Double = new DoubleFormat();
        public static readonly IPAddressFormat IPAddress = new IPAddressFormat();
        public static readonly IPEndPointFormat IPEndPoint = new IPEndPointFormat();
        public static readonly AngleFormat Angle = new AngleFormat();
        public static readonly BoolFormat Bool = new BoolFormat();
    }
}

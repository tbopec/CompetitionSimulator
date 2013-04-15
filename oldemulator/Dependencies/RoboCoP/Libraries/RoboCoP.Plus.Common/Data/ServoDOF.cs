using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DCIMAP.Thornado;
using DCIMAP.Thornado.IOs;

namespace RoboCoP.Plus.Common {
    public class ServoDOF {
        [ThornadoField("", typeof(StringIO))]
        public string ServoName { get; set; }

        [ThornadoField("", typeof(IntIO))]
        public int ServoNumber { get; set; }

        [ThornadoField("", typeof(DoubleIO))]
        public double Signal { get; set; }
    }
}

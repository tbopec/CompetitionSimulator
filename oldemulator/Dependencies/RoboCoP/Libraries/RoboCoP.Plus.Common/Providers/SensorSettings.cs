using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RoboCoP.Plus;
using AIRLab.Thornado;

namespace RoboCoP.Common
{
    [Serializable]
    public class SensorSettings : ServiceSettings
    {
        [Thornado("True, if sensor should produce a data by timer ticks")]
        public bool EnableTimer { get; set; }

        [Thornado("A time interval, in seconds, between time ticks")]
        public double TimerInterval { get; set; }
    }
}

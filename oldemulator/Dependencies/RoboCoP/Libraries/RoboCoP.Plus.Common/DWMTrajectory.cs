using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using AIRLab.Mathematics;
using AIRLab.Thornado;

namespace RoboCoP.Plus.Common
{
    [Serializable]
    public class DWMTrajectory
    {
        [Thornado]
        public List<Point> Path;
        [Thornado]
        public Angle EndAngle;
    }
}

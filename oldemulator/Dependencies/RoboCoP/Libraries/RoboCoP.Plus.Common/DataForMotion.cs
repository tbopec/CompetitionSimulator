using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AIRLab.Mathematics;
using AIRLab.Thornado;
using System.Drawing;

namespace RoboCoP.Plus.Common
{
    [Serializable]
    public class MotionPlan
    {
        [Thornado]
        public List<Point2D> Dots = new List<Point2D>();
        [Thornado]
        public Angle FinalAngle;
        [Thornado]
        public string Comment;
    }
}

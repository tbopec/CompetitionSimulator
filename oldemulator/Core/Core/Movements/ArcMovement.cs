using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AIRLab.Thornado;
using AIRLab.Mathematics;

namespace Eurosim.Core
{
    [Serializable]
    public class ArcMovement : IPlaneMovement
    {
        [Thornado]
        public Angle Rotation;

        [Thornado]
        public double Distance;

        [Thornado]
        public double TotalTime { get; set; }
        [Thornado]
        public string Comment { get; set; }


        public Frame2D GetOffset(double startTime, double dtime)
        {
            //Это неправильно. Но работать будет ^^
            var q = dtime / TotalTime;
            return new Frame2D(Distance * q,0,Rotation * q);
        }

       
    }
}

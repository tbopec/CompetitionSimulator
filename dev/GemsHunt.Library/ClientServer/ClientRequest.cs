using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AIRLab.Mathematics;
using AIRLab.Thornado;

namespace GemsHunt.Library.ClientServer
{
    public class ClientResponse
    {
        [Thornado]
        public string Team;
        [Thornado]
        public Command Command;

    }

    public class Command
    {
        [Thornado]
        public double Move;
        [Thornado]
        public double Angle;
        [Thornado]
        public bool Grip;
        [Thornado]
        public bool Release;
    }
    public class ClientRequest
    {
        [Thornado]
        public bool IsExit;
        [Thornado]
        public string Team;
        [Thornado]
        public Frame3D Position;

        [Thornado]
        public string Kinect;
        [Thornado]
        public string Camera;
    }
}

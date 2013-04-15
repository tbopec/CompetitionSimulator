using System;
using System.Collections.Generic;
using System.Text;
using RoboCoP.Plus;
using AIRLab.Thornado;
using AIRLab.Mathematics;
using Eurosim.Core;

namespace EmulatorBasicTest
{
    class EmulatorBasicTestSettings : ServiceSettings
    {
        [Thornado]
        public double MovementTime = 1;

        [Thornado]
        public List<TrivialPlaneMovement> CustomMovements = new List<TrivialPlaneMovement>();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eurosim.Core
{
    public class ActuatorAction : IRobotAction
    {
        public string ActuatorCommand;
        public double TotalTime { get; set; }
    }

    public class ActuatorExternalAction : ActuatorAction
    {
        public int ActuatorNumber;
    }
}

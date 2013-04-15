using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eurosim.Core
{
    public interface RobotAI
    {
        void DefineRobot(Robot robot);
        ACMCommand Perform(ACMSensorInfo info);
    }
}

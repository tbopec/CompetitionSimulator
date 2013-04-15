using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eurosim.Core;
using AIRLab.Mathematics;

namespace Eurosim.ChessUp
{
    public class Sheogorath : RobotAI
    {
        Random rnd; 

        public Sheogorath() { rnd=new Random(); }

        public void DefineRobot(Robot robot)
        {
			//robot.Actuators.Add(new ChessUpActuator(robot));
        }

        bool firstTime = true;

        public ACMCommand Perform(ACMSensorInfo info)
        {

var cmd = new ACMCommand();

if (firstTime)
    
{
    firstTime = false;
    return cmd.Mov(150).Rot(-Angle.HalfPi).Mov(100);
}
			//return cmd;
            cmd.TrivialPlaneMovement = new List<TrivialPlaneMovement>
            {
                new TrivialPlaneMovement()
                {
                Offset = new Frame2D(
                    (rnd.NextDouble() - 0.5) * 100,
                    (rnd.NextDouble() - 0.5) * 100,
                    Angle.FromGrad((rnd.NextDouble() - 0.5) * 180)),
                TotalTime = 1
                }
            };
            cmd.NextRequestInterval = 1;
            return cmd;
        }
    }
}

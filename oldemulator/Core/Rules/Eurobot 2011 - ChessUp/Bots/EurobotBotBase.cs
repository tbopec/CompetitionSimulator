using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eurosim.Core;
using AIRLab.Mathematics;

namespace Eurosim.ChessUp
{
    public abstract class EurobotBotBase : RobotAI
    {
        bool flip = false;

        public void DefineRobot(Robot robot)
        {
            flip = robot.RobotNumber == 1;
            robot.Navigators.Add(new NavigatorModel(robot, new EmulatedNavigatorSettings()));
            robot.MagicEyes.Add(new MagicEye(robot, new MagicEyeSettings { LocalCoordinates = false }));
            InternalDefine(robot);
        }

        public abstract void InternalDefine(Robot robot);

        public abstract ACMCommand InternalPerform(ACMSensorInfo info);

        public ACMCommand Perform(ACMSensorInfo info)
        {
            if (flip)
                InvertInput(info);
            var res = InternalPerform(info);
            if (flip)
                InvertOutput(res);
            return res;
        }

        Frame2D InvertFrame(Frame2D f)
        {
            return new Frame2D(-f.X, f.Y, Angle.Pi - f.Angle);
        }

        void InvertInput(ACMSensorInfo inf)
        {
            inf.NavigatorInfo[0].Location = InvertFrame(inf.NavigatorInfo[0].Location);
            foreach (var obj in inf.MagicEyeInfo[0].Objects)
                obj.Location = InvertFrame(obj.Location);
        }

        void InvertOutput(ACMCommand movs)
        {
            if (movs.TrivialPlaneMovement != null)
                movs.TrivialPlaneMovement = movs.TrivialPlaneMovement
                    .Select(z => new TrivialPlaneMovement
                    {
                        Offset = new Frame2D(z.Offset.X, z.Offset.Y, -z.Offset.Angle),
                        TotalTime = z.TotalTime
                    }).ToList();
        }

        public static ACMCommand Act(string action)
        {
            return new ACMCommand
            {
                ActuatorCommands = new string[] { action },
                NextRequestInterval = 0.5
            };
        }

    }
}

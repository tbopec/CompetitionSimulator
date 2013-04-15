using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eurosim.Core;
using AIRLab.Mathematics;

namespace Eurosim.ChessUp
{
    public class Namira : EurobotBotBase
    {
       
        public override void InternalDefine(Robot robot)
        {
            robot.Actuators.Add(new ChessUpActuator(robot, new ChessUpActuatorSettings { ActionAngle = Angle.FromGrad(90), ActionDistance = 40 , HasModel=true}));
        }


        ACMCommand PerformStep0(List<MagicEyeObject> objects, Frame2D location)
        {
            var king=objects.Where(z => z.Location.X < 100 && z.Name == "King").First();
            if (king.Location.Y<-70)
                king = objects.Where(z => z.Location.X < 100 && z.Name == "Queen").First();
            return new ACMCommand()
            .MoveTo(ref location, -90, location.Y)
            .MoveTo(ref location, -90, king.Location.Y)
            .MoveTo(ref location, -120, king.Location.Y);
        }

        ACMCommand GoToPawn(List<MagicEyeObject> objects, Frame2D location)
        {
        	Frame2D location1 = location;
        	var nearest = objects
                .Where(z => z.Name == "Pawn" && Math.Abs(z.Location.X) < 100)
                .Select(z => new { Obj = z, Dist = Angem.Hypot(z.Location-location1) })
                .OrderBy(z => z.Dist)
                .FirstOrDefault();
            return new ACMCommand()
            .MoveAlmostTo(ref location, nearest.Obj.Location.X, nearest.Obj.Location.Y, 10);
                
        }

        

        int step = -1;

        public override ACMCommand InternalPerform(ACMSensorInfo info)
        {
            var cmd = new ACMCommand();
            var objects = info.MagicEyeInfo[0].Objects;
            var location = info.NavigatorInfo[0].Location;
            step++;
            switch (step)
            {
                case 0: return PerformStep0(objects, location);
                case 1: return Act("Grip");
                case 2: return Act("Raise");
                case 3: return cmd.MoveTo(ref location, -90, location.Y);
                case 4: return GoToPawn(objects, location);
                case 5: return Act("Grip");
                case 6: return Act("Raise");
                case 7: return GoToPawn(objects, location);
                case 8: return Act("Grip");
                //case 9: return Act("Raise");
                case 10: return cmd.MoveAlmostTo(ref location, 45, -15, 7.5);
                case 11: return cmd.MoveAlmostTo(ref location, 45, -15, 7.5);
                case 12: return Act("Release");
                case 13: return cmd.Mov(-20);
            }
            return new ACMCommand
            {
                NextRequestInterval = 1
            };
        }
    }
}

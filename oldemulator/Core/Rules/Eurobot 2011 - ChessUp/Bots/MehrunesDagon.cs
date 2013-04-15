using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eurosim.Core;
using AIRLab.Mathematics;

namespace Eurosim.ChessUp
{
    public class MehrunesDagon : EurobotBotBase
    {
        public override void InternalDefine(Core.Robot robot)
        {
            
        }

        bool strike = true;

        public override Core.ACMCommand InternalPerform(Core.ACMSensorInfo info)
        {
            var cmd=new ACMCommand();
            if (!strike)
            {
                strike=true;
                return cmd.Mov(-100);
            }
                

            var c = info.MagicEyeInfo[0].Objects.Where(z => z.Name.Contains("Rob")).FirstOrDefault();
            if (c == null) return new Core.ACMCommand { NextRequestInterval = 0.1 };
            var loc = info.NavigatorInfo[0].Location;
            var dest = Angem.Hypot(c.Location.Center, info.NavigatorInfo[0].Location.Center);
            if (dest < 30)
            {
                strike = false;
                return cmd.MoveTo(ref loc, c.Location.X, c.Location.Y);
            }
            return cmd.MoveAlmostTo(ref loc, c.Location.X, c.Location.Y, dest - 10);
        }
    }
}

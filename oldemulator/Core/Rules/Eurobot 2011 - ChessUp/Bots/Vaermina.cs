using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AIRLab.Mathematics;
using Eurosim.Core;

namespace Eurosim.ChessUp
{
    public class Vaermina : EurobotBotBase
    {
        public override void InternalDefine(Core.Robot robot)
        {
            
        }
        public override Core.ACMCommand InternalPerform(Core.ACMSensorInfo info)
        {
            var cmd = new ACMCommand();
            var c = info.MagicEyeInfo[0].Objects.Where(z => z.Name.Contains("Rob")).FirstOrDefault();
            if (c == null) return new Core.ACMCommand { NextRequestInterval = 0.1 };
            var loc = info.NavigatorInfo[0].Location;
            var where = new Point2D(c.Location.X + 50 * Angem.Cos(c.Location.Angle), c.Location.Y + 50 * Angem.Sin(c.Location.Angle));
            var dest = Angem.Hypot(loc.Center, where);
            return cmd.MoveAlmostTo(ref loc, where.X, where.Y, dest - 10);
        }
    }
}

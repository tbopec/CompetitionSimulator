using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eurosim.Core;
using AIRLab.Mathematics;

namespace Eurosim.Core
{
    public static class ListTMExtentions
    {

        static double LinearVelocity = 40;
        static Angle AngularVelocity = Angle.FromGrad(100);

        public static ACMCommand Rot(this ACMCommand movs, Angle angle)
        {
            if (movs.TrivialPlaneMovement==null) movs.TrivialPlaneMovement = new List<TrivialPlaneMovement>();
            angle = angle.Simplify180();
            movs.TrivialPlaneMovement.Add(new TrivialPlaneMovement
            {
                Offset = new Frame2D(0, 0, angle),
                TotalTime = Math.Abs(angle / AngularVelocity)
            });
            movs.Autotime();
            return movs;
        }

        public static ACMCommand Mov(this ACMCommand movs, double dst)
        {
            if (movs.TrivialPlaneMovement == null) movs.TrivialPlaneMovement = new List<TrivialPlaneMovement>();
            movs.TrivialPlaneMovement.Add(new TrivialPlaneMovement
            {
                Offset = new Frame2D(dst, 0, Angle.Zero),
                TotalTime = Math.Abs(dst / LinearVelocity)
            });
            movs.Autotime();
            return movs;
        }

        public static ACMCommand MoveTo(this ACMCommand movs, ref Frame2D Location, double x, double y)
        {
            return movs.MoveTo(ref Location, x, y, double.NaN);
        }

        public static ACMCommand MoveTo(this ACMCommand movs, ref Frame2D Location, double x, double y, double grad)
        {
            var point = Location.Invert().Apply(new Point2D(x, y));
            var angle = Angem.Atan2(point.Y, point.X).Simplify180();
            movs.Rot(angle);
            movs.Mov(Angem.Hypot(point.Y, point.X));
            if (!double.IsNaN(grad))
            {
                movs.Rot(Angle.FromGrad(grad) - Location.Angle);
                Location = new Frame2D(x, y, Angle.FromGrad(grad));
            }
            else
                Location = new Frame2D(x, y, angle);
            return movs;
        }

        public static ACMCommand MoveAlmostTo(this ACMCommand movs, ref Frame2D Location, double x, double y, double defekt)
        {
            var point = Location.Invert().Apply(new Point2D(x, y));
            var angle = Angem.Atan2(point.Y, point.X);
            var dst = Angem.Hypot(point.Y, point.X) - defekt;
            movs.Rot(angle);
            movs.Mov(dst);
            return movs;
        }

       
        
    }
   
}

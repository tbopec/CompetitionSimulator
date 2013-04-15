using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eurosim.Core;
using AIRLab.Mathematics;

namespace Eurosim.ChessUp
{
    public class Nocturnal : EurobotBotBase
    {
        int step = -1;

        public override void InternalDefine(Robot robot)
        {
            robot.Actuators.Add(new ChessUpActuator(robot, new ChessUpActuatorSettings { ActionAngle = Angle.FromGrad(40), ActionDistance = 30, HasModel = true }));
        }


        void TestBoard(Action<int, int, bool,Point2D> action)
        {
             for (int x = 0; x < 6; x++)
                 for (int y = 0; y < 6; y++)
                 {
                     var xx = -33 * 3 + 16 + 33 * x;
                     var yy = 33 * 3 + 16 - 33 * y;
                     var mine = (x + y) % 2 == 0;
                     action(x, y, mine, new Point2D(xx, yy));
                 }
           
        }

        bool FigureInside(MagicEyeObject e, Point2D cell)
    {
        return !e.Name.Contains("Rob") && Math.Abs(e.Location.X - cell.X) < 10 && Math.Abs(e.Location.Y - cell.Y) < 10;
    }


        public ACMCommand FindGoodPlace(ACMSensorInfo info)
        {
            var list = new List<Point2D>();

            TestBoard((x, y, mine, point) =>
                {
                    if (y == 0) return;
                    if (y == 5) return; 
                    if (!mine) return;
                    if (info.MagicEyeInfo[0].Objects.Where(z => FigureInside(z, point)).Count() != 0) return;
                    list.Add(point);


                });

            if (list.Count == 0) return new ACMCommand() { NextRequestInterval = 1 };
            list = list.OrderBy(z => Angem.Hypot(z, info.NavigatorInfo[0].Location.Center)).ToList();
            var loc = info.NavigatorInfo[0].Location;
            return new ACMCommand().MoveAlmostTo(ref loc, list[0].X, list[0].Y, 15);

        }

        public ACMCommand FindEnemyFigure(ACMSensorInfo info)
        {
            var premium = new List<Frame2D>();
            var common = new List<Frame2D>();

            foreach (var e in info.MagicEyeInfo[0].Objects)
            {
                if (e.Name.Contains("Rob")) continue;
                bool skip=false;
                TestBoard((x, y, mine, point) =>
                    {
                        if (!FigureInside(e, point)) return;
                        skip = true;
                        if (mine) return;
                        if (FigureInside(e, point))
                            premium.Add(e.Location);
                    });
                if (skip) continue;
                common.Add(e.Location);
            }

            var list = common;
            if (premium.Count() != 0)
                list = premium;
            list=list.OrderBy(z => Angem.Hypot(z.Center, info.NavigatorInfo[0].Location.Center)).ToList();
            var loc = info.NavigatorInfo[0].Location;
            return new ACMCommand().MoveAlmostTo(ref loc, list[0].X, list[0].Y, 15);

        }

        public override ACMCommand InternalPerform(ACMSensorInfo info)
        {
            

            var cmd = new ACMCommand();
            var objects = info.MagicEyeInfo[0].Objects;
            var location = info.NavigatorInfo[0].Location;

            step++;

            switch (step)
            {
                case 0: return cmd.Mov(50).Rot(Angle.FromGrad(-45)).Mov(60);
                case 1: return FindEnemyFigure(info);
                case 2: return Act("Grip");
                case 3: return FindGoodPlace(info);
                case 4: return Act("Release");
                case 5: return cmd.Mov(-30).Rot(Angle.FromGrad(90));
                case 6: step = 0; break;
                    
            }

            

            return new ACMCommand
            {
                NextRequestInterval = 1
            };
        }

      
    }
}

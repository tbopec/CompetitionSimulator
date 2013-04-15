using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AIRLab.Mathematics;

namespace Eurosim.Core
{
    public class DumbGrabbingActuatorSettings : ActuatorSettings
    {
    }

    /// <summary>
    /// Тупой хватающий актуатор. Хватает любой объект в зоне действия. Хавает внутрь себя и делает нефизическим.
    /// Командой Release высыпает все обратно на поле
    /// </summary>
    public class DumbGrabbingActuator : Actuator
    {
        /// <summary>
        /// Взятые актуатором фигуры
        /// </summary>
        protected BodyCollection<Body> CarriedFigure = new BodyCollection<Body>();

        public DumbGrabbingActuator(Robot robot, ActuatorSettings settings)
            : base(robot, settings)
        {
            Name = "DumbGrabbingActuator" + Robot.RobotNumber;
            Add(CarriedFigure);
        }

        public override void DoActions(ActuatorAction __action, double startTime, double endTime)
        {
            var action = __action.ActuatorCommand;
            if (startTime != 0) return; //делаем только начало действия
            if (string.IsNullOrEmpty(action))
                return;
            switch (action)
            {
                case "Grip":
                    var near = FindNearest(Settings.ActionDistance, Settings.ActionAngle);
                    if (near.Count>0)
                    Grip(near.First(), new Frame3D());
                    break;
                case "Release": //Вернуть одно тело
                    Release(new Frame3D());
                    break;
            }
        }

        protected void Grip(Body what, Frame3D where)
        {
            if (what == null)
                return;
            Robot.Emulator.Objects.Remove(what);
            what.Location = where;
            CarriedFigure.Add(what);
        }

        protected Body Release(Frame3D where)
        {
            var body = CarriedFigure.FirstOrDefault();
            if (body == null)
                return null;
            CarriedFigure.Remove(body);
            body.Location = CarriedFigure.GetAbsoluteLocation().Apply(where);
            Robot.Emulator.Objects.Add(body);
            return body;
        }

        public override void Reset()
        {
            base.Reset();
            CarriedFigure.Clear();
            State = String.Empty;
        }

        /// <summary>
        /// Находит ближайшее тело типа T лежащее в заданном секторе круга
        /// </summary>
        /// <param name="dist">Радиус</param>
        /// <param name="areaAngle">Угол сектра </param>
        /// <param name="center">Центр</param>
        /// <returns></returns>
        protected List<Body> FindNearest(double dist, Angle areaAngle, Frame3D center)
        {
            var list = new SortedList<double, Body>();
            var thisLocation = GetAbsoluteLocation().Apply(center);
            var loc = thisLocation.Invert();
            foreach (var e in Robot.Emulator.Objects)
            {
                var objLoc = loc.Apply(e.GetAbsoluteLocation()).ToFrame2D();
                var ang = -Angem.Atan2(objLoc.Y, objLoc.X);
                if (ang < (areaAngle / 2) && ang > (-areaAngle / 2))
                {
                    var newdist = Angem.Hypot(objLoc);
                    if (newdist < dist)
                    {
                        try
                        {
                            list.Add(newdist, e);
                        }catch{}
                    }
                }
            }
            return list.Values.ToList();
        } 

        protected List<Body> FindNearest(double dist, Angle areaAngle)
        {
            return FindNearest(dist, areaAngle, Frame3D.Identity);
        }
    }
}

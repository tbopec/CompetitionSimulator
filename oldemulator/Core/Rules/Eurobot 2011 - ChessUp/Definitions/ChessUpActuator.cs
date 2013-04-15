using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using AIRLab.Mathematics;
using AIRLab.Thornado;
using Eurosim.Core;
using System.Linq;

namespace Eurosim.ChessUp
{
    public class ChessUpActuatorSettings:DumbGrabbingActuatorSettings
    {
        [Thornado] public bool HasModel;
    }
    public enum GripState { Lowered, Raised }
    public class ChessUpActuator : DumbGrabbingActuator
    {
        private GripState GripState=GripState.Lowered;

        //лапки
        private readonly BodyCollection<Body> Grips=new BodyCollection<Body>();
        /// <summary>
        /// Точка, в которую помещаются фигуры
        /// </summary>
        private readonly Frame3D DefaultLocation = new Frame3D(15, 0, 0);
        private const double FigureHeight = 5;

        public ChessUpActuator(Robot robot, ChessUpActuatorSettings settings)
            : base(robot, settings)
        {
            Name = "ChessUpActuator" + Robot.RobotNumber;
            if (settings.HasModel)
            {
                foreach (var i in new[] {1, -1})
                    Grips.Add(new PhysicalPrimitiveBody(new BoxShape(10, 1, 5), Color.Green)
                                  { Location = new Frame3D(10, i*13, 0), Mass = 0.5 });
                Add(Grips);
            }
            CarriedFigure.Name = "CarriedFigure";
        }

        public override void DoActions(ActuatorAction __action, double startTime, double endTime)
        {
             var action = __action.ActuatorCommand;
            if (startTime!=0) return; //делаем только начало действия
            if (string.IsNullOrEmpty(action))
                return;
            switch (action)
            {
                case "Grip":
                    var near = FindNearest(Settings.ActionDistance, Settings.ActionAngle);
                    var nearest = near.FirstOrDefault();
                    if (nearest != null)
                    {
                        var fig = CarriedFigure.FirstOrDefault();
                        if(fig!=null && fig.Name.Contains("3"))return;
                        if (fig!=null &&
                           (nearest.Name.Contains(ChessUpFigure.Queen)||
                           nearest.Name.Contains(ChessUpFigure.King)))
                            return;
                        ChangeHeight("Lower");
                        Grip(nearest, DefaultLocation);
                        if (CarriedFigure.Count() >=2)
                        {
                            ChessUpRules.Figures type;
                            Enum.TryParse(CarriedFigure.First().Name, out type);
                            if (CarriedFigure.ElementAt(1).Name.Contains("2"))
                                type += 2;
                            else
                                type++;
                            CarriedFigure.Clear();
                            var x = ChessUpFigure.CreateFigure(type, DefaultLocation);
                            CarriedFigure.Add(x);
                        }
                        State = CarriedFigure.First().Name;
                    }
                    break;
                case "Release":
                    var c = CarriedFigure.FirstOrDefault();
                    if (c == null) return;
                    if (GripState == GripState.Raised)
                    {
                        var onFloor = FindNearest(6, Angle.Pi, DefaultLocation).FirstOrDefault();
                        if (onFloor != null && onFloor.Name == ChessUpFigure.Pawn && !c.Name.Contains("3"))
                        {
                            Robot.Emulator.Objects.Remove(onFloor);
                            IncTower();
                        }
                        ChangeHeight("Lower");
                    }
                    Release(c.Location);
                    State = String.Empty;
                    break;
                case "Raise":
                case "Lower":
                    ChangeHeight(action);
                    break;
            }
        }
        private void IncTower()
        {
            ChessUpRules.Figures type;
            Enum.TryParse(CarriedFigure.First().Name, out type);
            type++;
            CarriedFigure.Clear();
            var x = ChessUpFigure.CreateFigure(type, DefaultLocation);
            CarriedFigure.Add(x);
        }
        public override void Reset()
        {
            base.Reset();
            if (GripState == GripState.Raised)
                ChangeHeight("Lower");
        }

        private void ChangeHeight(string command)
        {
            if (GripState == GripState.Lowered && command.Equals("Raise"))
            {
                this.AddZ(5);
                foreach (var body in CarriedFigure)
                {
                    body.SetMaterial(false);
                }
                GripState = GripState.Raised;
            }
            else if (GripState == GripState.Raised && command.Equals("Lower"))
            {
                this.AddZ(-5);
                foreach (var body in CarriedFigure)
                {
                    body.SetMaterial(true);
                }
                GripState = GripState.Lowered;
            }
        }
    
    }
    static class SomeExtensions
    {
        public static void AddZ(this Body body, double z)
        {
            body.Location=body.Location.NewZ(body.Location.Z+z);
        }
        public static void SetZ(this Body body, double z)
        {
            body.Location = body.Location.NewZ(z);
        }
    }

}

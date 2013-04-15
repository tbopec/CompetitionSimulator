using System.Collections.Generic;
using System.Drawing;
using AIRLab.Mathematics;
using AIRLab.Thornado;
using Eurosim.Core;
using System;
using System.Linq;

namespace Eurosim.TreasureIsland
{
    /// <summary>
    /// Универсальный актуатор для TreasureIsland.
    ///
    /// Кроме обычных Grab и Release может выполнять и эти действия:
    /// открыть сундук, закрыть сундук, нажать кнопку
    /// </summary>
    public class TreasureIslandActuatorSettings : DumbGrabbingActuatorSettings
    {
        [Thornado] public bool HasModel;
    }

    public class TreasureIslandActuator : DumbGrabbingActuator
    {

        public TreasureIslandActuator(Robot robot, TreasureIslandActuatorSettings settings)
            : base(robot, settings)
        {
            if (settings.HasModel)
            {
                Add(new PhysicalPrimitiveBody(new BoxShape(1, 28, 5), Color.Gray)
                        {
                            Location = new Frame3D(6, 0, 2.5),
                            Mass=0.01,
                            FrictionCoefficient = 0
                        });
                foreach (var i in new[] {-1, 1})
                    Add(new PhysicalPrimitiveBody(new BoxShape(10, 1, 5), Color.Gray)
                            {
                                Location = new Frame3D(10, 14*i, 2.5),
                                Mass = 0.01,
                                FrictionCoefficient = 0
                            });
            }
        }
        public override void DoActions(ActuatorAction __action, double startTime, double endTime)
        {
            var action = __action.ActuatorCommand;
            if (startTime != 0) return; //делаем только начало действия
            if (string.IsNullOrEmpty(action))
                return;
            var near = FindNearest(Settings.ActionDistance, Settings.ActionAngle);
            var nearChest = FindNearest(50, Angle.Pi).OfType<Chest>();
            try
            {
                switch (action)
                {
                    case "Grip":
                        var first = near.Find(x => (x is Coin || x is Ingot || x is Cloth));
                        first.SetMaterial(false); // не работает без кода внизу
						if (first is PhysicalPrimitiveBody) // работает IsMaterial = true только если схвачена одна монета, если больше => лаги
							(first as PhysicalPrimitiveBody).IsMaterial = false;
                        Grip(first, new Frame3D(15, 0, CarriedFigure.Count()*3 + 2));						
                        break;

                    case "Release":
                        var body=Release(new Frame3D(10, 0, 0));
                        body.SetMaterial(true);
                        break;

                    case "OpenChest":
                    case "CloseChest":
                        if (nearChest.Any())
                        {
                            var chest = nearChest.First();
                            chest.State=(chest.State.Equals(Chest.ChestState.Closed)?
                                Chest.ChestState.Open:Chest.ChestState.Closed);
                        }
                        break;

                    case "PushButton":
                        if (near.Count == 0)
                            return;
                        var first1 = near.Find(x => x is Button);
                        (first1 as Button).State = Button.ButtonState.Activated;
                        break;

                    case "PillageTotem":
                        var loc = this.GetAbsoluteLocation();
                        var totemLocation = new Frame3D(Math.Sign(loc.X)*40, 0, 0);
                        if (IsInsideSquare(loc, totemLocation, 45))
                            foreach (var e in Robot.Emulator.Objects.ToList().
                                Where(e => IsInsideSquare(e.Location, totemLocation, 25)))
                            {
                                e.Location = e.Location.NewX(20);
                                Robot.Emulator.Objects.Remove(e);
                                CarriedFigure.Add(e);
                            }
                        break;
                }
            }
            catch (Exception)
            {
                //Не получилось взять элемент. пустой List или пустая BodyCollection
            }
            State = CarriedFigure.Count() + "Items";
        }

        private static bool IsInsideSquare(Frame3D frame, Frame3D center, double size)
        {
            return Math.Abs(frame.X - center.X) < size/2 && Math.Abs(frame.Y - center.Y) < size/2;
        }
    }
}


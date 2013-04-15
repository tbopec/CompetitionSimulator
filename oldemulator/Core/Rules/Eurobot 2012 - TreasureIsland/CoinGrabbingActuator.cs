using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AIRLab.Mathematics;
using AIRLab.Thornado;
using Eurosim.TreasureIsland;

namespace Eurosim.Core
{
    public class CoinGrabbingActuatorSettings:DumbGrabbingActuatorSettings
    {
        [Thornado] public bool HasModel;
    }
    /// <summary>
    /// Актуатор умееет хватать только монетки. Складывает их вертикально в стопку.
    /// </summary>
    class CoinGrabbingActuator:DumbGrabbingActuator
    {
        private int coinCount;
        private const double figureHeight = 4;

        public CoinGrabbingActuator(Robot robot, CoinGrabbingActuatorSettings settings):base (robot, settings)
        {
            Location = Location.NewZ(Location.Z - 15);
            if (settings.HasModel)
                Add(new PrimitiveBody("TreasureIsland.coinactuatorbody"){Location = new Frame3D(12.5, 0, 1)});
            Name = "CoinGrabbingActuator" + Robot.RobotNumber;
        }
        public override void DoActions(ActuatorAction __action, double startTime, double endTime)
        {
            var action = __action.ActuatorCommand;
            if (startTime != 0) return; //делаем только начало действия
            if (string.IsNullOrEmpty(action))
                return;
            try
            {
                switch (action)
                {
                    case "Grip":
                        var near = FindNearest(Settings.ActionDistance, Settings.ActionAngle).OfType<Coin>();
                        var n = near.First();
                        n.SetMaterial(false);
                        Grip(n, new Frame3D(10, 0, 2 + figureHeight*coinCount));
                        coinCount++;
                        break;
                    case "Release":
                        var coin=Release(new Frame3D(10, 0, 0));
                        coin.SetMaterial(true);
                        coinCount--;
                        break;
                }
            }catch(Exception)
            {
                
            }
            State=CarriedFigure.Count()+"Items";
        }


        public override void Reset()
        {
            base.Reset();
            coinCount = 0;
        }

    }
}

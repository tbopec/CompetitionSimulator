using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using AIRLab.Mathematics;
using Eurosim.Core;
using Eurosim.Core.Physics;

namespace Eurosim.TreasureIsland
{
    class TreasureIslandRules:Rules
    {
        public override void PositionRobots()
        {
            foreach (var robot in Emulator.Robots.ToArray())
            {
                if (robot.RobotNumber == 0)
                    robot.TeleportRobot(new Frame3D(-150 + 20, 105 - 20, 0));
                else if (robot.RobotNumber == 1)
                    robot.TeleportRobot(new Frame3D(150 - 20, 105 - 20, 0,Angle.Zero, Angle.Pi, Angle.Zero));
            }
        }
        public override void AdditionalDefineRobots()
        {
            AddArrows(redside:1);
        }
        private static readonly Color brown = Color.FromArgb(0x55, 0x33, 0x33);
        public static readonly Color sand = Color.FromArgb(0xff, 0xee, 0x11);
        private static readonly Color jungle = Color.FromArgb(0x55, 0xff, 0x11);
        private static readonly Color sea = Color.FromArgb(0x11, 0x99, 0xcc);
        private static readonly Color red = Color.FromArgb(0xbb, 0x00, 0x11);
        private static readonly Color blue =Color.FromArgb(0x55,0x11, 0xee);

    	public TreasureIslandRules(Emulator emulator):base(emulator)
    	{
    	}

    	#region Инициализация тел
        public override IEnumerable<Body> InitializePieces()
        {
			var is3d = (!PhysicalManager.Is2d);

            var objects = new List<Body>();
            //номера черных монет в списке
            var blacknums = new List<int> { 3, 9, 1, 12 };
            var startcoin = new Frame3D(50, 50, 0);//always white
            var coins = new List<Tuple<Frame3D, bool>>();
            var coinsOnFloor = new List<Frame3D>{
                 new Frame3D(105, -70,0), new Frame3D(40, 22.5,0),
                 new Frame3D(56, 16,0),new Frame3D(62.5,0, 0), new Frame3D(56,-16,0), 
                 new Frame3D(40,-22.5,0), new Frame3D(24, -16,0),new Frame3D(24,16, 0),
                 new Frame3D(10, -68.5,0),new Frame3D(0,-62,0), new Frame3D(0,-80,0)
            };
            foreach (var frame in coinsOnFloor)
            {
                coins.Add(new Tuple<Frame3D, bool>(frame, true));
                if (frame.X != 0)
                    coins.Add(new Tuple<Frame3D, bool>(frame.NewX(-frame.X), true));
            }
            var coinsOnTotem = new List<Frame3D>
            {
                new Frame3D(40+12.5-5.3, 12.5-5.3,17),new Frame3D(40+12.5-5.3,-12.5+5.3,17), 
                new Frame3D(40-12.5+5.3, 12.5-5.3, 17),new Frame3D(40-12.5+5.3, -12.5+5.3, 17),
                new Frame3D(40+12.5-5.3, 12.5-5.3,2.3), new Frame3D(40+12.5-5.3,-12.5+5.3,2.3), 
                new Frame3D(40-12.5+5.3, 12.5-5.3, 2.3), new Frame3D(40-12.5+5.3, -12.5+5.3, 2.3)
            };
            foreach (var frame in coinsOnTotem)
            {
				coins.Add(new Tuple<Frame3D, bool>(frame, is3d));
				coins.Add(new Tuple<Frame3D, bool>(frame.NewX(-frame.X), is3d));
            }

            objects.Add(new Ingot(new Frame3D(0, -35.3, 0, Angle.Zero, Angle.HalfPi, Angle.Zero), true));
            foreach (var i in new[] {-1, 1})
            {
                //ingots on side
                objects.Add(new Ingot(new Frame3D(108*i, 14, 0, Angle.Zero, Angle.FromGrad(3), Angle.Zero), true));
                foreach (var j in new[] {-1, 1}) //on totem
					objects.Add(new Ingot(new Frame3D(40 * i, 9 * j, 9.1, Angle.Zero, Angle.HalfPi, Angle.Zero), is3d));
                //тряпочки на карте
                objects.Add(new Cloth(new Frame3D(9.1 * i, 100, 7),(i > 0 ? "red" : "blue") ));
                
                //монеты возле старта всегда белые
                objects.Add(new Coin(startcoin.NewX(startcoin.X*i), true, "white"));
                //кнопки
                foreach (var j in new[] { -1, 1 })
                    objects.Add(new Button(
                                    new Frame3D(j*23.85 + i*62.15, -100, 0, Angle.Zero, Angle.Pi, Angle.Zero),
                                    (j > 0) ? "red" : "blue"));
                //ящики
                objects.Add(new Chest( new Frame3D(133*i, -69.5, 7)));
            }
            for (var j = 0; j < coins.Count; j++)
                    objects.Add(new Coin(coins[j].Item1.NewX(coins[j].Item1.X), coins[j].Item2, 
                        (blacknums.Contains(j)?"black":"white")));

            return objects;
        }

        public override IEnumerable<Body> InitializeTable()
        {
            var table = new List<Body>();
			if (PhysicalManager.Is2d)
				Floor = new PrimitiveBody(new BoxShape(300, 210, 3), sea, "TreasureIsland.table")
				{
					Location = new Frame3D(0, 0, -0.5)					
				};			
			else
				Floor = new PhysicalPrimitiveBody(new BoxShape(300, 210, 3), sea, "TreasureIsland.table")
				{
					Location = new Frame3D(0, 0, -0.5),
					IsStatic = true
				};
            table.Add(Floor);
            table.Add(new PhysicalPrimitiveBody(new CyllinderShape(7.5, 7.5, 25), brown, "TreasureIsland.palm")
            {
                IsStatic = true
            });
            table.Add(new PrimitiveBody(new BoxShape(30,1, 20), sea,"TreasureIsland.map"){Location = new Frame3D(0,100,7)});
            foreach (var i in new[]{-1,1})
            {
                //стенки
                table.Add(new PhysicalPrimitiveBody(new BoxShape(300, 2, 10), sea)
                {
                    Location = new Frame3D(0, i * 101, 0),
                    IsStatic = true
                });
                table.Add(new PhysicalPrimitiveBody(new BoxShape(2, 202, 10), sea)
                {
                    Location = new Frame3D(i*151, 0, 0),
                    IsStatic = true
                });
                table.Add(new PhysicalPrimitiveBody(new BoxShape(50, 1.8, 1.8),brown)
                {
                    IsStatic = true,
                    Location =new Frame3D(i*125, 49.2, 0) 
                });
                table.Add(new PhysicalPrimitiveBody(new BoxShape(75, 1.8, 1.8), brown)
                {
                    IsStatic = true,
                    Location = new Frame3D(i * 114.7, -62.5, 0, Angle.Zero, Angle.FromGrad(180-i*87.2),Angle.Zero)
                });
                //тотемы
				if (PhysicalManager.Is2d)
					table.Add(new PhysicalPrimitiveBody(new BoxShape(25, 25, 16.3), brown, "TreasureIsland.totem")
					{
						Location = new Frame3D(i * 40, 0, 0),
						IsStatic = true
					});
				else
				{
					for (var j = 0; j < 3; j++)
					{
						table.Add(new PhysicalPrimitiveBody(new BoxShape(25, 25, 1.8), brown)
						{
							Location = new Frame3D(i * 40, 0, j * 7.25),
							IsStatic = true
						});
					}
				}
            }
            return table;
        }
#endregion


        public override Actuator CreateActuator(Robot robot, ActuatorSettings settings)
        {
            if (settings is TreasureIslandActuatorSettings)
                return new TreasureIslandActuator(robot, (TreasureIslandActuatorSettings) settings);
            if (settings is CoinGrabbingActuatorSettings)
                return new CoinGrabbingActuator(robot, (CoinGrabbingActuatorSettings) settings);
            if (settings is DumbGrabbingActuatorSettings)
                return new DumbGrabbingActuator(robot, (DumbGrabbingActuatorSettings)settings);
            throw new Exception("Requested actuator with settings " + settings.GetType() + " is not found");
        }

        public override RobotAI CreateAI(string AIName)
        {
            throw new Exception("Unexpected AIName: " + AIName);
        }
    }
}

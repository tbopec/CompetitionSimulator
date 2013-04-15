using System;
using System.Linq;
using AIRLab.Mathematics;
using System.Collections.Generic;
using Eurosim.Core;
using System.Drawing;
using Eurosim.Core.Physics;

namespace Eurosim.ChessUp
{
    public partial class ChessUpRules : Rules
    {
    	public ChessUpRules(Emulator emulator)
    		: base(emulator)
    	{
    		
    	}

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
			AddArrows(redside:0);    
        }
        #region Задание конфигурации поля
        class FigureTask
        {
            public Figures Figure;
            public double x;
            public double y;
            public FigureTask(Figures f, double _x, double _y)
            {
                Figure = f;
                x = _x;
                y = _y;
            }
        }

         void AddPawnsC(List<FigureTask> list, int rowLine,int columnLine)
        {
            list.Add(new FigureTask(Figures.Pawn, (2 - columnLine) * 35, (rowLine - 2) * 35));
            list.Add(new FigureTask(Figures.Pawn, (columnLine-2) * 35, (rowLine - 2) * 35));
        }

         void AddPawns(List<FigureTask> list, int confNum, int columnLine)
        {
            var c = "01";
            switch (confNum)
            {
                case 0: c = "01"; break;
                case 1: c = "02"; break;
                case 2: c = "03"; break;
                case 3: c = "04"; break;
                case 4: c = "12"; break;
                case 5: c = "13"; break;
                case 6: c = "14"; break;
                case 7: c = "23"; break;
                case 8: c = "24"; break;
                case 9: c = "34"; break;
            }
            AddPawnsC(list, c[0] - '0', columnLine);
            AddPawnsC(list, c[1] - '0', columnLine);
        }
        

         List<FigureTask> CreateConfiguration(int ConfNum)
        {
            var task = new List<FigureTask>();
            task.Add(new FigureTask(Figures.Pawn, 0, 0));


            var queenConfNum = ConfNum / 100;
            var queenPlaces = queenConfNum % 10;

            string conf = "PPPPP";
            switch (queenPlaces)
            {
                case 0: conf = "QKPPP"; break;
                case 1: conf = "QPKPP"; break;
                case 2: conf = "QPPKP"; break;
                case 3: conf = "QPPPK"; break;
                case 4: conf = "PQKPP"; break;
                case 5: conf = "PQPKP"; break;
                case 6: conf = "PQPPK"; break;
                case 7: conf = "PPQKP"; break;
                case 8: conf = "PPQPK"; break;
                case 9: conf = "PPPQK"; break;
            }
            if (queenConfNum > 10) conf = conf.Replace('Q', 'T').Replace('K', 'Q').Replace('T', 'K');
            for (int i = 0; i < conf.Length; i++)
                foreach (var side in new int[] { -1, +1 })
                {
                    var fig = Figures.Pawn;
                    if (conf[i] == 'Q') fig = Figures.Queen;
                    if (conf[i] == 'K') fig = Figures.King;
                    task.Add(new FigureTask(fig, 130 * side, -85 + i * 170 / 6));
                }

            AddPawns(task, ConfNum % 10, 0);
            AddPawns(task, (ConfNum / 10) % 10, 1);
            return task;


        }
        #endregion


        public override IEnumerable<Body> InitializePieces()
        {
            var list = new List<Body>();
            var num = Emulator.Settings.PlayConfiguration;
            if (num == -1) num = new Random().Next(2000);
            var task = CreateConfiguration(num);
            foreach (var e in task)

                list.Add(ChessUpFigure.CreateFigure(e.Figure, new Frame3D(e.x, e.y, 0)));
            return list;
            
        }


        public override IEnumerable<Body> InitializeTable()
        {
            var TABLECOLOR = Color.FromArgb(0x22, 0x22, 0x22);
            const double TABLEXSIZE = 300;
            const double TABLEYSIZE = 210;
            const double WALLTHICKNESS = 2.2;
            const double WALLHEIGHT = 7;

            var table = new List<Body>();

			if (PhysicalManager.Is2d)			
				Floor = new PrimitiveBody(new BoxShape(300, 210, 3),
					Color.DarkGray, "table","untitled.png")
				{
					Location = new Frame3D(0, 0, -1.5),					
				};			
			else
				Floor = new PhysicalPrimitiveBody(new BoxShape(300, 210, 3), 
					Color.DarkGray, "table", "untitled.png")
				{
					Location = new Frame3D(0, 0, -1.5),
					IsStatic = true
				};

			table.Add(Floor);

            foreach (int i in new[] { -1, 1 })
            {
                table.Add(new PhysicalPrimitiveBody(
                    new BoxShape(2.2, TABLEYSIZE + WALLTHICKNESS, WALLHEIGHT),TABLECOLOR)
                    {
                        IsStatic = true,
                        Location = new Frame3D(i*(TABLEXSIZE/2 + WALLTHICKNESS/2), 0, 0)
                     });
                table.Add(new PhysicalPrimitiveBody(
                    new BoxShape(TABLEXSIZE + WALLTHICKNESS, WALLTHICKNESS, WALLHEIGHT), TABLECOLOR)
                    {
                        IsStatic = true,
                        Location = new Frame3D(0, i*(TABLEYSIZE/2 + WALLTHICKNESS/2), 0)
                    });
                //startzones
                var iColor = i > 0 ? Color.Blue : Color.Red;
                table.Add(new PhysicalPrimitiveBody(new BoxShape(40, WALLTHICKNESS, WALLHEIGHT), iColor)
                {
                    IsStatic = true,
                    Location = new Frame3D(i*(TABLEXSIZE/2 - 20), 65, 0)
                });
                //Закрытые зоны.
                table.Add(new PhysicalPrimitiveBody(new BoxShape(70, 12, WALLHEIGHT), TABLECOLOR)
                {
                    IsStatic = true,
                    Location = new Frame3D(i * 35 * 2, -TABLEYSIZE / 2 + 6, 0)
                });
                int l = 1;
                foreach (int j in new[] { -3, -1, 1, 3 })
                {
                    table.Add(new PhysicalPrimitiveBody(new BoxShape(WALLTHICKNESS, 13, WALLHEIGHT),TABLECOLOR)
                    {
                        IsStatic = true,
                        Location=new Frame3D(35 * j + l * (WALLTHICKNESS / 2), -TABLEYSIZE / 2 + 18.5, 0)
                    });
                    l = -l;
                }

            }
            return table;
        }

        public override Actuator CreateActuator(Robot robot, ActuatorSettings settings)
        {
            if (settings is ChessUpActuatorSettings)
                return new ChessUpActuator(robot, (ChessUpActuatorSettings) settings);
            if (settings is DumbGrabbingActuatorSettings)
                return new DumbGrabbingActuator(robot, settings);
            throw new Exception("Requested Actuator Not Found");
        }

        public override RobotAI CreateAI(string AIName)
        {
            switch (AIName)
            {
                case "Sheogorath": return new Sheogorath();
                case "Namira": return new Namira();
                case "Nocturnal": return new Nocturnal();
                case "MehrunesDagon": return new MehrunesDagon();
                case "Vaermina": return new Vaermina();
                default: return null;
            }
        }

        public enum Figures
        {
            Pawn,
            PawnTower2,
            PawnTower3,
            King,
            KingTower2,
            KingTower3,
            Queen,
            QueenTower2,
            QueenTower3
        };

       
    }
}
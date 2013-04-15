using System.Linq;
using System.Threading;
using System;
using System.Windows.Forms;
using AIRLab;
using Eurosim.Graphics;
using Eurosim.Core.Physics;
using Eurosim.Core.Physics.BepuWrap;
using Eurosim.Core.Physics.FarseerWrap;
using RoboCoP.Plus;
using System.Collections.Generic;

namespace Eurosim.Core
{
	public delegate RobotAI RobotAIFactory(string botName);

	public class Emulator : BodyCollection<Body>
	{
		public ServiceAppEnvironment App;

		public BodyCollection<Robot> Robots { get; private set; }
		public BodyCollection<Body> Objects { get; private set; }
		public BodyCollection<Body> TableObjects { get; private set; }
		public Rules Rules { get; private set; }
		public ScoreCollection Scores { get; private set; }
		public EmulatorSettings Settings { get; private set; }

		public double LocalTime { get; private set; }

		public readonly double DT = 1.0 / 60;
		public readonly int PhysicalPrecision = 10;

		private bool _firstTime = true;

		public event Action FirstTimeStarted;

		public bool ResetRequest { get; internal set; }

		public DateTime StartTime { get; private set; }

		
		public void MakeCycle(bool realtime)
		{
			if(_firstTime)
			{
				if(FirstTimeStarted != null)
					FirstTimeStarted();
				StartTime = DateTime.Now;
				_firstTime = false;
			}
			if(ResetRequest)
			{
				ResetInternal();
				ResetRequest = false;
			}

			var begin = DateTime.Now;
			if(Settings.PhysicsMode != PhysicalEngines.No)
			{
				// Посчитаем подходящий для физического мира dt
				var physicalDT = DT / PhysicalPrecision;
				for(int i = 0; i < PhysicalPrecision; i++)
				{
					foreach(var r in Robots)
					{
						r.MoveRobot(physicalDT);
					}
					PhysicalManager.MakeIteration(physicalDT, World);
				}
			}
			else
				foreach(var r in Robots)
				{
					r.MoveRobot(DT);
				}

			foreach(var robot in Robots)
				foreach(var sen in robot.SensorModels)
					sen.ProvideMeasure();

			foreach(var robot in Robots)
				robot.PerformAI(DT);

			Rules.AccountScores();
			var elapsed = 1000 * DT - (DateTime.Now - begin).TotalMilliseconds;
			if(realtime && elapsed > 0)
				Thread.Sleep((int)elapsed);
			LocalTime += DT;
		}


		public Emulator(EmulatorSettings settings)
			: this(settings, a => null)
		{
		}

		public Emulator(EmulatorSettings settings, RobotAIFactory customAiFactory)
		{
			World = new WorldBody {this};
			Settings = settings;
			switch(Settings.PhysicsMode)
			{
				case PhysicalEngines.Bepu:
					PhysicalManager.InitializeEngine(PhysicalEngines.Bepu, new BepuWorld());
					break;
				case PhysicalEngines.Farseer:
				case PhysicalEngines.No:
					PhysicalManager.InitializeEngine(PhysicalEngines.Farseer, new FarseerWorld());
					break;
			}
			Objects = new BodyCollection<Body>();
			Robots = new BodyCollection<Robot>();
			TableObjects = new BodyCollection<Body>();
			switch (Settings.Rules)
			{
				case AvailableRules.Dumb:
					Rules = new DumbRules(this);
					break;
			/*	case AvailableRules.Demo:
					Rules = new DemoRules(this);
					break;*/
				case AvailableRules.ChessUp:
					Rules = new ChessUp.ChessUpRules(this);
					break;
				case AvailableRules.TreasureIsland:
					Rules = new TreasureIsland.TreasureIslandRules(this);
					break;
				default:
					throw new Exception("Rules were not set up in configuration file");
			}
			for(int i = 0; i < settings.Robots.Count; i++)
				if(settings.Robots[i] != null)
					Robots.Add(new Robot(this, i, settings.Robots[i], 
						customAiFactory));

			Rules.InitializeField();
			Add(Robots);
			Rules.AdditionalDefineRobots();
			Add(TableObjects);
			Add(Objects);
			Scores = new ScoreCollection(2);
		}

		private void ResetInternal()
		{
			Objects.Clear();
			foreach(var body in Rules.InitializePieces())
				Objects.Add(body);
			foreach(var robot in Robots)
				robot.Reset();
			Scores.ResetAll();
			Rules.PositionRobots();
		}

		
		
		public void CreateGraphics()
		{
			if (Settings.Drawers.Count == 0)
				Settings.Drawers.Add(new DrawerSettings());
			Func<DrawerSettings, Form> formFactory = x => FormDrawer.CreateForm(x, Scores);
			DrawerFactory = new DrawerFactory();
			Drawers.AddRange(DrawerFactory.CreateForSettingsList(Settings.VideoMode,
				Settings.Drawers,this, formFactory));
			Drawers.Where(x=>x!=null).ForEach(x=>x.Run());
		}
		public readonly List<FormDrawer> Drawers = new List<FormDrawer>();
		public DrawerFactory DrawerFactory { get; private set; }
	}
}

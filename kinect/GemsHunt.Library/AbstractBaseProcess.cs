using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Eurosim.Core;
using Eurosim.Core.Physics.FarseerWrap;
using Eurosim.Core.Replay;
using Eurosim.Graphics;
using Eurosim.Physics;

namespace GemsHunt.Library
{
	public abstract class AbstractBaseProcess
	{
		protected AbstractBaseProcess()
		{
			Root = new Body();
			DrawerFactory=new DrawerFactory(Root);
			ReplayLogger = new ReplayLogger(Root, DT);
			//TODO. Подумать, как избавиться от виртуального метода в конструкторе?
			InitializeBodies(Root);
			InitializePhysics();
			InitializeGraphics();
			MatchEnd += SaveReplay;
		}
		public event Action MatchEnd;

		public void RunInBackgroundThread()
		{
			new Thread(() =>
				{
					while(true)
						MakeCycle(true);
				})
				{
					IsBackground = true
				}.Start();
		}

		public IEnumerable<Robot2013> Robots
		{
			//TODO. Dirty hack.
			get { return Root.GetSubtreeChildrenFirst().Where(x=>x is Robot2013).Cast<Robot2013>(); }
		}
		public IEnumerable<FormDrawer> Drawers { get { return _drawers; } }
		public IEnumerable<Form> Forms { get { return Drawers.Select(x => x.Form); } } 
		public Body Root { get; private set; }
		public double Time { get; private set; }
		public ReplayLogger ReplayLogger { get; private set; }

		public const bool NoPhysics = false;
		public const double DT = 1.0 / 60;
		public const int PhysicalPrecision = 10;
		public const double MatchTimeLimit = 10;

		protected virtual void MakeCycle(bool realtime)
		{
			DateTime cycleBeginning = DateTime.Now;
			if(!NoPhysics)
				MakePhysicsCycle();
			UpdateBodies();
			foreach(var robot in Robots)
				robot.WorkAi();
			MakeReplayCycle();
			OnCycleEnd(realtime, cycleBeginning);
		}

		protected virtual void MakeReplayCycle()
		{
			if(Time < MatchTimeLimit)
				ReplayLogger.LogBodies();
		}

		protected virtual void UpdateBodies()
		{
			//TODO. Нужно ли это? Что вообще это делает?
			foreach(Body body in Root)
				body.Update(DT);
		}

		protected virtual void InitializePhysics()
		{
			PhysicalManager.InitializeEngine(PhysicalEngines.Farseer, new FarseerWorld(), Root);
		}

		protected virtual void InitializeGraphics()
		{
			var drawerSettings = new DrawerSettings
				{
					Robot = Root.GetSubtreeChildrenFirst().FirstOrDefault(x => x is Robot2013)
				};
			_drawers.Add(DrawerFactory.CreateOne(VideoModes.DirectX, Root, drawerSettings, FormDrawer.CreateDefaultEmptyForm));
			foreach(var formDrawer in _drawers)
				formDrawer.Run();
		}

		public readonly DrawerFactory DrawerFactory;

		protected abstract void InitializeBodies(Body root);

		protected virtual void MakePhysicsCycle()
		{
			const double physicalDT = DT / PhysicalPrecision;
			for(int i = 0; i < PhysicalPrecision; i++)
				PhysicalManager.MakeIteration(physicalDT, Root);
		}

		protected virtual void OnMatchEnd()
		{
			if(Time >= MatchTimeLimit && !_endFired && MatchEnd != null)
			{
				_endFired = true;
				MatchEnd();
			}
		}

		protected virtual void SaveReplay()
		{
			ReplayLogger.WriteReplayToFile("log.replay");
		}

		protected virtual void OnCycleEnd(bool realtime, DateTime cycleBeginning)
		{
			double elapsed = 1000 * DT - (DateTime.Now - cycleBeginning).TotalMilliseconds;
			if(realtime && elapsed > 0)
				Thread.Sleep((int)elapsed);
			Time += DT;
			OnMatchEnd();
		}

		private readonly List<FormDrawer> _drawers = new List<FormDrawer>();

		private bool _endFired;
	}
}
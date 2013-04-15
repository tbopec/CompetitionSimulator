using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using AIRLab.Mathematics;
using Eurosim.Core.Physics;
using Eurosim.Core.Physics.BepuWrap;
using Eurosim.Core.Physics.FarseerWrap;
using Eurosim.Graphics.DirectX;
using Eurosim.Physics;

namespace Eurosim.Core
{
	internal class FakeEmulator
	{
		public FakeEmulator(PhysicalEngines physicsMode)
		{
			InitializePhysics(physicsMode);
		}

		public event Action FirstTime;
		public event Action Cycle;

		public void MakeCycle(bool realtime)
		{
			if(_firstTime)
			{
				if(FirstTime != null)
					FirstTime();
				_firstTime = false;
			}
			if(Cycle != null)
				Cycle();
			DateTime begin = DateTime.Now;
			lock(_movementLock)
				foreach(var body in _movements)
					MoveBody(body.Key, Dt);
			MakePhysicsIterationForWholeWorld();
			double elapsed = 1000 * Dt - (DateTime.Now - begin).TotalMilliseconds;
			if(realtime && elapsed > 0)
				Thread.Sleep((int)elapsed);
		}

		public void Add(Body body)
		{
			lock(_movementLock)
				_movements[body] = new ActionQueue<IPlaneMovement>();
			_root.Add(body);
		}

		public void CreateGraphics()
		{
			var scene = new DirectXScene(_root);
			var drawer = new DirectXFormDrawer(scene);
			drawer.Run();
			Form = drawer.Form;
		}

		public void Move(Body body, int distance, Angle angle, int time, Action callback)
		{
			Move(body, new ArcMovement
			           	{
			           		Distance = distance,
			           		Rotation = angle,
			           		TotalTime = time,
			           	}, callback);
		}

		public Form Form { get; private set; }
		public PhysicalEngines PhysicsMode { get; private set; }
		public bool Disposing { get; set; }

		private void Move(Body body, ArcMovement movement, Action callback = null)
		{
			lock(_movementLock)
				_movements[body].Enqueue(movement, callback);
		}

		private void MoveBody(Body body, double dt)
		{
			var offset = new Frame2D();
			IEnumerable<ActionQueueSelection<IPlaneMovement>> movs =
				_movements[body].Dequeue(dt);
			foreach(var m in movs)
			{
				Frame2D dist = m.Action.GetOffset(m.StartTime, m.DTime);
				offset = offset.Apply(dist);
			}
			if(PhysicsMode == PhysicalEngines.No || PhysicalTestBase.BodyIsPhysical(body))
				body.Location = body.Location.Apply(offset.ToFrame3D());
			else
				MoveWithPhysics(body, dt, offset);
		}

		private static void MoveWithPhysics(Body body, double dt, Frame2D offset)
		{
			Frame2D frame = body.Location.ToFrame2D().Apply(offset);
			Frame2D locationdiff = frame - body.Location.ToFrame2D();
			locationdiff.NewA(offset.Angle);
			SetNewVelocityForPhysical(body, (locationdiff / dt).ToFrame3D());
		}

		//TODO. Переписать на новые тела/физику

		private void MakePhysicsIterationForWholeWorld()
		{
			PhysicalManager.MakeIteration(Dt, _root);
		}

		//TODO. Переписать на новые тела/физику

		private static void SetNewVelocityForPhysical(Body body, Frame3D newVelocity)
		{
			throw new NotImplementedException();
			/*IPhysical physicalObject = (body as IPhysicalBody).PhysicalModel;
			physicalObject.Velocity = newVelocity;*/
		}

		//TODO. Переписать на новые тела/физику

		private void InitializePhysics(PhysicalEngines physicsMode)
		{
			PhysicsMode = physicsMode;
			switch(physicsMode)
			{
				case PhysicalEngines.Bepu:
					PhysicalManager.InitializeEngine(PhysicalEngines.Bepu, new BepuWorld(), _root);
					break;
				case PhysicalEngines.Farseer:
				case PhysicalEngines.No:
					PhysicalManager.InitializeEngine(PhysicalEngines.Farseer, new FarseerWorld(), _root);
					break;
			}
		}

		private readonly Dictionary<Body, ActionQueue<IPlaneMovement>> _movements =
			new Dictionary<Body, ActionQueue<IPlaneMovement>>();

		private bool _firstTime = true;

		private readonly Body _root = new Body();

		private readonly object _movementLock = new object();

		private const double Dt = 1.0 / 60;

		#region Old emulator classes

		private class ArcMovement : IPlaneMovement
		{
			public double TotalTime { get; set; }

			public Frame2D GetOffset(double startTime, double dtime)
			{
				double q = dtime / TotalTime;
				return new Frame2D(Distance * q, 0, Rotation * q);
			}

			public Angle Rotation;

			public double Distance;
		}

		private class ActionQueueSelection<T>
		{
			public double DTime { get { return EndTime - StartTime; } }
			public T Action;
			public double StartTime;
			public double EndTime;
		}

		private class ActionQueue<T>
			where T : IPlaneMovement
		{
			public void Enqueue(T action, Action callBack)
			{
				_queue.Enqueue(new ActionQueueElement<T> {Action = action, CallBack = callBack});
			}

			public IEnumerable<ActionQueueSelection<T>> Dequeue(double dtime)
			{
				while(true)
				{
					if(dtime == 0) yield break;
					if(_queue.Count == 0) yield break;
					ActionQueueElement<T> top = _queue.Peek();
					if(_currentElementElapsed + dtime < top.Action.TotalTime)
					{
						_currentElementElapsed += dtime;
						yield return new ActionQueueSelection<T>
						             	{
						             		Action = top.Action,
						             		StartTime = _currentElementElapsed - dtime,
						             		EndTime = _currentElementElapsed
						             	};
						yield break;
					}
					yield return new ActionQueueSelection<T>
					             	{
					             		Action = top.Action,
					             		StartTime = _currentElementElapsed,
					             		EndTime = top.Action.TotalTime
					             	};
					dtime -= top.Action.TotalTime - _currentElementElapsed;
					_currentElementElapsed = 0;
					if(top.CallBack != null)
						top.CallBack();
					_queue.Dequeue();
				}
			}

			private readonly Queue<ActionQueueElement<T>> _queue = new Queue<ActionQueueElement<T>>();
			private double _currentElementElapsed;
		}

		private class ActionQueueElement<T>
			where T : IPlaneMovement
		{
			public T Action;
			public Action CallBack;
		}

		private interface IPlaneMovement
		{
			Frame2D GetOffset(double startTime, double dtime);
			double TotalTime { get; }
		}

		#endregion
	}
}
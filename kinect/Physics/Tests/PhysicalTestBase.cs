using System;
using System.Collections.Generic;
using System.Threading;
using AIRLab.Mathematics;
using Eurosim.Graphics;
using Eurosim.Physics;
using NUnit.Framework;

namespace Eurosim.Core
{
	internal class PhysicalTestBase
	{
		protected void Initialize(PhysicalEngines physicalMode, bool showGraphicsForDebug)
		{
			_fakeEmulator = new FakeEmulator(physicalMode);
			_fakeEmulator.FirstTime += () =>
				{
					_sync.Set();
					Console.WriteLine("Fake emulator started.");
				};
			if(showGraphicsForDebug)
				_fakeEmulator.CreateGraphics();
			bool realtime = !showGraphicsForDebug;
			_thread = new Thread(() =>
				{
					while(!_fakeEmulator.Disposing)
					{
						try
						{
							_fakeEmulator.MakeCycle(realtime);
						}
						catch
						{
							_sync.Set();
						}
						if(realtime)
							Thread.Sleep(1);
					}
				}) {IsBackground = true};
			_thread.Start();
			_sync.WaitOne();
		}

		protected void Dispose()
		{
			_fakeEmulator.Disposing = true;
			if(_fakeEmulator.Form != null)
				_fakeEmulator.Form.EasyInvoke(x=>x.Close());
			Console.WriteLine("Disposed test fixture.");
			Thread.Sleep(100);
		}

		protected void Add(Body body)
		{
			_fakeEmulator.Add(body);
		}

		protected void MoveAsync(Body body, int distance, Angle angle, int time, Action callback)
		{
			_fakeEmulator.Move(body, distance,angle,time,callback);
		}

		protected void MoveSync(Body body, int distance, Angle angle, int time)
		{
			MoveAsync(body, distance,angle,time, () => _sync.Set());
			Console.WriteLine("Added movement, started waiting for completion");
			_sync.WaitOne();
			Console.WriteLine("Movement complete");
		}

		protected void MoveWithCycleCallback(Body body, int distance, Angle angle, int time, Action callback)
		{
			_fakeEmulator.Cycle += callback;
			MoveSync(body, distance,angle, time);
			_fakeEmulator.Cycle -= callback;
		}

		protected void MoveStraightWhileCheckingLocation(Body body, int distance, int time)
		{
			var initLoc = body.GetAbsoluteLocation();
			Action callback = () => IsOnContinuation(initLoc, body.GetAbsoluteLocation());
			MoveWithCycleCallback(body, distance, Angle.Zero, time, callback);
		}

		protected void CheckAbsoluteLocation(Body body, Frame3D expectedLocation, 
			Action<Frame3D, Frame3D> assertion)
		{
			assertion(expectedLocation,body.GetAbsoluteLocation());
			if (_fakeEmulator.PhysicsMode != PhysicalEngines.No && BodyIsPhysical(body))
				assertion(expectedLocation, GetRealPhysicalLocation(body));
			
			foreach(var child in body.Nested)
				CheckAbsoluteLocation(child, expectedLocation.Apply(child.Location),assertion);
		}

		protected void CheckAbsoluteLocation(Body child, Frame3D expectedLocation)
		{
			CheckAbsoluteLocation(child, expectedLocation,(exp,act)=>TestExtensions.AssertPlaneEquals(exp, act));
		}

		//TODO.Переписать на новую физику/тела
		protected Frame3D GetRealPhysicalLocation(Body body)
		{
			throw new NotImplementedException();
			/*return FarseerConverter.GetFrame(((body as IPhysicalBody).PhysicalModel as FarseerBody).RealBody,Frame3D.Identity);*/
		}

		public static bool BodyIsPhysical(Body body)
		{
			return body.IsMaterial;
		}

		public static bool IsOnContinuation(Frame3D start, Frame3D current, double epsilon = 2)
		{
			var objLoc = new Frame3D(start.X,start.Y, start.Z).Invert().Apply(current).ToFrame2D();
			Console.WriteLine("Diff={0}", objLoc);
			var ang = Angem.Atan2(objLoc.Y, objLoc.X);
			Console.WriteLine("Angle={0}", ang);
			return Math.Abs(start.Yaw.Grad - ang.Grad) < epsilon;
		}
	

		private FakeEmulator _fakeEmulator;
		private Thread _thread;
		private readonly AutoResetEvent _sync = new AutoResetEvent(false);
	}

	[TestFixture]
	public class TestUtilityMethodTests
	{
		public static IEnumerable<TestCaseData> LineContinuationTestCases
		{
			get
			{
				return new[]
					       	{
					       		new TestCaseData(new Frame3D(2,0,0), new Frame3D(10, 0, 0)).Returns(true),
					       		new TestCaseData(Frame3D.DoYaw(Angle.HalfPi), new Frame3D(0, 10, 0)).Returns(true),
					       		new TestCaseData(Frame3D.DoYaw(Angle.FromGrad(45)), new Frame3D(10, 10, 0)).Returns(true),
					       		new TestCaseData(Frame3D.DoYaw(Angle.FromGrad(45)), new Frame3D(10, 5, 0)).Returns(false),
					       		new TestCaseData(new Frame3D(10,20,0,Angle.Zero,Angle.FromGrad(45),Angle.Zero), new Frame3D(20, 30, 0)).Returns(true),
					       		new TestCaseData(new Frame3D(10,20,0,Angle.Zero,Angle.FromGrad(-45),Angle.Zero), new Frame3D(20, 10, 0)).Returns(true),
					       		new TestCaseData(new Frame3D(10,20,0,Angle.Zero,Angle.FromGrad(-45),Angle.Zero), new Frame3D(11, 19, 0)).Returns(true),
					       	};
			}
		}

		[TestCaseSource("LineContinuationTestCases")]
		public bool LineContinuation(Frame3D start, Frame3D current)
		{
			return PhysicalTestBase.IsOnContinuation(start, current);
		}
	}

	public static class TestExtensions
	{
		public static Frame3D AddX(this Frame3D frame3D, double x)
		{
			return frame3D.NewX(frame3D.X + x);
		}

		public static void AssertPlaneEquals(Frame3D expected, Frame3D actual, double epsilon = 5)
		{
			Assert.That(Angem.Hypot(actual.ToFrame2D() - expected.ToFrame2D()) < epsilon,
			            "Expected within {1} of {2}, but was {0}", actual, epsilon, expected);
		}
	}
}
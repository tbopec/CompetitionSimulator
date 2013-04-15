using System.Collections.Generic;
using System.Threading;
using AIRLab.Mathematics;
using Eurosim.Core.Physics;
using Eurosim.Physics;
using NUnit.Framework;
using System.Linq;

namespace Eurosim.Core
{
	internal class PhysicalTests1 : PhysicalTestBase
	{
		[SetUp]
		public void SetUp()
		{
			Initialize(PhysicalEngines.Farseer, false);
		}

		[TearDown]
		public void TearDown()
		{
			Dispose();
		}

		[Test]
		public void CollisionWithWall()
		{
			Body robotbox = TestBodyFactory.CreateRobotBox();
			Add(robotbox);
			Body brickwall = TestBodyFactory.CreateWall();
			Add(brickwall);
			bool collisionOccured = false;
			robotbox.Collision +=
				collided => { collisionOccured = CheckCollision(robotbox, collided, brickwall); };
			MoveSync(robotbox, 92, Angle.Zero, 3);
			Thread.Sleep(100);
			Assert.That(collisionOccured);
		}

		[Test]
		public void SimpleMovementsSimpleBody()
		{
			var robotbox = TestBodyFactory.CreateRobotBox();
			var smallBall = TestBodyFactory.CreateSmallPhysicalBall(new Frame3D(10, 0, 0));
			robotbox.Add(smallBall);
			Add(robotbox);
			CheckAbsoluteLocation(robotbox, Frame3D.Identity);
			MoveAlongSquare(robotbox);
		}

		[Test]
		public void SimpleAttachingComplexBody()
		{
			var robotbox = TestBodyFactory.CreateRobotBox();
			var smallBall = TestBodyFactory.CreateSimplestJoinedBody(new Frame3D(10, 0, 0));
			robotbox.Add(smallBall);
			Add(robotbox);
			CheckAbsoluteLocation(robotbox, Frame3D.Identity);
			MoveAlongSquare(robotbox);
		}


		private void MoveAlongSquare(Body robotbox)
		{
			for(int i = 0; i < 4; i++)
			{
				MoveStraightWhileCheckingLocation(robotbox, 30, 1);
				MoveSync(robotbox, 0, Angle.HalfPi, 1);
				//Thread.Sleep(2000);
			}
			CheckAbsoluteLocation(robotbox, Frame3D.Identity);
		}

		public static IEnumerable<TestCaseData> CreateCollectionTestSource
		{
			get
			{
				return new[]
				       	{
				       		Frame3D.Identity, 
							Frame3D.DoYaw(Angle.HalfPi),
							new Frame3D(100, -23, 0),
							new Frame3D(-3, 56, 0)
				       	}.Select(x => new TestCaseData(x));
			}
		}

		[TestCaseSource("CreateCollectionTestSource")]
		public void CreateCollection(Frame3D collectionLoc)
		{
			Body bodyCollection = TestBodyFactory.CreateEmptyCollection(collectionLoc);
			var childLocs = new[]
			                	{
			                		new Frame3D(10, 0, 0),
			                		new Frame3D(0, 10, 0),
			                		new Frame3D(5, 5, 5)
			                	};
			foreach(Frame3D loc in childLocs)
				bodyCollection.Add(TestBodyFactory.CreateSmallPhysicalBall(loc));
			Add(bodyCollection);
			Thread.Sleep(10);
			CheckAbsoluteLocation(bodyCollection, collectionLoc);
			foreach(Body child in bodyCollection)
				CheckAbsoluteLocation(child, bodyCollection.Location.Apply(child.Location));
		}

		

		private bool CheckCollision(Body eventSender, Body collided, Body brickwall)
		{
			Assert.AreEqual(brickwall, collided);
			Frame3D expectedRobotLocation = brickwall.GetAbsoluteLocation()
				.AddX(-(GetXSizeIfBodyIsBox(brickwall) + GetXSizeIfBodyIsBox(eventSender)) / 2);
			CheckAbsoluteLocation(eventSender, expectedRobotLocation);
			return true;
		}

		private static double GetXSizeIfBodyIsBox(Body body)
		{
			return (body as Box).XSize;
		}
	}
}
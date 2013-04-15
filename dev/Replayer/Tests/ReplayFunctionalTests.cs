using System;
using System.Collections.Generic;
using System.Linq;
using AIRLab.Mathematics;
using Eurosim.Core;
using Eurosim.Core.Replay;
using NUnit.Framework;

namespace EurosimReplayer
{
	internal class ReplayFunctionalTests
	{

		[Test]
		public void ExtraUpdatesAfterEnd()
		{
			Assert.DoesNotThrow(() =>
				{
					var replaylogger = new ReplayLogger(new Body(), DT);
					//replaylogger.LogBodies();
					var replayPlayer = new ReplayPlayer(replaylogger.GetReplayString());
					for(int i = 0; i < 10; i++)
						replayPlayer.Update();
					Console.WriteLine(replayPlayer.RootBody);
					Console.WriteLine(replayPlayer.Scores);
				});
		}

		[Test]
		public void EndFlagWithLongReplay()
		{
			const int iterationCount = 10;
			var logger = new ReplayLogger(new Body(), DT);
			for(int i = 0; i < iterationCount; i++)
				logger.LogBodies();
			var player=new ReplayPlayer(logger.GetReplayString());
			for(int i = 0; i < iterationCount; i++)
			{
				Assert.That(!player.IsAtEnd);
				player.Update();	
			}
			for(int i = 0; i < iterationCount; i++)
				Assert.That(player.IsAtEnd);	
		}

		[Test]
		public void EndFlagWithEmptyReplay()
		{
			var player = new ReplayPlayer(new ReplayLogger(new Body(), DT).
											GetReplayString());
			Assert.That(!player.IsAtEnd);
			player.Update();
			Assert.That(player.IsAtEnd);
		}

		[Test]
		public void SimpleTree()
		{
			var root = new Body {new Box(), new Ball(), new Cylinder()};
			root.Name = "Root";
			var newRoot = WriteAndGetRootBody(root);
			Assert.AreEqual(3, newRoot.Nested.Count());
		}

		[Test]
		public void ComplexTree()
		{
			var root = MakeBigTree();
			var replayedRoot = WriteAndGetRootBody(root);
			Assert.AreEqual(100, replayedRoot.Nested.Count());
		}

		private static Body MakeBigTree()
		{
			var root = new Body();
			Body leaf = root;
			for(int i = 0; i < 100; i++)
			{
				var body = new Box();
				if(i % 2 == 0)
					root.Add(body);
				else
				{
					leaf.Add(body);
					leaf = body;
				}
			}
			return root;
		}

		[Test]
		public void Movement()
		{
			var root = new Body();
			Body ball = new Ball {Radius = 5};
			root.Add(ball);
			var replaylogger = new ReplayLogger(root,DT);
			var locations = new List<Frame3D>();
			for(int i = 0; i < 100; i++)
			{
				ball.Location = ball.Location.Apply(new Frame3D(1, 1.5, 0));
				locations.Add(ball.Location);
				replaylogger.LogBodies();
			}
			var replayPlayer = new ReplayPlayer(replaylogger.GetReplayString());
			Body res = replayPlayer.RootBody;
	
			foreach(Frame3D location in locations)
			{
				replayPlayer.UpdateBodies();
				Body loadedBall = res.Nested.Single();
				Assert.AreEqual(location, loadedBall.Location);
			}
		}

		[Test]
		public void Visibility()
		{
			var root = new Body();
			Body ball = new Ball();
			root.Add(ball);
			var replaylogger = new ReplayLogger(root,DT);
			var visibilities = new List<bool>();
			for(int i = 0; i < 5; i++)
			{
				replaylogger.LogBodies();
				var containsBall = root.Nested.Contains(ball);
				visibilities.Add(containsBall);
				if(containsBall)
					root.Remove(ball);
				else root.Add(ball);
			}
			var replayPlayer = new ReplayPlayer(replaylogger.GetReplayString());
			Body replayedResult = replayPlayer.RootBody;
			foreach (var visibility in visibilities)
			{
				replayPlayer.UpdateBodies();
				Assert.AreEqual(visibility, replayedResult.Nested.Count() == 1);
			}
		}

		[Category("SpeedTest")]
		[Ignore]
		[Test]
		public void SavingPerformanceTest()
		{
			var root = MakeBigTree();
			var random = new Random(32781);
			var replayLogger = new ReplayLogger(root, 0.1);
			
			for(int i = 0; i < 1000; i++)
			{
				if (random.Next(3)==0)
					foreach (var child in root.GetSubtreeChildrenFirst())
					{
						if (random.Next(5)==0)
							child.Location = child.Location.Apply(new Frame3D(random.Next(), random.Next(), random.Next()));
					}
				replayLogger.LogBodies();
				replayLogger.LogTempScores(new[] {-1, 1});
			}
			var size = replayLogger.GetReplayString();
			Console.WriteLine("Serialized size: {0}", size.Length);
		}

		private static Body WriteAndGetRootBody(Body root)
		{
			var replaylogger = new ReplayLogger(root, DT);
			replaylogger.LogBodies();
			var replayPlayer1 = new ReplayPlayer(replaylogger.GetReplayString());
			var replayPlayer = replayPlayer1;
			replayPlayer.UpdateBodies();
			return replayPlayer.RootBody;
		}

		private const double DT = 0.1;
	}
}
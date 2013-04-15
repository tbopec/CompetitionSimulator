using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using AIRLab.Mathematics;
using AIRLab.Thornado;
using Eurosim.ChessUp;
using Eurosim.ClientLib;
using Eurosim.Core;
using Eurosim.Graphics;
using Eurosim.Core.Physics;
using Eurosim.Core.Replay;
using NUnit.Framework;

namespace EurosimNetworkServer.Tests
{
	internal class ClientServer_Tests
	{
		[SetUp]
		public void SetUp()
		{
			settings = MakeTestServerSettings();
			server = new EurosimNetworkServer(settings);
			serverThread = new Thread(() => server.Run()) {IsBackground = true};
			serverThread.Start();
			Thread.Sleep(1000); //Cancer.
		}

		[TearDown]
		public void TearDown()
		{
			Console.WriteLine("Started teardown");
			if(client != null)
			{
				client.Dispose();
				client = null;
			}
			server.StopServer();
			Thread.Sleep(1000);
			server.Dispose();
		}

		[Test]
		public void ParsingExceptions()
		{
			var fakeClient = new FakeEurosimClient(new IPEndPoint(IPAddress.Loopback, settings.Port));
			client = fakeClient;
			Assert.Throws<EurosimClientException>(fakeClient.SendMalformedHello);
		}

		[Test]
		public void IncompleteRequest()
		{
			string info;
			Assert.Throws<EurosimClientException>(() => GetClient().Hello(new HelloRequest(), out info));
		}

		[Test]
		public void RequestInterval()
		{
			string info;
			GetClient().Hello(GetValidHello(), out info);
			CheckException<EurosimClientException>(
				() => GetClient().Move(MovementCommand.LineMovement(1, 0.01)),
				e => e.Message.Contains(OnlineBot.RequestIntervalErrorMessage));
		}

		[Test]
		public void ConnectionTimeout()
		{
			string info;
			GetClient().Hello(GetValidHello(), out info);
			Thread.Sleep(OnlineBot.Timeout + OnlineBot.Timeout);
			CheckException<EurosimClientException>(() => GetClient().Move(MovementCommand.LineMovement(0, 1)),
			                                       e => e.Message.Contains("timed out"));
		}
		[Test]
		public void FieldConfiguration()
		{
			string info;
			var client = GetClient();
			var hello = GetValidHello();
			const int configNo = 817;
			hello.Field = configNo;
			client.Hello(hello, out info);
			Assert.AreEqual(configNo,server.Emulator.Settings.PlayConfiguration);
			client.End();
		}

		[Test]
		public void ValidReplaySaved()
		{
			const string replayname = "replay.txt";
			client = new EurosimClient(new IPEndPoint(IPAddress.Loopback, settings.Port), replayname);
			string info;
			client.Hello(GetValidHello(), out info);
			client.End();
			int bodyCount = server.Emulator.GetSubtreeChildrenFirst().OfType<PrimitiveBody>().Count();
			Assert.That(File.Exists(replayname));
			var replayPlayer = new ReplayPlayer();
			var rootBody = new BodyCollection<Body>();
			replayPlayer.Construct(replayname, ref rootBody);
			for(int i = 0; i < 100; i++)
				replayPlayer.UpdateBodies(replayPlayer.DT);
			Assert.AreEqual(bodyCount, rootBody.GetSubtreeChildrenFirst().OfType<PrimitiveBody>().Count());
		}

		[Test]
		public void MovementAndNavigatorData()
		{
			string info;
			EurosimClient client = GetClient();
			client.Hello(GetValidHello(), out info);
			Robot playerRobot = server.Emulator.Robots.ElementAt((int)GetValidHello().Position);
			server.Emulator.Objects.Clear();
			Frame3D initialLocation = playerRobot.Location;
			try
			{
				string serializedSensorData = client.Move(MovementCommand.LineMovement(100));
				Frame3D newLocation = initialLocation.Apply(new Frame3D(100, 0, 0));
				CheckLocation(newLocation, playerRobot.Location);
				CheckNavigatorData(serializedSensorData, playerRobot.Location);
				client.Move(MovementCommand.AngleRotation(-90));
				serializedSensorData = client.Move(MovementCommand.LineMovement(30));
				newLocation = newLocation.Apply(new Frame3D(0, -30, 0));
				CheckLocation(newLocation, playerRobot.Location);
				CheckNavigatorData(serializedSensorData, playerRobot.Location);
				client.Move(MovementCommand.AngleRotation(180));
				serializedSensorData = client.Move(MovementCommand.LineMovement(30));
				newLocation = initialLocation.Apply(new Frame3D(100, 0, 0));
				CheckLocation(newLocation, playerRobot.Location);
				CheckNavigatorData(serializedSensorData, playerRobot.Location);
				client.Move(MovementCommand.AngleRotation(90));
				serializedSensorData = client.Move(MovementCommand.LineMovement(100));
				CheckLocation(initialLocation, playerRobot.Location);
				CheckNavigatorData(serializedSensorData, playerRobot.Location);
			}
			finally
			{
				client.End();
			}
		}

		[Ignore]
		//[Test]
		public void DifferentActuators()
		{
		
		}

		[Ignore]
		//[Test]
		public void ValidMagicEyeData()
		{
		
		}

		private static void CheckNavigatorData(string serializedSensorData, Frame3D actualLocation)
		{
			var receivedInfo = GetNavigatorInfo(serializedSensorData);
			const double eps = 1;
			Assert.That(Math.Abs(receivedInfo.Location.X - actualLocation.X) < eps,
			            "Expected {0}, but was actually {1}", actualLocation, receivedInfo);
			Assert.That(Math.Abs(receivedInfo.Location.Y - actualLocation.Y) < eps,
			            "Expected {0}, but was actually {1}", actualLocation, receivedInfo);
			Assert.That(Math.Abs((receivedInfo.Location.Angle - actualLocation.Yaw).Grad) < eps,
			            "Expected {0}, but was actually {1}", actualLocation, receivedInfo);
		}

		private static NavigatorData GetNavigatorInfo(string serializedSensorData)
		{
			return IO.XML.ParseString<ACMSensorInfo>(serializedSensorData).NavigatorInfo.First();
		}

		private static void CheckLocation(Frame3D expectedLocation, Frame3D robotLocation)
		{
			Assert.That(Angem.Hypot((expectedLocation - robotLocation).ToPoint3D()) < 10,
			            "Expected {0}, but was actually {1}", expectedLocation, robotLocation);
		}

		private static void CheckException<T>(Action testDelegate, Predicate<Exception> exceptionCondition = null)
		{
			try
			{
				testDelegate();
				Assert.Fail("Did not throw expected exception");
			}
			catch(Exception ex)
			{
				Assert.AreEqual(typeof(T), ex.GetType());
				if(exceptionCondition != null)
					Assert.That(exceptionCondition(ex));
			}
		}

		private EurosimClient GetClient()
		{
			return client ?? (client = new EurosimClient(new IPEndPoint(IPAddress.Loopback, settings.Port), "1.txt"));
		}

		private static HelloRequest GetValidHello()
		{
			return new HelloRequest
			       	{
			       		Affiliation = "a",
			       		City = "a",
			       		Country = "a",
			       		Email = "a@bc.com",
			       		Name = "a",
			       		Position = Position.Left
			       	};
		}

		private static EurosimServerSettings MakeTestServerSettings()
		{
			var emulatorSettings = new EmulatorSettings
			                       	{
			                       		Rules = AvailableRules.ChessUp,
			                       		VideoMode = VideoModes.No,
			                       		PhysicsMode = PhysicalEngines.No,
			                       		TimeLimit = 90,
			                       	};
			return new EurosimServerSettings
			       	{
			       		Port = 9876,
			       		Emulator = emulatorSettings,
			       		PlayerRobot = new RobotSettings
			       		              	{
			       		              		Navigators = new List<EmulatedNavigatorSettings> {new EmulatedNavigatorSettings()},
			       		              		MagicEyes = new List<MagicEyeSettings> {new MagicEyeSettings {Radius = 100}},
			       		              		Actuators = new List<ActuatorSettings> {new ChessUpActuatorSettings {HasModel = true, ActionAngle = Angle.Pi, ActionDistance = 50}}
			       		              	}
			       	};
		}

		private EurosimNetworkServer server;
		private EurosimServerSettings settings;
		private Thread serverThread;
		private EurosimClient client;
	}

	internal class FakeEurosimClient : EurosimClient
	{
		public FakeEurosimClient(EndPoint serverAdress)
			: base(serverAdress, "1.txt")
		{
		}

		public void SendMalformedHello()
		{
			Send("<HelloEurosim"
			     + "<Name>{0}</Name>" +
			     "<Affiliation>{1}</Affiliation>" +
			     "<Email>{2}</Email>" +
			     "<City>{3}</City>" +
			     "<Position>{4}</Position>" +
			     "<Opponent>{5}</Opponent>" +
			     "</HelloEurosim>");
			Receive();
		}
	}
}
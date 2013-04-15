using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Reflection;
using AIRLab.Thornado;
using Eurosim.ClientLib;
using Eurosim.Core;
using Eurosim.Core.Replay;
using RoboCoP.Plus;

namespace EurosimNetworkServer
{
	internal class EurosimNetworkServer : IDisposable
	{
		public EurosimNetworkServer(EurosimServerSettings serverSettings)
		{
			this.serverSettings = serverSettings;
			logger = new Logger(this,DefaultLogPath);
			ConsoleLogInfo("Parsed server settings");
		}

		public EurosimNetworkServer(string[] args)
		{
			new ActuatorSettings();//ну врот мне ноги. без этого Торнадо не резолвит классы из Eurosim.Core.
			stopwatch.Restart();
			commonEnv = new ServiceAppEnvironment("Eurosim", args);
			serverSettings = IO.INI.ParseString<EurosimServerSettings>
				(commonEnv.CfgFileEntry, "EurosimNetworkServer");
			logger = new Logger(this, commonEnv.LocalPath(DefaultLogPath));
			stopwatch.Stop();
			ConsoleLogInfo("Parsed server settings in {0} ms", stopwatch.ElapsedMilliseconds);
		}

		public void Dispose()
		{
			NetworkInterface.Dispose();
			logger.Dispose();
		}

		public void Run()
		{
			NetworkInterface = new NetworkInterface(this, serverSettings.Port, logger);
			NetworkServerHelloRequest helloRq;
			if(!NetworkInterface.ValidHelloRequestReceived(out helloRq))
				return;
			SetupUsingHelloRequest(helloRq);
			var helloReply = new NetworkServerHelloReply
			                 	{
			                 		Position = helloRq.Position,
			                 		SessionNumber = logger.Log.SessionNumber
			                 	};
			NetworkInterface.HelloReply(helloReply);
			ai = new OnlineBot(NetworkInterface, logger);
			ConsoleLogInfo("Creating emulator...");
			stopwatch.Restart();
			Emulator = new Emulator(serverSettings.Emulator, a => (a == "Online" ? ai : null))
			           	{
			           		App = commonEnv
			           	};
			Emulator.FirstTimeStarted += () =>
				{
					stopwatch.Stop();
					ConsoleLogInfo("Emulator start time: {0} ms", stopwatch.ElapsedMilliseconds);
				};
			Emulator.CreateGraphics();

			ReplayLogger.SetDT(Emulator.DT);
			while(Emulator.LocalTime < Emulator.Settings.TimeLimit && !stopFlag)
			{
				Emulator.MakeCycle(false);
				ReplayLogger.LogObjects(Emulator.DT, Emulator);
				ReplayLogger.LogScore(Emulator.DT, Emulator.Scores.TempSum);
			}
			ConsoleLogInfo("Finished main loop. Sending replay");
			ReplayLogger.LogPenalties(Emulator.Scores.Penalties);
			NetworkInterface.SendFeedback(ReplayLogger.SerializeToString());
			ConsoleLogInfo("Sent replay");
		}

		public static void Main(string[] args)
		{
			EurosimNetworkServer server = null;
			try
			{
				server = new EurosimNetworkServer(args);
				server.Run();
			}
			catch(SocketException ex)
			{
				server.logger.LogError(ex.Message);
				Console.WriteLine("ERROR: " + ex.Message);
			
			}
				catch(Exception ex)
				{
					if (ex is ReflectionTypeLoadException)
					{
						var loaEx = ex as ReflectionTypeLoadException;
						foreach(var loaderException in loaEx.LoaderExceptions)
						{
							Console.WriteLine(loaderException);
							Console.WriteLine(loaderException.Message);
						}
					}
				}
			finally
			{
				if (server!=null)
					server.Dispose();
			}
		}

		public void StopServer()
		{
			stopFlag = true;
		}

		public static void ConsoleLogInfo(string format, params object[] args)
		{
			Console.WriteLine(format, args);
		}

		private void SetupUsingHelloRequest(NetworkServerHelloRequest helloRq)
		{
			if(helloRq.Position == Position.Random)
				helloRq.Position = rnd.Next(2) == 0 ? Position.Left : Position.Right;
			if(helloRq.Field == -1)
				helloRq.Field = rnd.Next(1800);
			serverSettings.Emulator.PlayConfiguration = helloRq.Field;
			for(int i = 0; i < 2; i++)
			{
				var rs = new RobotSettings();
				if(i == (int)helloRq.Position)
				{
					rs = serverSettings.PlayerRobot;
					rs.AI = "Online";
				}
				else
					rs.AI = helloRq.Opponent.ToString();
				serverSettings.Emulator.Robots.Add(rs);
			}
		}

		public Emulator Emulator { get; private set; }
		public NetworkInterface NetworkInterface { get; private set; }
		private readonly EurosimServerSettings serverSettings;

		private readonly Logger logger;
		private readonly Random rnd = new Random();
		private readonly ServiceAppEnvironment commonEnv;
		private bool stopFlag;
		private OnlineBot ai;
		private readonly Stopwatch stopwatch  = new Stopwatch();
		private const string DefaultLogPath = "logs";
	}
}
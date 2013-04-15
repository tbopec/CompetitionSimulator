using System;
using System.IO;
using System.Net.Sockets;
using AIRLab.Mathematics;
using AIRLab.Thornado;
using Eurosim.Core;
using GemsHunt.Library;

namespace GemsHunt.Server
{
	public class Process : AbstractBaseProcess
	{
		public static void Main(string[] args)
		{
			Instance = new Process();
			Instance.RunInBackgroundThread();
			NetworkServer();
		}

		protected override void InitializeBodies(Body root)
		{
			CurrentWorld = new World();
			CurrentWorld.FillRoot();
			root.Add(CurrentWorld);
		}
		public static void NetworkServer()
		{
			StreamWriter streamWriter;
			StreamReader streamReader;
			NetworkStream networkStream;
			var tcpListener = new TcpListener(5555);
			tcpListener.Start();
			Console.WriteLine("The Server has started on port 5555");
			Socket serverSocket = tcpListener.AcceptSocket();
			try
			{
				if(serverSocket.Connected)
				{
					while(true)
					{
						//MainBody.Velocity = new Frame3D(200, 0, 0);
						Console.WriteLine("Client connected");
						networkStream = new NetworkStream(serverSocket);
						streamWriter = new StreamWriter(networkStream);
						streamReader = new StreamReader(networkStream);
						string str = streamReader.ReadLine();
						Console.WriteLine(str);
						Console.WriteLine("\n\nTTT\n\n");
						var str2 = IO.XML.ParseString<string>(str);
						//string str2 = "w";
						//MainBody.Velocity = new Frame3D(200, 0, 0);
						var controlledRobot = Instance.CurrentWorld.RobotLeft;
						switch(str2[0])
						{
							case 'w':
								controlledRobot.Velocity = new Frame3D(100, 0, 0);
								break;
							case 'a':
								controlledRobot.Velocity = new Frame3D(0, 100, 0);
								break;
							case 's':
								controlledRobot.Velocity = new Frame3D(-100, 0, 0);
								break;
							case 'd':
								controlledRobot.Velocity = new Frame3D(0, -100, 0);
								break;
							case 'g':
								controlledRobot.AddCommand("grip");
								break;
							case 'r':
								controlledRobot.AddCommand("release");
								break;
						}
						streamWriter.WriteLine(controlledRobot.Location.X);
						streamWriter.Flush();
						Console.WriteLine("ok");
						//serverSocket.Close();
					}
				}
				serverSocket.Close();
				// Console.Read();
			}
			catch(SocketException ex)
			{
				serverSocket.Close();
				//Console.WriteLine(ex);
			}
		}

		public static Process Instance { get; private set; }
		public World CurrentWorld;

		
	}
}
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using AIRLab.Mathematics;
using AIRLab.Thornado;
using Eurosim.Core;
using GemsHunt.Library;
using System.Threading;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace GemsHunt.Server
{
	public class Process : AbstractBaseProcess
	{

        public static Process Instance { get; private set; }
        public World CurrentWorld;

		public static void Main(string[] args)
		{
			Instance = new Process();
			Instance.RunInBackgroundThread();

            int MaxThreadsCount = Environment.ProcessorCount * 4;
            ThreadPool.SetMaxThreads(MaxThreadsCount, MaxThreadsCount);
            ThreadPool.SetMinThreads(2, 2);
            new Server(600);
		}

		protected override void InitializeBodies(Body root)
		{
			CurrentWorld = new World();
			CurrentWorld.FillRoot();
			root.Add(CurrentWorld);
		}


        class ClientRequest
        {
            [Thornado]
            public string Team = null;
            [Thornado]
            public Command Command = null;

        }
        class Command
        {
            [Thornado]
            public double Move = 0;
            [Thornado]
            public double Angle = 0;
            [Thornado]
            public bool Grip = false;
            [Thornado]
            public bool Release = false;

        }



        class ClientResponse
        {
            [Thornado]
            public string Team;
            [Thornado]
            public Sensors Sensors;

        }

        class Sensors
        {
            [Thornado]
            public Position Position;
        }

        class Position
        {
            [Thornado]
            public double x;
            [Thornado]
            public double y;
            [Thornado]
            public double angle;
        }



        struct State
        {
           public bool Lock;
           public bool Ready;
           public bool Run;
        };

        class Server
        {
            TcpListener Listener;

            public static State RobotLeft = new State { Lock = false, Ready =  false, Run = false };
            public static State RobotRight = new State { Lock = false, Ready =  false, Run = false };

            public static Stopwatch stopWatch = new Stopwatch();
            //public static bool StartTime = false;
            public const double dtime = 500;
            
            public Server(int Port)
            {
                Listener = new TcpListener(IPAddress.Any, Port);
                Listener.Start();

                while (true)
                {
                    ThreadPool.QueueUserWorkItem(new WaitCallback(ClientThread), Listener.AcceptTcpClient());    
                }
            }

            static void ClientThread(Object StateInfo)
            {
                new Client((TcpClient)StateInfo);
            }

            ~Server()
            {
                if (Listener != null)
                {
                    Listener.Stop();
                }
            }

        }

        class Client
        {
            public Client(TcpClient Client)
            {
                bool RobotLeft = false;
                bool RobotRight = false;
                bool team = false;

                try
                {
                    NetworkStream networkStream = Client.GetStream();
                    StreamReader streamReader = new StreamReader(networkStream);
                    StreamWriter streamWriter = new StreamWriter(networkStream);

                    var controlledRobot = Instance.CurrentWorld.RobotLeft;

                    while (true)
                    {

                        string Request = streamReader.ReadLine();
                        //Console.WriteLine(Request);

                        var ParseRequest = IO.XML.ParseString<ClientRequest>(Request);

                        //var Robot = Server.RobotLeft;
                        
                        if (team == false)
                        {
                            team = true;                            

                            switch (ParseRequest.Team)
                            {
                                case "Left":
                                    if (!Server.RobotLeft.Lock)
                                    {
                                        RobotLeft = true;
                                        Server.RobotLeft.Lock = true;
                                        controlledRobot = Instance.CurrentWorld.RobotLeft;
                                        //Robot = Server.RobotLeft;
                                    }
                                    else
                                    {
                                        streamWriter.WriteLine("Такой игрок уже играет");
                                        throw new System.Exception("Такой игрок уже играет");
                                    }
                                    break;
                                case "Right":

                                    if (!Server.RobotRight.Lock)
                                    {
                                        RobotRight = true;
                                        Server.RobotRight.Lock = true;
                                        controlledRobot = Instance.CurrentWorld.RobotRight;
                                        //Robot = Server.RobotRight;
                                    }
                                    else
                                    {
                                        streamWriter.WriteLine("Такой игрок уже играет");
                                        throw new System.Exception("Такой игрок уже играет");
                                    }
                                    break;
                                default:
                                    throw new System.Exception("Несуществующий игрок");
                            }
                        }

                        if (RobotRight) { Server.RobotRight.Ready = true; Server.RobotRight.Run = false; }
                        if (RobotLeft) { Server.RobotLeft.Ready = true; Server.RobotLeft.Run = false; }
                        
                        while (Server.stopWatch.ElapsedMilliseconds < Server.dtime)
                        {


                            if (Server.RobotLeft.Lock && Server.RobotRight.Lock && !Server.stopWatch.IsRunning)
                            {
                                Server.stopWatch.Start();
                            } 
                            

                              //Robot.Ready = true;

                             //Console.WriteLine(Server.stopWatch.ElapsedMilliseconds.ToString());
                              

                            //если готовы оба - можно не ждать, или не можно?
                            if (Server.RobotRight.Ready && Server.RobotLeft.Ready)
                            {
                                //Console.WriteLine(Server.RobotRight.Ready.ToString());
                                //Console.WriteLine(Server.RobotLeft.Ready.ToString());
                               //break;
                            }

                        }

                        
                        controlledRobot.Move(ParseRequest.Command.Move, ParseRequest.Command.Angle);

                        if (ParseRequest.Command.Grip)
                        {
                            controlledRobot.AddCommand("grip");
                        }

                        if (ParseRequest.Command.Release)
                        {
                            controlledRobot.AddCommand("release");
                        }

                        var resp = new ClientResponse 
                        { 
                            Team = "Left", 
                            Sensors = new Sensors
                            { 
                                Position = new Position 
                                { 
                                    x = controlledRobot.Location.X, 
                                    y = controlledRobot.Location.Y, 
                                    angle = controlledRobot.Location.Yaw.Grad 
                                } 
                            } 
                        };

                        var XMLCommand = IO.XML.WriteToString(resp);
                        streamWriter.WriteLine(XMLCommand);
                        streamWriter.Flush();

                        if (RobotRight) { Server.RobotRight.Run = true; }
                        if (RobotLeft) { Server.RobotLeft.Run = true; }

                       // if (RobotRight) { Console.WriteLine("RobotRight  " + Server.RobotRight.Run.ToString() +" "+ Server.RobotLeft.Run.ToString()); }
                       // if (RobotLeft) { Console.WriteLine("RobotLeft    " + Server.RobotRight.Run.ToString() +" " +Server.RobotLeft.Run.ToString()); }

                        //без этого сбивается почему-то
                        Thread.Sleep(50);

                         if (RobotRight) { Server.RobotRight.Ready = false; Server.RobotRight.Run = false; }
                         if (RobotLeft) { Server.RobotLeft.Ready = false; Server.RobotLeft.Run = false; }

                        lock(Server.stopWatch)
                        {
                            Server.stopWatch.Reset();                       
                        }
                        
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());                    
                }
                finally
                {
                    Client.Close();

                    if (RobotLeft)
                    {
                        Server.RobotLeft.Lock = false;
                        
                    }

                    if (RobotRight)
                    {
                        Server.RobotRight.Lock = false;
                    }
                   
                }

            }
        }

        
		
	}
}
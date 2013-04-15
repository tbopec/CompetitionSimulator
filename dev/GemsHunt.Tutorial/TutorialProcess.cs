using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using AIRLab.Drawing;
using AIRLab.Mathematics;
using AIRLab.Thornado;
using Eurosim.Core;
using Eurosim.Graphics;
using Eurosim.Graphics.DirectX;
using GemsHunt.Library;
using GemsHunt.Library.ClientServer;
using kinect.Integration;

namespace GemsHunt.Tutorial
{
	public class TutorialProcess : AbstractBaseProcess
	{
		public static void Main(string[] args)
		{	
			Instance = new TutorialProcess();
			Instance.RunInBackgroundThread();
            var nw = new NetworkController(Instance.CurrentWorld, Instance.Robots.ElementAt(1), Instance.Robots.ElementAt(0), Instance.DrawerFactory);
            Instance.MainForm.Closing += (sender, eventArgs) => nw.Stop();
            nw.Run(10001);
		}

		public TutorialProcess()
		{
			ScoreCounter = new GemsHuntScoreCounter(Scores,Root);

            var kb = new KeyboardController(MainForm, Robots.ElementAt(1), Robots.ElementAt(0));
		}

		
		protected override void InitializeGraphics()
		{
			var localSync = new ManualResetEventSlim();
			var t = new Thread(() =>
				{
					var frm = new TutorialForm<kinect.Integration.KinectData>
						{
							ScoreDisplayControl = new ScoreDisplayControl
								{
                                  Scores = Scores,
								},
							DrawerControls = new List<DrawerControl>
								{
									new DrawerControl(new DirectXFormDrawer(DrawerFactory.GetDirectXScene(), new DrawerSettings
										{
											BodyCameraLocation = new Frame3D(30,0,30,Angle.FromGrad(-45),Angle.Zero,Angle.Zero),
											Robot = Robots.First(),
											ViewMode = ViewModes.FirstPerson
										})),
									new DrawerControl(new DirectXFormDrawer(DrawerFactory.GetDirectXScene(), new DrawerSettings{ViewMode = ViewModes.Top})),
								},
							BitmapDisplayer = GetBitmapDispayerForRobotCamera(Robots.ElementAt(0))
						};
				    MainForm = frm;
					MainForm.Load += (o, e) => localSync.Set();
					Application.Run(MainForm);
				});
			t.SetApartmentState(ApartmentState.STA);
			t.Start();
			localSync.Wait();
		}

		public static TutorialProcess Instance { get; private set; }

		public World CurrentWorld;
		private BitmapDisplayer<KinectData> GetBitmapDispayerForRobotCamera(Robot2013 robot)
		{
		    Func<Robot2013, Frame3D> getCameraLocation = a =>
		        {
		            var movementDistance = 30;
		            return new Frame3D(a.Location.X + movementDistance * Math.Cos(a.Location.Yaw.Radian), a.Location.Y+movementDistance * Math.Sin(a.Location.Yaw.Radian), a.Location.Z+30, a.Location.Pitch.AddGrad(45), a.Location.Yaw, a.Location.Roll);
		        };
		    var l = new object();
		    var size = 50;
		    var kinect = new Kinect(CurrentWorld, new kinect.Integration.KinectSettings(getCameraLocation(robot), Angle.FromGrad(120), Angle.FromGrad(120/1.35), size,(int)(size/1.35)));
			var bitmapDisplayer = new BitmapDisplayer<KinectData>(
				kinect,
				250,
				data =>
				    {
				        kinect.Location = getCameraLocation(robot);
				        var img = new FastBitmap(data.Depth.GetLength(1), data.Depth.GetLength(0));
				        lock(l)
				        {
				            for(int i = 0; i < img.Width; ++i)
				                for(int j = 0; j < img.Height; ++j)
				                {
                                    var clr = (int)(data.Depth[img.Height - j - 1, img.Width-i-1]);
				                    if(clr > 255) clr = 255;
				                    if(clr < 0) clr = 255;
				                    img[i, j] = Color.FromArgb(clr, clr, clr);
				                }
				        }
                        return img.ToBitmap();
				    });
		    return bitmapDisplayer;
		}

		protected override void InitializeBodies(Body root)
		{	
			CurrentWorld = new World();
			CurrentWorld.FillRoot();
			root.Add(CurrentWorld);
		}
	}

    public class NetworkController
    {
        private readonly World _currentWorld;
        private readonly Robot2013 _left;
        private readonly Robot2013 _right;
        private readonly DrawerFactory _drawerFactory;
        private TcpListener _serv;

        public NetworkController(World currentWorld, Robot2013 left, Robot2013 right, DrawerFactory drawerFactory)
        {
            _currentWorld = currentWorld;
            _left = left;
            _right = right;
            _drawerFactory = drawerFactory;
        }
        private Frame3D GetCameraLocation(Robot2013 a)
        {
            var movementDistance = 30;
            return new Frame3D(a.Location.X + movementDistance * Math.Cos(a.Location.Yaw.Radian), a.Location.Y + movementDistance * Math.Sin(a.Location.Yaw.Radian), a.Location.Z + 30, a.Location.Pitch.AddGrad(45), a.Location.Yaw, a.Location.Roll);   
        }
        public Kinect GetKinect(Robot2013 robot)
        {
            var size = 50;
            Body first = _currentWorld.GetSubtreeChildrenFirst().FirstOrDefault(a => a.Name == "floor");
            return new Kinect(_currentWorld, new KinectSettings(GetCameraLocation(robot), Angle.FromGrad(120), Angle.FromGrad(120 / 1.35), size, (int)(size / 1.35)){Exclude = new []{robot,first}.ToList()});
        }
        
        public void Run(int port)
        {
            _serv = new TcpListener(IPAddress.Any, port);
            _serv.Start();
            try
            {
                var cl1 = _serv.AcceptTcpClient();
                new Thread(() => WorkWithClient(cl1, "Left")).Start();
                var cl2 = _serv.AcceptTcpClient();
                new Thread(() => WorkWithClient(cl2, "Right")).Start();
                Ready = true;
            } catch{}
        }

        private void WorkWithClient(TcpClient cl1, string team)
        {
            var sr = new StreamReader(cl1.GetStream());
            var sw = new StreamWriter(cl1.GetStream());
            
            var robot = team == "Left" ? _left : _right;
            var kinect = GetKinect(robot);
            var cam = new RobotCamera(robot, _drawerFactory, new RobotCameraSettings());
            while(!Ready)
            {
                Thread.Sleep(100);
            }
            Thread.Sleep(3000);
            sw.WriteLine(team);
            sw.Flush();
            while(!Exit && cl1.Connected)
            {
                var input = sr.ReadLine();
                var cmd = IO.XML.ParseString<ClientResponse>(input);
                robot.Move(cmd.Command.Move, cmd.Command.Angle);
                if(cmd.Command.Grip)
                    robot.AddCommand("grip");
                if (cmd.Command.Release)
                    robot.AddCommand("release");
                Thread.Sleep(1000);
                var req = new ClientRequest{Team = team};
                req.Position = robot.GetAbsoluteLocation();
                kinect.Location = req.Position;
                req.Kinect = MakeString(kinect.Measure().Depth);
                req.Camera = MakeStringByte(cam.Measure().Bitmap);
                var res = IO.XML.WriteToString(req);
                sw.WriteLine(res);
                sw.Flush();
            }
            sw.Close();
            sr.Close();
            cl1.Close();
        }

        private string MakeStringByte(byte[] bitmap)
        {
            return string.Join(";", bitmap.Select(a => a.ToString()));
        }

        private string MakeString(double[,] depth)
        {
            var res = new StringBuilder();
            for(int i = 0; i < depth.GetLength(0); ++i)
            {
                for(int j = 0; j < depth.GetLength(1); ++j)
                {
                    res.Append(depth[i, j]);
                    res.Append(";");
                }
                res.Append("|");
            }
            return res.ToString();
        }

        protected bool Exit { get; set; }

        protected bool Ready { get; set; }

        public void Stop()
        {
            Exit = true;
            _serv.Stop();
            Thread.Sleep(1000);
        }
    }
}
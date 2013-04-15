using System.Drawing;
using System.IO;
using System.Linq;
using Eurosim.Core;
using GemsHunt.Library;

namespace GemsHunt.Tutorial
{
	public class TutorialProcess : AbstractBaseProcess
	{
		public TutorialProcess()
		{
			new KeyboardController(Forms.First(), CurrentWorld.RobotRight);
			//Раскомментировать эту строчку, чтобы поднять камеру на роботе
			RunCameraOnRobot(CurrentWorld.RobotRight);
		}

		public static void Main(string[] args)
		{
			Instance = new TutorialProcess();
			Instance.RunInBackgroundThread();
		}

		public static TutorialProcess Instance { get; private set; }

		public World CurrentWorld;

		private void RunCameraOnRobot(Robot2013 robot)
		{
			var robotCamera = new RobotCamera(robot,
											DrawerFactory, new RobotCameraSettings
												{
													WriteToFile = true
												});
			var bitmapDisplayer = new BitmapDisplayer<RobotCameraData>(
				robotCamera,
				100,
				data => Image.FromStream(new MemoryStream(data.Bitmap)));
			bitmapDisplayer.Form.FormClosing += (o, e) => robotCamera.Dispose();
			foreach(var form in Forms)
				form.FormClosing += (o, e) => robotCamera.Dispose();
		}

		protected override void InitializeBodies(Body root)
		{	
			CurrentWorld = new World();
			CurrentWorld.FillRoot();
			root.Add(CurrentWorld);
		}
	}
}
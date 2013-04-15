using System;
using System.IO;
using System.Windows.Forms;
using AIRLab.Mathematics;
using Eurosim.Graphics;
using Eurosim.Graphics.DirectX;
using SlimDX.Direct3D9;

namespace Eurosim.Core
{
	public class RobotCamera : SensorModel<RobotCameraSettings>, IDisposable
	{
		public RobotCamera(Robot robot, RobotCameraSettings settings)
			: base(robot, settings)
		{
			Location = settings.Location;
			Name = "RobotCamera";
			Add(new PrimitiveBody("camera"));
			if (Settings.Compress)
				_imageFormat = ImageFileFormat.Jpg;
			else
				Enum.TryParse(Settings.Format, true, out _imageFormat);

			if (robot.Emulator.Settings.VideoMode!=VideoModes.DirectX)
				Application.ThreadExit += (o, e) => Dispose();
			Angle viewAngle = Settings.ViewAngle;
			if (viewAngle.Grad < 0.1)
				viewAngle = SceneConfig.FirstPersonViewAngle;
			_camera = new FirstPersonCamera(this, Location,
			                                viewAngle, DefaultWidth/(double) DefaultHeight);
			Emulator tempQualifier = Robot.Emulator;
			_drawer = new OffscreenDirectXDrawer(tempQualifier.DrawerFactory.GetDirectXScene(tempQualifier), DefaultWidth,
			                                     DefaultHeight, _imageFormat);
		}

		public void Dispose()
		{
			_drawer.Dispose();
		}

		public override object InternalMeasure()
		{
			var data = new RobotCameraData();
			bool result = _drawer.TryGetImage(_camera, out data.Bitmap);
			if (Settings.WriteToFile && result)
				WriteToFile(data.Bitmap);
			return data;
		}

		public const int DefaultHeight = 600;
		public const int DefaultWidth = 800;

		private void WriteToFile(byte[] bitmap)
		{
			string path = Robot.Emulator.App.LocalPath("test." + _imageFormat);
			File.WriteAllBytes(path, bitmap);
		}

		private readonly ImageFileFormat _imageFormat;
		private readonly OffscreenDirectXDrawer _drawer;
		private readonly FirstPersonCamera _camera;
	}
}
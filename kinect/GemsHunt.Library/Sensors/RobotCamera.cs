using System;
using System.IO;
using AIRLab.Mathematics;
using AIRLab.Thornado;
using Eurosim.Graphics;
using Eurosim.Graphics.DirectX;
using GemsHunt.Library;
using GemsHunt.Library.Sensors;

namespace Eurosim.Core
{
	public class RobotCamera : ISensor<RobotCameraData>, IDisposable
	{
		/// <summary>
		/// Создает камеру
		/// </summary>
		/// <param name="robot">Тело, вместе с которым камера будет перемещаться</param>
		/// <param name="factory">см <see cref="AbstractBaseProcess.DrawerFactory"/> в классе <see cref="AbstractBaseProcess"/></param>
		/// <param name="settings">Настройки камеры</param>
		public RobotCamera(Body robot, DrawerFactory factory, RobotCameraSettings settings)
		{
			Settings = settings;
			/*var cameraBody=new Body
				{
					Model = Model.FromResource(()=>Resources.camera),
					Location=Settings.Location
				};
			robot.Add(cameraBody);*/
			Angle viewAngle = Settings.ViewAngle;
			_camera = new FirstPersonCamera(robot, Settings.Location,
			                                viewAngle, DefaultWidth/(double) DefaultHeight);
			_drawer = new OffscreenDirectXDrawer(factory.GetDirectXScene(), DefaultWidth,
			                                     DefaultHeight);
		}

		public RobotCameraSettings Settings { get; private set; }

		/// <summary>
		/// Освобождает unmanaged ресурсы, используемые камерой.
		/// </summary>
		public void Dispose()
		{
			_drawer.Dispose();
		}

		/// <summary>
		/// Снимает изображение с камеры и возвращает объект с данными камеры. 
		/// </summary>
		/// <returns></returns>
		public RobotCameraData Measure()
		{
			var data = new RobotCameraData();
			bool result = _drawer.TryGetImage(_camera, out data.Bitmap);
			if (Settings.WriteToFile && result)
				WriteToFile(data.Bitmap);
			return data;
		}

		public const int DefaultHeight = 600;
		public const int DefaultWidth = 800;

		private static void WriteToFile(byte[] bitmap)
		{
			const string tempDir = "CameraTestImages";
			if(!Directory.Exists(tempDir))
				Directory.CreateDirectory(tempDir);
			string path = Path.Combine(tempDir,"test.jpg");
			File.WriteAllBytes(path, bitmap);
		}

		private readonly OffscreenDirectXDrawer _drawer;
		private readonly FirstPersonCamera _camera;
	}
	
	[Serializable]
	public class RobotCameraData
	{
		/// <summary>
		/// Массив байтов, содержащий изображение в формате jpeg
		/// </summary>
		public byte[] Bitmap;
	}
	
	[Serializable]
	public class RobotCameraSettings
	{
		/// <summary>
		/// Угол зрения
		/// </summary>
		[Thornado]
		public Angle ViewAngle=Angle.HalfPi;

		/// <summary>
		/// Точка крепления камеры
		/// </summary>
		[Thornado]
		public Frame3D Location = new Frame3D(0, 0, 10, Angle.FromGrad(-25), Angle.Zero, Angle.Zero);

		/// <summary>
		/// Писать в файл для дебага
		/// </summary>
		[Thornado]
		public bool WriteToFile;
	}
}
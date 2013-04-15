using System;
using System.Drawing;
using System.IO;
using System.Threading;
using AIRLab.Mathematics;
using Eurosim.Core;
using Eurosim.Graphics.Properties;
using NUnit.Framework;
using SlimDX.Direct3D9;
using Box = Eurosim.Core.Box;

namespace Eurosim.Graphics.DirectX
{
	[TestFixture]
	internal class OffscreenDrawerTests
	{
		[SetUp]
		public void SetUp()
		{
			SceneConfig.Lights.Clear();
			SceneConfig.Lights.Add(new LightSettings
			                       	{
			                       		Type = LightSettings.MyLightType.Ambient,
			                       		ColorString = "White",
			                       	});
			_scene = new DirectXScene(_rootBody);
			_offscreenDrawer = new OffscreenDirectXDrawer(_scene,
			                                              Width, Height, ImageFormat);
		}

		[TearDown]
		public void TearDown()
		{
			_offscreenDrawer.Dispose();
			_scene.DeviceWorker.TryDispose();
			Thread.Sleep(500);
		}

		[Test]
		public void TestSimpleTopView()
		{
			_rootBody.Clear();
			_rootBody.Add(RedBox);
			var camera = new TopViewCamera(new Frame3D(0, 0, 200), Width / Height);
			byte[] bitmapBytes;
			_offscreenDrawer.TryGetImage(camera, out bitmapBytes);
			var bitmap = new Bitmap(new MemoryStream(bitmapBytes));
			CheckImageSize(Width, Height, bitmap);
			CheckBitmapCenter(bitmap, Color.Red);
		}

		[Test]
		public void TestRotation()
		{
			_rootBody.Clear();
			_rootBody.Add(YellowWall);
			_rootBody.Add(GreenWall);
			var sourceBody = new Body {Model =Model.FromResource(()=>Resources.arrowblue)};
			_rootBody.Add(sourceBody);
			var camera = new FirstPersonCamera(sourceBody, new Frame3D(0, 0, 20), SceneConfig.FirstPersonViewAngle,
			                                   Width / Height);
			byte[] bitmapBytes;
			_offscreenDrawer.TryGetImage(camera, out bitmapBytes);
			SaveImage(bitmapBytes);
			var bitmap = new Bitmap(new MemoryStream(bitmapBytes));
			CheckBitmapCenter(bitmap, Color.Green);
			sourceBody.Location = sourceBody.Location.NewYaw(Angle.Pi);
			Console.WriteLine(sourceBody.Location);
			_offscreenDrawer.TryGetImage(camera, out bitmapBytes);
			SaveImage(bitmapBytes);
			bitmap = new Bitmap(new MemoryStream(bitmapBytes));
			CheckBitmapCenter(bitmap, Color.Yellow);
		}

		private static void SaveImage(byte[] bitmap)
		{
			File.WriteAllBytes(Guid.NewGuid() + "." + ImageFormat, bitmap);
		}

		private static void CheckImageSize(int width, int height, Image image)
		{
			Assert.AreEqual(width, image.Width);
			Assert.AreEqual(height, image.Height);
		}

		private static void CheckBitmapCenter(Bitmap bitmap, Color color)
		{
			Color centerColor = bitmap.GetPixel(bitmap.Width / 2, bitmap.Height / 2);
			Assert.That(GetColorDiff(color, centerColor) < 20, "Expected {0}, but was {1}", color, centerColor);
		}

		private static int GetColorDiff(Color c1, Color c2)
		{
			return Math.Abs(c1.A - c2.A) + Math.Abs(c1.R - c2.R) +
			       Math.Abs(c1.G - c2.G) + Math.Abs(c1.B - c2.B);
		}

		private readonly Body _rootBody = new Body();

		private DirectXScene _scene;
		private OffscreenDirectXDrawer _offscreenDrawer;

		private static readonly Body YellowWall = new Box
		                                          	{
		                                          		XSize = 1,
		                                          		YSize = 100,
		                                          		ZSize = 100,
		                                          		DefaultColor =  Color.Yellow,
		                                          		Location = new Frame3D(-10, 0, 0)
		                                          	};

		private static readonly Body GreenWall = new Box
		                                         	{
		                                         		XSize = 1,
		                                         		YSize = 100,
		                                         		ZSize = 100,
		                                         		DefaultColor = Color.Green,
		                                         		Location = new Frame3D(10, 0, 0)
		                                         	};

		private static readonly Body RedBox = new Box
		                                      	{
		                                      		XSize = 1000,
		                                      		YSize = 1000,
		                                      		ZSize = 1,
		                                      		DefaultColor= Color.Red,
		                                      	};

		private const ImageFileFormat ImageFormat = ImageFileFormat.Jpg;

		private const int Width = 800;
		private const int Height = 600;
	}
}
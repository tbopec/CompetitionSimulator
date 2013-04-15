using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using AIRLab.Mathematics;
using Eurosim.Core;
using Eurosim.Graphics.Properties;
using NUnit.Framework;

namespace Eurosim.Graphics.DirectX
{
	public partial class DirectXFormDrawer
	{
		[TestFixture]
		public class DrawerTests
		{
			[SetUp]
			public void Setup()
			{
				_testScene = new DirectXScene(_rootBody, null);
			}

			[TearDown]
			public void TearDown()
			{
				Console.WriteLine("Teardown started...");
				foreach(DirectXFormDrawer drawer in _drawers)
					drawer.Form.EasyInvoke(form => form.Close());
				Console.WriteLine("Form closed");
				Thread.Sleep(400);
				Console.WriteLine("Teardown completed.");
			}

			[Test]
			public void StaticSize()
			{
				_drawers = new List<DirectXFormDrawer> {CreateDrawer()};
				DirectXFormDrawer drawer1 = _drawers[0];
				int width = drawer1.Form.ClientSize.Width;
				int height = drawer1.Form.ClientSize.Height;
				CheckSwapChainSize(_drawers[0], width, height);
				double aspectRatio = width / (double)height;
				CheckEqual(aspectRatio, drawer1.AspectRatio);
				CheckEqual(aspectRatio, drawer1._camera.AspectRatio);
			}

			[Test]
			public void OneResize()
			{
				_drawers = new List<DirectXFormDrawer> {CreateDrawer()};
				const int width = 1000;
				const int height = 700;
				SetFormSize(_drawers[0], width, height);
				CheckSwapChainSize(_drawers[0], width, height);
				const double aspectRatio = width / (double)height;
				DirectXFormDrawer drawer1 = _drawers[0];
				CheckEqual(aspectRatio, drawer1.AspectRatio);
				CheckEqual(aspectRatio, drawer1._camera.AspectRatio);
			}

			[Test]
			public void ResizeTwice()
			{
				_drawers = new List<DirectXFormDrawer> {CreateDrawer()};
				int width = 1000;
				int height = 700;
				SetFormSize(_drawers[0], width, height);
				CheckSwapChainSize(_drawers[0], width, height);
				double aspectRatio = width / (double)height;
				DirectXFormDrawer drawer1 = _drawers[0];
				CheckEqual(aspectRatio, drawer1.AspectRatio);
				CheckEqual(aspectRatio, drawer1._camera.AspectRatio);
				width = 120;
				height = 70;
				SetFormSize(_drawers[0], width, height);
				CheckSwapChainSize(_drawers[0], width, height);
				CheckDeviceSize(_drawers[0], width, height);
				aspectRatio = width / (double)height;
				CheckEqual(aspectRatio, drawer1.AspectRatio);
				CheckEqual(aspectRatio, drawer1._camera.AspectRatio);
			}

			[Test]
			public void TwoDrawers()
			{
				_drawers = new List<DirectXFormDrawer> {CreateDrawer(), CreateDrawer()};
				SetFormSize(_drawers[0], 900, 600);
				const int smallwidth = 113;
				const int smallheight = 127;
				SetFormSize(_drawers[1], smallwidth, smallheight);
				CheckSwapChainSize(_drawers[1], smallwidth, smallheight);
				CheckSwapChainSize(_drawers[0], 900, 600);
				CheckDeviceSize(_drawers[1], 900, 600);
				SetFormSize(_drawers[1], 1000, 700);
				CheckSwapChainSize(_drawers[1], 1000, 700);
				CheckSwapChainSize(_drawers[0], 900, 600);
				CheckDeviceSize(_drawers[1], 1000, 700);
				const int newSmallSize = 180;
				SetFormSize(_drawers[0], newSmallSize, newSmallSize);
				SetFormSize(_drawers[1], newSmallSize, newSmallSize);
				CheckSwapChainSize(_drawers[1], newSmallSize, newSmallSize);
				CheckSwapChainSize(_drawers[0], newSmallSize, newSmallSize);
				CheckDeviceSize(_drawers[1], newSmallSize, newSmallSize);
				Console.WriteLine("Closing one form");
				_drawers[0].Form.EasyInvoke(x => x.Close());
				//one remaining now.
				SetFormSize(_drawers[1], 900, 600);
				CheckSwapChainSize(_drawers[1], 900, 600);
				CheckDeviceSize(_drawers[1], 900, 600);
				SetFormSize(_drawers[1], smallwidth, smallheight);
				CheckSwapChainSize(_drawers[1], smallwidth, smallheight);
				CheckDeviceSize(_drawers[1], smallwidth, smallheight);
			}

			[Test]
			public void FormMinimized()
			{
				_drawers = new List<DirectXFormDrawer> {CreateDrawer()};
				int width = _drawers[0].Form.ClientSize.Width;
				int height = _drawers[0].Form.ClientSize.Height;
				_drawers[0].Form.WindowState = FormWindowState.Minimized;
				CheckSwapChainSize(_drawers[0], width, height);
			}

			[Test]
			[Category("SpeedTest")]
			[Ignore]
			public void HardPerformanceTest()
			{
				SceneConfig.Framerate = 1000;
				for(int i = 0; i < 30; i++)
				{
					_rootBody.Add(new Box
					              	{
					              		XSize = 10,
					              		YSize = 20,
					              		ZSize = 30,
					              		DefaultColor = Color.Yellow,
					              		Top = PlaneImageBrush.FromResource(() => Resources.untitled),
					              		Bottom = PlaneImageBrush.FromResource(() => Resources.untitled),
					              		Location = GetRandomFrame()
					              	});
					_rootBody.Add(new Body
					              	{
					              		Model = Model.FromResource(() => Resources.totem),
					              		Location = GetRandomFrame()
					              	});
					_rootBody.Add(new Cylinder
					              	{
										RTop = 20,
										RBottom = 30,
										Height = 10,
					              		DefaultColor = Color.Red,
					              		Location = GetRandomFrame()
					              	});
				}
				_drawers = new List<DirectXFormDrawer> {CreateDrawer()};
				Thread.Sleep(TimeSpan.FromSeconds(20));
			}

			private Frame3D GetRandomFrame()
			{
				return new Frame3D(_random.Next(100), _random.Next(100), _random.Next(100));
			}

			private DirectXFormDrawer CreateDrawer()
			{
				var drawer = new DirectXFormDrawer(_testScene,
				                                   _drawerSettings, CreateDefaultEmptyForm);
				drawer.Run();
				return drawer;
			}

			private static void CheckDeviceSize(DirectXFormDrawer drawer, int width, int height)
			{
				Assert.AreEqual(width, drawer._deviceWorker.DeviceSize.Width);
				Assert.AreEqual(height, drawer._deviceWorker.DeviceSize.Height);
			}

			private static void CheckSwapChainSize(DirectXFormDrawer drawer, int width, int height)
			{
				Assert.AreEqual(width, drawer._swapChain.PresentParameters.BackBufferWidth);
				Assert.AreEqual(height, drawer._swapChain.PresentParameters.BackBufferHeight);
			}

			// ReSharper disable UnusedParameter.Local
			private static void CheckEqual(double expected, double actual)
			{

				Assert.That(Math.Abs(expected - actual) < 0.0001, "Expected {0}, but was {1}", expected, actual);
			}

			// ReSharper restore UnusedParameter.Local

			private static void SetFormSize(FormDrawer drawer, int width, int height)
			{
				drawer.Form.EasyInvoke(x => x.ClientSize = new Size(width, height));
			}

			private readonly DrawerSettings _drawerSettings = new DrawerSettings();

			private readonly Body _rootBody = new Body
			                                  	{new Ball {Radius = 15, DefaultColor = Color.Red}};

			private List<DirectXFormDrawer> _drawers = new List<DirectXFormDrawer>();
			private DirectXScene _testScene;
			private readonly Random _random=new Random(12345);
		}
	}
}
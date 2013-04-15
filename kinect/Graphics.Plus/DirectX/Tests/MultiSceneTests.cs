using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using Eurosim.Core;
using NUnit.Framework;

namespace Eurosim.Graphics.DirectX
{
	internal class TwoSceneTests
	{
		[SetUp]
		public void SetUp()
		{
			_scenes.Add(
				new DirectXScene(new Ball
				                 	{
				                 		Radius = 10,
				                 		DefaultColor= Color.Yellow
				                 	}));
			_scenes.Add(
				new DirectXScene(new Box
				                 	{
				                 		XSize = 10,
										YSize = 10,
										ZSize = 10, 
										DefaultColor=Color.Red
				                 	}));
			foreach (DirectXScene scene in _scenes)
			{
				var drawer = new DirectXFormDrawer(scene, _drawerSettings,
					FormDrawer.CreateDefaultEmptyForm);
				_drawers.Add(drawer);
				drawer.Run();
			}
		}

		[TearDown]
		public void TearDown()
		{
			foreach (DirectXFormDrawer directXDrawer in _drawers)
				directXDrawer.Form.Close();
			Thread.Sleep(500);
		}

		[Test]
		public void Test1()
		{
			Thread.Sleep(1000);
			Assert.Pass();
		}

		private readonly List<DirectXScene> _scenes = new List<DirectXScene>();
		private readonly List<DirectXFormDrawer> _drawers = new List<DirectXFormDrawer>();
		private readonly DrawerSettings _drawerSettings = new DrawerSettings();
	}
}
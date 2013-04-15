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
				new DirectXScene(new BodyCollection<Body>
				                 {
				                 	new PrimitiveBody(new BallShape(10), Color.Yellow)
				                 }, null));
			_scenes.Add(
				new DirectXScene(new BodyCollection<Body>
				                 {
				                 	new PrimitiveBody(new BoxShape(10, 10, 10), Color.Red)
				                 }, null));
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
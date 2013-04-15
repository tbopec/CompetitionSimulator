using System;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using AIRLab.Mathematics;
using Eurosim.Core;
using NUnit.Framework;

namespace Eurosim.Graphics.DirectX
{
	class CameraTests
	{
		[Test]
		public void ModePlusPlus()
		{
			var camera = new SwitchableCamera(new PrimitiveBody(new BoxShape(1, 1, 1), Color.Yellow), new Frame3D(),
			                                  new System.Windows.Forms.Form(), new Frame3D(100, 200, 300));
			var expectedModes = typeof(ViewModes).
				GetMembers(BindingFlags.Public | BindingFlags.Static).
				Select(x=>(ViewModes)Enum.Parse(typeof(ViewModes),x.Name))
				.ToList();
			var firstMode = camera.Mode;
			var modeCountAtStart = expectedModes.Count;
			for(int i = 0; i < modeCountAtStart; i++)
			{
				Console.WriteLine("Mode is {0}",camera.Mode);
				expectedModes.Remove(camera.Mode);
				camera.Mode++;
			}
			Assert.AreEqual(0,expectedModes.Count);
			Assert.AreEqual(firstMode,camera.Mode);

		}

	}
}

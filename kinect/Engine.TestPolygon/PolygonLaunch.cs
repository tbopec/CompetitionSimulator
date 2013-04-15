using System.Threading;
using Eurosim.Core.Physics;
using Eurosim.Physics;
using EurosimStandalone;

namespace Eurosim.Engine.TestPolygon
{
	class PolygonLaunch
	{
		public static void Main(string[] args)
		{

			var polygon = new TestPolygon(new TestPolygonSettings() {PhysicsMode = PhysicalEngines.Farseer});

			var emulatorThread = new Thread(() => { while (true) polygon.MakeCycle(true); })
			{
				Name = "polygon",
				IsBackground = true
			};
			emulatorThread.Start();
			polygon.CreateDrawer();


			var emulatorForm = polygon.Drawers[0].Form;
			new KeyboardController(polygon, emulatorForm);			
		}
	}
}

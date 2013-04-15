using System.Threading;
using Eurosim.Physics;

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
			polygon.CreateGraphics();
		
		}
	}
}

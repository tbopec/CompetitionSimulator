using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using AIRLab.Mathematics;

namespace Eurosim.Core
{
	public class DumbRules : Rules
	{
		public DumbRules(Emulator emulator)
			: base(emulator)
		{
		}

		public override IEnumerable<Body> InitializePieces()
		{
			yield break;
		}

		public override IEnumerable<Body> InitializeTable()
		{
			Floor = new PrimitiveBody(new BoxShape(210, 300, 3), Color.Gray)
			        {
			        	Location = new Frame3D(0, 0, -1.5)
			        };
			return new List<Body> {Floor};
		}

		public override void PositionRobots()
		{
			if(Emulator.Robots.Any())
				Emulator.Robots.First().Location = new Frame3D(0, 0, 15);
		}
	}
}
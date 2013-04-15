using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eurosim.Core.Replay
{
	[Serializable]
	public abstract class Description
	{
		public Description() { }

		public abstract void UpdateBody(PrimitiveBody pb, double dt);

		public abstract bool HasNextChange();
	}
}

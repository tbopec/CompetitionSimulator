using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eurosim.Core.Replay
{
	[Serializable]
	public class Visibility : BodyDescription
	{
		public bool IsVisible = false;

		public Visibility(bool isVisible, double startTime)
		{
			IsVisible = isVisible;
			StartTime = startTime;
			IsSingleAct = true;
		}
	}
}

using System;
using AIRLab.Mathematics;
using AIRLab.Thornado;

namespace Eurosim.Core
{
	[Serializable]
	public class NavigatorData
	{
		[Thornado]
		public DateTime Time { get; set; }

		[Thornado]
		public Frame2D Location { get; set; }
	}
}
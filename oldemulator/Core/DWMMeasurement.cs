using System;
using System.Collections.Generic;
using AIRLab.Thornado;

namespace Eurosim.Core
{
	public class DWMMeasurementData
	{
		[Thornado]
		public DateTime startTime;

		[Thornado]
		public List<DoubleWheelMovement> Movs = new List<DoubleWheelMovement>();

		[Thornado]
		public List<NavigatorData> Navs = new List<NavigatorData>();
	}
}
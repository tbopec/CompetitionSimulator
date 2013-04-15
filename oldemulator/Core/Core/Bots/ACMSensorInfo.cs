using System;
using System.Collections.Generic;
using System.Linq;
using AIRLab.Thornado;

namespace Eurosim.Core
{
	[Serializable]
	public class ACMSensorInfo
	{
		public static ACMSensorInfo Create(Robot robot)
		{
			var inf = new ACMSensorInfo();
			inf.NavigatorInfo = robot.Navigators.Select(z => z.InternalMeasure()).Cast<NavigatorData>().ToList();
			inf.MagicEyeInfo = robot.MagicEyes.Select(z => z.InternalMeasure()).Cast<MagicEyeData>().ToList();
			inf.ActuatorStates = robot.Actuators.Select(z => z.State).ToList();
			//... и дальше продолжаем для всех остальных сенсоров...
			return inf;
		}

		[Thornado]
		public List<NavigatorData> NavigatorInfo { get; set; }

		[Thornado]
		public List<MagicEyeData> MagicEyeInfo { get; set; }

		[Thornado]
		public List<string> ActuatorStates { get; set; }
	}
}
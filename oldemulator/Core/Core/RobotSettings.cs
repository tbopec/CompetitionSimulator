using System;
using System.Collections.Generic;
using AIRLab.Thornado;
using RoboCoP.Plus.Common;

namespace Eurosim.Core
{
	[Serializable]
	public class RobotSettings
	{
		public RobotSettings()
		{
			Navigators = new List<EmulatedNavigatorSettings>();
			TrivialInput = new ConfirmingServiceSettings();
			Actuators = new List<ActuatorSettings>();
			MagicEyes = new List<MagicEyeSettings>();
			RobotCameras = new List<RobotCameraSettings>();
			DoubleWheelInput = new DoubleWheelMovementSettings();
			ArcInput = new ConfirmingServiceSettings();
		}

		[Thornado]
		public List<EmulatedNavigatorSettings> Navigators { get; set; }

		[Thornado]
		public List<MagicEyeSettings> MagicEyes { get; set; }

		[Thornado]
		public List<RobotCameraSettings> RobotCameras { get; private set; }

		[Thornado]
		public ConfirmingServiceSettings TrivialInput { get; private set; }

		[Thornado]
		public DoubleWheelMovementSettings DoubleWheelInput { get; private set; }

		[Thornado]
		public ConfirmingServiceSettings ArcInput { get; private set; }

		[Thornado]
		public List<ActuatorSettings> Actuators { get; set; }

		[Thornado]
		public string AI { get; set; }
	}
}
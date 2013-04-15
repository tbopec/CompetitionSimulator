using System;
using System.Collections.Generic;
using AIRLab.Thornado;
using Eurosim.Graphics;
using Eurosim.Core.Physics;
using RoboCoP.Plus;

namespace Eurosim.Core
{
	public enum AvailableRules
	{
		Dumb,
		ChessUp,
		TreasureIsland
	}

	[Serializable]
	public class EmulatorSettings : ServiceSettings
	{
		public EmulatorSettings()
		{
			Robots = new List<RobotSettings>();
			Drawers = new List<DrawerSettings>();
			VideoMode = VideoModes.WPF;
			PlayConfiguration = -1;
		}

		[Thornado]
		public PhysicalEngines PhysicsMode { get; set; }

		[Thornado]
		public List<RobotSettings> Robots { get; private set; }

		[Thornado]
		public AvailableRules Rules { get; set; }

		[Thornado]
		public VideoModes VideoMode { get; set; }

		[Thornado]
		public List<DrawerSettings> Drawers { get; private set; }

		[Thornado]
		public double TimeLimit { get; set; }

		[Thornado]
		public int PlayConfiguration { get; set; }
	}
}
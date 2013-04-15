using System;
using System.Collections.Generic;
using AIRLab.Thornado;
using Eurosim.Core;
using Eurosim.Core.Physics;
using Eurosim.Graphics;
using Eurosim.Physics;

namespace Eurosim.Engine.TestPolygon
{
	[Serializable]
	public class TestPolygonSettings
	{
		public TestPolygonSettings()
		{
			Drawers = new List<DrawerSettings>();
			VideoMode = VideoModes.DirectX;
		}

		[Thornado]
		public PhysicalEngines PhysicsMode { get; set; }

		[Thornado]
		public VideoModes VideoMode { get; set; }

		[Thornado]
		public List<DrawerSettings> Drawers { get; private set; }
	}
}
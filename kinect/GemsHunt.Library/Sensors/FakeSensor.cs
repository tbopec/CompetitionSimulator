using System;
using System.Linq;
using AIRLab.Thornado;
using Eurosim.Core;
using GemsHunt.Library;

namespace GemsHunt.Library.Sensors
{
	public class FakeSensor : ISensor<FakeSensorData>
	{
		public FakeSensor(Robot2013 robot, Body worldRoot, FakeSensorSettings settings)
		{
			_robot = robot;
			_worldRoot = worldRoot;
			_settings = settings;
		}

		public FakeSensorData Measure()
		{
			var count = _worldRoot.GetSubtreeChildrenFirst().Count();
			return new FakeSensorData
				{
					Time = DateTime.Now,
					Message = string.Format("Oooh, I see so many pretty bodies! A whole {0} pretty bodies!",count)
				};
		}

		private readonly Robot2013 _robot;
		private readonly Body _worldRoot;
		private readonly FakeSensorSettings _settings;
	}

	public class FakeSensorSettings
	{
	}

	public class FakeSensorData
	{
		[Thornado]
		public string Message;
		[Thornado]
		public DateTime Time;
	}
}
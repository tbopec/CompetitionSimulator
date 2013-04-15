using System.Collections.Generic;
using AIRLab.Mathematics;

namespace Eurosim.Core
{
	public class MagicEye : SensorModel<MagicEyeSettings>
	{
		public MagicEye(Robot robot, MagicEyeSettings settings)
			: base(robot, settings)
		{
		}

		public override object InternalMeasure()
		{
			var list = new MagicEyeData();
			foreach(Body o in Robot.Emulator.Objects)
				AddObject(list.Objects, o.Name, o.Location);
			foreach(Robot r in Robot.Emulator.Robots)
			{
				if(r.RobotNumber == Robot.RobotNumber) 
					continue;
				AddObject(list.Objects, r.Name, r.Location);
			}
			return list;
		}

		private void AddObject(List<MagicEyeObject> objects, string name, Frame3D location)
		{
			if(Angem.Hypot(Robot.Location.ToPoint3D(), location.ToPoint3D()) > Settings.Radius) 
				return;
			if(Settings.LocalCoordinates) 
				location = Robot.Location.Invert().Apply(location);
			objects.Add(new MagicEyeObject
			            	{
			            		Name = name,
			            		Location = location.ToFrame2D()
			            	});
		}
	}
}
using System;
using AIRLab.Mathematics;

namespace Eurosim.Core
{
	public class NavigatorModel : SensorModel<EmulatedNavigatorSettings>
	{
		public NavigatorModel(Robot robot, EmulatedNavigatorSettings settings)
			: base(robot, settings)
		{	
		}

		public double Randomize(double value, double range)
		{
			return value + (_rnd.NextDouble() - 0.5) * 2 * range;
		}

		public override void Reset()
		{
			base.Reset();
			_firstTime = true;
		}

		public override object InternalMeasure()
		{
			var data = new NavigatorData();
			data.Time = Robot.Emulator.StartTime.AddSeconds(Robot.Emulator.LocalTime);
			Frame2D loc = Robot.Location.ToFrame2D();
			if(!Settings.IsPlane)
				throw new NotImplementedException("");
			if(Settings.NoiseModel == NoiseModel.Local)
			{
				data.Location = new Frame2D(
					Randomize(loc.X, Settings.LengthNoise),
					Randomize(loc.Y, Settings.LengthNoise),
					Angle.FromGrad(Randomize(loc.Angle.Grad, Settings.AngleNoise.Grad))
					);
			}
			if(Settings.NoiseModel == NoiseModel.Integral)
			{
				if(_firstTime)
				{
					_oldSentLocation = _oldRealLocation = loc;
					data.Location = loc;
					_firstTime = false;
				}
				else
				{
					Frame2D movement = _oldRealLocation.Invert().Apply(loc);
					movement = new Frame2D(
						Randomize(movement.X, Settings.LengthNoise),
						Randomize(movement.Y, Settings.LengthNoise),
						Angle.FromGrad(Randomize(movement.Angle.Grad, Settings.AngleNoise.Grad)));
					data.Location = _oldSentLocation.Apply(movement);
					_oldRealLocation = loc;
					_oldSentLocation = data.Location;
				}
			}
			return data;
		}

		private readonly Random _rnd=new Random();

		private bool _firstTime = true;
		private Frame2D _oldRealLocation;
		private Frame2D _oldSentLocation;
	}
}
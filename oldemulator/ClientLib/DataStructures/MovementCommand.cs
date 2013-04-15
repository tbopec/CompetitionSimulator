using System;

namespace Eurosim.ClientLib
{
	public class MovementCommand
	{
		private MovementCommand()
		{
		}

		public static MovementCommand LineMovement(double distance)
		{
			return LineMovement(distance, Math.Abs(distance / LinSpeed));
		}

		public static MovementCommand LineMovement(double distance, double time)
		{
			return new MovementCommand
			       	{
			       		Angle = 0,
			       		Distance = distance,
			       		Time = time
			       	};
		}

		public static MovementCommand AngleRotation(double angle)
		{
			return AngleRotation(angle, Math.Abs(angle / AngularSpeed));
		}

		public static MovementCommand AngleRotation(double angle, double time)
		{
			return new MovementCommand
			       	{
			       		Angle = angle,
			       		Distance = 0,
			       		Time = time
			       	};
		}

		public static MovementCommand ArcMovement(double distance, double angle)
		{
			double maxtime = Math.Max(Math.Abs(distance / LinSpeed), Math.Abs(angle / AngularSpeed));
			return ArcMovement(distance, angle, maxtime);
		}

		public static MovementCommand ArcMovement(double distance, double angle, double time)
		{
			return new MovementCommand
			       	{
			       		Angle = angle,
			       		Distance = distance,
			       		Time = time
			       	};
		}

		public static MovementCommand ActuatorAction(string cmd, int actuatorNumber)
		{
			return new MovementCommand
			       	{
			       		ActuatorCommand = cmd,
			       		ActuatorNumber = actuatorNumber,
			       		Angle = 0,
			       		Distance = 0,
			       		Time = 0.5
			       	};
		}

		public override string ToString()
		{
			return string.Format("ActuatorCommand: {0}, Angle: {1}, Distance: {2}, Time: {3}", ActuatorCommand, Angle, Distance, Time);
		}

		public int ActuatorNumber { get; private set; }
		public string ActuatorCommand { get; private set; }
		public double Angle { get; private set; }
		public double Distance { get; private set; }
		public double Time { get; private set; }
		private const double LinSpeed = CommonConsts.MaxLinearSpeed - 1;
		private const double AngularSpeed = CommonConsts.MaxAngularSpeed - 1;
	}
}
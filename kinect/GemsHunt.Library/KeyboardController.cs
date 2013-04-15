using System;
using System.Windows.Forms;
using AIRLab.Mathematics;

namespace GemsHunt.Library
{
	public class KeyboardController
	{
		private const int MovementDistance = 100;

		public KeyboardController(Form form, Robot2013 robot)
		{
			_robot = robot;
			form.KeyPreview = true;
			form.KeyDown += MoveRobotIfRequired;
		}

		private void MoveRobotIfRequired(object sender, KeyEventArgs keyPressEventArgs)
		{
			switch(keyPressEventArgs.KeyData)
			{
				//TODO (для Serj) Что это за вычисления? Мб можно использовать стандартные функции?
				case Keys.W:
					_robot.Velocity = new Frame3D(MovementDistance * Math.Cos(_robot.Location.Yaw.Radian), MovementDistance * Math.Sin(_robot.Location.Yaw.Radian), 0);
					break;
				case Keys.S:
					_robot.Velocity = new Frame3D(-MovementDistance * Math.Cos(_robot.Location.Yaw.Radian), -MovementDistance * Math.Sin(_robot.Location.Yaw.Radian), 0);
					break;
				case Keys.A:
					_robot.Velocity = Frame3D.DoYaw(Angle.FromGrad(75));
					break;
				case Keys.D:
					_robot.Velocity = Frame3D.DoYaw(Angle.FromGrad(-75));
					break;
				case Keys.R:
					_robot.AddCommand("grip");
					break;
				case Keys.F:
					_robot.AddCommand("release");
					break;
			}
		}

		private readonly Robot2013 _robot;
	}
}
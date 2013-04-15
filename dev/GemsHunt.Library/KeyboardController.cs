using System;
using System.Windows.Forms;
using AIRLab.Mathematics;

namespace GemsHunt.Library
{
	public class KeyboardController
	{
		private const int MovementDistance = 100;

		public KeyboardController(Control control, Robot2013 RobotLeft, Robot2013 RobotRight)
		{
			_robotleft = RobotLeft;
            _robotright = RobotRight;
			var form = (control.TopLevelControl as Form);
			if (form==null)
				throw new Exception("Control must be added to a top-level form.");
			form.KeyPreview = true;
			form.KeyDown += MoveRobotIfRequired;
		}

		private void MoveRobotIfRequired(object sender, KeyEventArgs keyPressEventArgs)
		{
			switch(keyPressEventArgs.KeyData)
			{
				
				case Keys.W:
                    _robotleft.Velocity = new Frame3D(MovementDistance * Math.Cos(_robotleft.Location.Yaw.Radian), MovementDistance * Math.Sin(_robotleft.Location.Yaw.Radian), 0);
					break;
				case Keys.S:
                    _robotleft.Velocity = new Frame3D(-MovementDistance * Math.Cos(_robotleft.Location.Yaw.Radian), -MovementDistance * Math.Sin(_robotleft.Location.Yaw.Radian), 0);
					break;
				case Keys.A:
					_robotleft.Velocity = Frame3D.DoYaw(Angle.FromGrad(75));
					break;
				case Keys.D:
					_robotleft.Velocity = Frame3D.DoYaw(Angle.FromGrad(-75));
					break;
				case Keys.R:
					_robotleft.AddCommand("grip");
					break;
				case Keys.F:
					_robotleft.AddCommand("release");
					break;


                case Keys.U:
                    _robotright.Velocity = new Frame3D(MovementDistance * Math.Cos(_robotright.Location.Yaw.Radian), MovementDistance * Math.Sin(_robotright.Location.Yaw.Radian), 0);
                    break;
                case Keys.J:
                    _robotright.Velocity = new Frame3D(-MovementDistance * Math.Cos(_robotright.Location.Yaw.Radian), -MovementDistance * Math.Sin(_robotright.Location.Yaw.Radian), 0);
                    break;
                case Keys.H:
                    _robotright.Velocity = Frame3D.DoYaw(Angle.FromGrad(75));
                    break;
                case Keys.K:
                    _robotright.Velocity = Frame3D.DoYaw(Angle.FromGrad(-75));
                    break;
                case Keys.O:
                    _robotright.AddCommand("grip");
                    break;
                case Keys.L:
                    _robotright.AddCommand("release");
                    break;
			}
		}

		private readonly Robot2013 _robotleft;
        private readonly Robot2013 _robotright;
	}
}
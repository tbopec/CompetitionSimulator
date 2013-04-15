using System;
using System.Collections.Generic;
using System.Windows.Forms;
using AIRLab.Mathematics;
using Eurosim.Core;
using Eurosim.Engine.TestPolygon;

namespace EurosimStandalone
{
	internal class KeyboardController
	{
		public KeyboardController(TestPolygon polygon, Form form)
		{
			_robot = polygon.MainBody;
			form.KeyPreview = true;
			form.KeyPress += MoveRobotIfRequired;
		}

		private void MoveRobotIfRequired(object sender, KeyPressEventArgs keyPressEventArgs)
		{
//			IRobotAction actionToDo;
//			if (_keyMap.TryGetValue(keyPressEventArgs.KeyChar, out actionToDo)
//			   && _robot != null)
//				lock (_robot.Movements)
//					_robot.Movements.Enqueue(actionToDo);
			switch (keyPressEventArgs.KeyChar)
			{
				case 'w': _robot.Velocity = new Frame3D(100, 0, 0); break;
			}
		}

//		private static IRobotAction Forward(double distance)
//		{
//			return new TrivialPlaneMovement
//			       	{
//			       		Offset = new Frame2D(distance, 0, Angle.Zero),
//			       		TotalTime = Math.Abs(distance / 100)
//			       	};
//		}
//
//		private static IRobotAction Right(double grad)
//		{
//			return new TrivialPlaneMovement
//			       	{
//			       		Offset = new Frame2D(0, 0, -Angle.FromGrad(grad)),
//			       		TotalTime = Math.Abs(grad / 100)
//			       	};
//		}
//
//		private static IRobotAction Left(double grad)
//		{
//			return Right(-grad);
//		}
//
//		private static IRobotAction Backward(double distance)
//		{
//			return Forward(-distance);
//		}
//
//		private static IRobotAction Action(string action)
//		{
//			return new ActuatorExternalAction {ActuatorCommand = action, ActuatorNumber = 0, TotalTime = 1};
//		}
//
//		private readonly Dictionary<char, IRobotAction> _keyMap =
//			new Dictionary<char, IRobotAction>
//				{
//					{'w', Forward(MovementDistance)},
//					{'a', Left(MovementDistance)},
//					{'s', Backward(MovementDistance)},
//					{'d', Right(MovementDistance)},
//					{'e', Action("Grip")},
//					{'r', Action("Release")},
//					{'q', Action("PushButton")}
//				};

		private readonly Body _robot;
		private const double MovementDistance = 10;
	}
}
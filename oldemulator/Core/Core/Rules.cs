using System;
using System.Collections.Generic;
using System.Drawing;
using AIRLab.Mathematics;

namespace Eurosim.Core
{
	public abstract class Rules
	{
		protected Rules(Emulator emulator)
		{
			Emulator = emulator;
		}

		/// <summary>
		/// Пересчитать очки набранные роботами
		/// </summary>
		public virtual void AccountScores()
		{
		}

		/// <summary>
		/// Инициализировать игровое поле
		/// </summary>
		public virtual void InitializeField()
		{
			foreach(Body b in InitializeTable())
				Emulator.TableObjects.Add(b);
			foreach(Body e in InitializePieces())
				Emulator.Objects.Add(e);
			PositionRobots();
			InitializeScores();
		}

		public abstract IEnumerable<Body> InitializePieces();
		public abstract IEnumerable<Body> InitializeTable();
		public abstract void PositionRobots();

		public virtual void InitializeScores()
		{
		}

		/// <summary>
		/// Доопределить роботов в соответствии с правилами(в частности - добавить стрелки)
		/// </summary>
		public virtual void AdditionalDefineRobots()
		{
		}

		public virtual RobotAI CreateAI(string AIName)
		{
			return null;
		}

		public virtual Actuator CreateActuator(Robot robot, ActuatorSettings settings)
		{
			throw new Exception("Requested actuator with settings " + settings.GetType() + " is not found");
		}

		/// <summary>
		/// Пол - плоская часть стола.
		/// </summary>
		public PrimitiveBody Floor { get; protected set; }

		public readonly Emulator Emulator;

		/// <summary>
		/// Добавить роботам цветные стрелки
		/// </summary>
		/// <param name="redside">Номер стороны красного робота (0-слева, 1 -справа)</param>
		protected void AddArrows(int redside)
		{
			foreach(Robot robot in Emulator.Robots)
			{
				var colorString = (robot.RobotNumber == redside ? "red" : "blue");
				robot.Add(new PrimitiveBody(new BoxShape(10, 2, 2), Color.FromName(colorString),
											"arrow" + colorString)
						  {
							  Location = new Frame3D(0, 0, 30)
						  });
			}
		}
	}
}
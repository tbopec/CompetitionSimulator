using System;
using System.Drawing;
using AIRLab.Mathematics;
using Eurosim.Core;

namespace Eurosim.TreasureIsland
{
	/// <summary>
	/// монета
	/// </summary>
	public class Coin : PhysicalPrimitiveBody
	{
		public Coin(Frame3D location, bool isMaterial, String color)
		{
			var visModel = new PrimitiveBody(new CyllinderShape(6, 6, 0.5), Color.FromName(color),
			                                 "TreasureIsland." + (color == "black" ? color : "") + "cd");
			var disc = new PhysicalPrimitiveBody(new CyllinderShape(6, 6, 0.5), Color.Transparent)
			           {
			           	Mass = DiscMass,
			           	IsMaterial = isMaterial,
			           	FrictionCoefficient = FrictionCoefficient,
			           	Location = new Frame3D(0, 0, 1.16)
			           };
			var box = new PhysicalPrimitiveBody(new BoxShape(1.8, 1.8, 1.8), Color.Transparent)
			          {
			          	Mass = BoxMass,
			          	IsMaterial = isMaterial,
			          	FrictionCoefficient = FrictionCoefficient * 5,
			          	Location = new Frame3D(4.05, 0, -1.3)
			          };
			PhysicalModel = disc.PhysicalModel;
			Add(visModel);
			Add(box);
			Add(disc);
			Location = location;
		}

		private const double FrictionCoefficient = 1;
		private const double DiscMass = 0.1;
		private const double BoxMass = 0.5;
	}

	/// <summary>
	/// Тряпочка
	/// </summary>
	public class Cloth : PrimitiveBody
	{
		public Cloth(Frame3D frame3D, String color)
			: base(new RectangleShape(20, 0), "TreasureIsland.cloth" + color)
		{
			Location = frame3D;
			Color = Color.FromName(color);
			RobotNumber = color == "red" ? 0 : 1;
		}

		public int RobotNumber;
	}

	/// <summary>
	/// слиток
	/// </summary>
	public class Ingot : PhysicalPrimitiveBody
	{
		public Ingot(Frame3D location, bool isMaterial)
			:
				base(new BoxShape(7, 15, 4.5), TreasureIslandRules.sand
				     , "TreasureIsland.ingot"
				)
		{
			Location = location;
			IsMaterial = isMaterial;
			Mass = ingotMass;
			FrictionCoefficient = _FrictionCoefficient;
		}

		private const double _FrictionCoefficient = 5;
		private const double ingotMass = 2;
	}

	/// <summary>
	/// Крышка сундука. Является телом с изменяемым состоянием
	/// </summary>
	public class Chest : BodyCollection<Body>
	{
		public Chest(Frame3D location)
		{
			Location = location;
			Top = new PrimitiveBody(new BoxShape(34, 61, 1.8),
			                        Color.FromArgb(128, Color.LightGray), "TreasureIsland.chest");
			Add(Top);
/*            Add(new PhysicalPrimitiveBody(new BoxShape(34, 0.1, 1.8),Color.Transparent )
                    {
                        Location = new Frame3D(0,location.Y-30.5,0),
                        IsStatic=true
                    });//невидимая часть сбоку
           */
			rotationCenter = Top.Location.NewX(Math.Sign(Location.X) * 17);
			_state = ChestState.Closed;
		}

		public Enum State
		{
			get { return _state; }
			set
			{
				if(!(value is ChestState))
					return;
				if(_state == ChestState.Closed && (ChestState)value == ChestState.Open)
				{
					Top.PitchRotateBody(rotationCenter, Angle.FromGrad(45));
					_state = ChestState.Open;
				}
				if(_state == ChestState.Open && (ChestState)value == ChestState.Closed)
				{
					Top.PitchRotateBody(rotationCenter, Angle.FromGrad(-45));
					_state = ChestState.Closed;
				}
			}
		}

		public enum ChestState
		{
			Open,
			Closed
		};

		private ChestState _state;
		private readonly Frame3D rotationCenter;
		private readonly Body Top;
	}

	/// <summary>
	/// Кнопка-бутылка
	/// </summary>
	public class Button : PrimitiveBody
	{
		public Button(Frame3D location, String color)
			: base(new BoxShape(20,5,5),Color.FromName(color),"TreasureIsland.bottle" + color)
		{
			_state = ButtonState.Normal;
			Location = location;
			RobotNumber = color == "red" ? 0 : 1;
		}

		public Enum State
		{
			get { return _state; }
			set
			{
				if(_state == ButtonState.Normal && value is ButtonState &&
				   (ButtonState)value == ButtonState.Activated)
				{
					this.PitchRotateBody(Location, Angle.HalfPi);
					_state = ButtonState.Activated;
				}
			}
		}

		public int RobotNumber { get; private set; }

		public enum ButtonState
		{
			Activated,
			Normal
		}

		private ButtonState _state;
	}
}
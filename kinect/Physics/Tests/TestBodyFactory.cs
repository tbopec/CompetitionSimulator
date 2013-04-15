using System.Drawing;
using AIRLab.Mathematics;

namespace Eurosim.Core
{
	class TestBodyFactory
	{
		public static Body CreateRobotBox()
		{
			var b = new Box
			        	{
			        		XSize = 10,
			        		YSize = 10,
			        		ZSize = 20,
			        		DefaultColor = Color.Green,
			        		Density = Density.Aluminum,
			        		FrictionCoefficient = 0.5,
			        		IsMaterial = true
			        	};
			return b;
		}
		public static Body CreateWall()
		{
			var b = new Box
			        	{
			        		XSize = 10,
			        		YSize = 100,
			        		ZSize = 10,
			        		DefaultColor = Color.LightGray,
			        		IsStatic = true,
			        		IsMaterial = true,
			        		Location = new Frame3D(100, 0, 0)
			        	};
			return b;
		}

		public static Body CreateEmptyCollection(Frame3D loc)
		{
			var b = new Body {Location = loc};
			return b;
		}

		public static Body CreateSmallPhysicalBall(Frame3D loc)
		{
			var ball = new Ball
			           	{
			           		Radius = 3,
			           		DefaultColor = Color.Red,
			           		Location = loc,
			           		Density = Density.Aluminum,
			           		FrictionCoefficient = 0.7,
			           		IsMaterial = true
			           	};
			return ball;
		}

		public static Body CreateSimplestJoinedBody(Frame3D frame3D)
		{
			var parent = CreateSmallPhysicalBall(frame3D);
			parent.Add(CreateSmallPhysicalBall(new Frame3D(5,0,5)));
			parent.Add(CreateSmallPhysicalBall(new Frame3D(10,0,10)));
			return parent;
		}
	}
}

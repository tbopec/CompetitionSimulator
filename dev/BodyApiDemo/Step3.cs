using System.Drawing;
using AIRLab.Mathematics;
using Eurosim.Core;
using Eurosim.Graphics;
using Eurosim.Graphics.DirectX;
using Eurosim.Graphics.Properties;

namespace Eurosim.BodyAPIDemo
{
	internal class Step3
	{
		//метод переименован чтобы избежать конфликтов.
		public static void gMain(string[] args)
		{
			var root = new Body();
			var floor = new Body
				{
					//графическое представление тел можно задавать при помощи модели
					Model = Model.FromResource(() => Resources.treasureIslandTable)
				};
			root.Add(floor);
			//можно создавать сложные тела.
			//Здесь будут двигаться вместе с перемещением родителя.
			var flyingSaucer = new Cylinder
				{
					RTop = 0,
					RBottom = 30,
					Height = 4,
					DefaultColor = Color.Blue,
					Location = new Frame3D(0, 0, 50)
				};
			root.Add(flyingSaucer);
			for(int i = 0; i < 4; i++)
			{
				flyingSaucer.Add(new Ball
					{
						DefaultColor = Color.Blue,
						Radius = 5,
						//при добавлении частей к телу Location считается относительно родителя
						Location = Frame3D.DoYaw(i * Angle.HalfPi).Apply(new Frame3D(30, 0, -3))
					});
			}
			//для определения перемещения телу можно присвоить делегат
			flyingSaucer.CustomAnimationDelegate = (body, dt) =>
				{
					var diff = new Frame3D(0, 0, 1);
					if(body.Location.Z > 50 || body.Location.Z < 10)
						diff = diff.Invert();
					body.Location += diff;
				};
			//Если сложному телу сделать 
			flyingSaucer.IsMaterial = true;
			//то тело должно стать материальным и физически взаимодействующим вместе со всеми детьми.
			//На данный момент, вероятно, это будет реализовано только для тел, который близки к плоскости Z=0.
			CreateGraphics(root);
		}

		//это заклинание поднимает графику.
		//root - это корень дерева тел.
		public static void CreateGraphics(Body root)
		{
			new DrawerFactory(root).CreateAndRunDrawerInStandaloneForm(VideoModes.DirectX);
		}
	}
}
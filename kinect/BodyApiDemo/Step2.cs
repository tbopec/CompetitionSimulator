using System.Drawing;
using AIRLab.Mathematics;
using Eurosim.Core;
using Eurosim.Graphics.DirectX;
using Eurosim.Graphics.Properties;

namespace Eurosim.BodyAPIDemo
{
	internal class Step2
	{
		//метод переименован чтобы избежать конфликтов.
		public static void AMain(string[] args)
		{
			//Тела могут быть разными.
			//Само по себе это тело пустое.
			//У него нет графического отображения и оно не будет физически взаииодействовать.
			//но мы можем использовать его в качестве корня дерева.
			var body = new Body();
			var bigFreckingWall = new Box
				{
					XSize = 1,
					YSize = 100,
					ZSize = 100,
					//А этот ящик будет невидимой материальной стеной.
					IsMaterial = true,
					//Статической, т.е. бетонной и неподвижной.
					DefaultColor = Color.Transparent,
					IsStatic = true,
					Location = new Frame3D(100, 0, 0)
				};
			body.Add(bigFreckingWall);
			//А Ball- это шар.
			var ball = new Ball
				{
					Radius = 10,
					//Шару нельзя присваивать изображение. только цвет.
					DefaultColor = Color.Red,
					//этот шар будет физическим.
					IsMaterial = true,
					Density = Density.Aluminum,
					FrictionCoefficient = 0.7,
					Location = new Frame3D(-50, 0, 5)
				};
			body.Add(ball);
			//A Сylinder- это цилиндр.
			body.Add(new Cylinder
				{
					RBottom = 20,
					RTop = 10,
					Height = 10,
					//на данный момент поддерживаются только цвета.
					DefaultColor = Color.Green,
					Top = new SolidColorBrush
						{
							Color = Color.Red
						},
					Bottom = new SolidColorBrush
						{
							Color = Color.Blue
						},
					IsMaterial = true,
					Density = Density.Wood,
					FrictionCoefficient = 0.8,
					Location = new Frame3D(50, 0, 5)
				});
			//тела не обязательно должны быть Box/Ball/Cylinder
			//но только Box / Ball /Cylinder будут иметь соответствующее им представление в физическом мире.
			//например,
			body.Add(new Body
				{
					Model = Model.FromResource(() => Resources.queentower3),
					Location = new Frame3D(0, 10, 0)
					//красиво выглядит, но не будет участвовать в физическом взаимодействии
					//даже при выставленных своийствах
					//IsMaterial=true,
					//Density=Density.Iron,
				});
			//А вот это будет выглядеть также, но будет физически взаимодействовать
			//как цилиндр радиусом 10см, высотой, массой, итд.
			body.Add(new Cylinder
				{
					RBottom = 10,
					RTop = 10,
					Height = 20,
					Model = Model.FromResource(() => Resources.queentower3),
					DefaultColor = Color.Yellow,
					Location = new Frame3D(0, -10, 0),
					IsMaterial = true,
					Density = Density.Wood,
					FrictionCoefficient = 0.8
				});
			body.Add(new Box
				{
					XSize = 300,
					YSize = 200,
					ZSize = 3,
					DefaultColor = Color.LightGray,
					Top = PlaneImageBrush.FromResource(() => Resources.testtexture),
				});
			CreateGraphics(body);
		}

		//это заклинание поднимает графику.
		//root - это корень дерева тел.
		public static void CreateGraphics(Body root)
		{
			var scene = new DirectXScene(root);
			new DirectXFormDrawer(scene).Run();
		}
	}
}
using System.Drawing;
using AIRLab.Mathematics;
using Eurosim.Core;
using Eurosim.Graphics.DirectX;
using Eurosim.Graphics.Properties;

namespace Eurosim.BodyAPIDemo
{

	internal class Step4
	{
		//метод переименован чтобы избежать конфликтов.
		public static void AMain(string[] args)
		{
			var root = new Body();
			//можно создавать тела, имеющие определенную форму - это параллелепипед
			root.Add(new Box
				{
					XSize = 10,
					YSize = 10,
					ZSize = 10,
					Location = new Frame3D(50, 50, 5),
					DefaultColor = Color.DarkGoldenrod,
					IsMaterial = true, //это необязательно. 
					Name = "orange, a physical one",
					Velocity = new Frame3D(-100, -19, 10), // назначим объекту начальную скорость
					FrictionCoefficient = 1, // можно включить трение				
					Density = Density.Aluminum,
					//дефолтно этот ящик будет нематериальным и не сможет участвовать в столкновениях.
				});
			root.Add(new Cylinder
				{
					RTop = 10,
					RBottom = 10,
					Height = 20,
					Location = new Frame3D(75, 0, 10),
					DefaultColor = Color.GreenYellow,
					IsMaterial = true, //это необязательно. 
					Name = "Cylinder",
					FrictionCoefficient = 1, // можно включить трение
					Density = Density.Aluminum,
					//дефолтно этот ящик будет нематериальным и не сможет участвовать в столкновениях.
				});
			root.Add(new Box
				{
					XSize = 300,
					YSize = 200,
					ZSize = 3,
					//а это дефолтный цвет. им будут покрыты все грани, которым не присвоены кисти.
					DefaultColor = Color.White,
					//кроме цвета параллелепипед можно также покрыть картинкой.
					Top = new PlaneImageBrush
						{
							//это кисть с изображением.
							//изображение можно грузить из файла.
							FilePath = "clouds-1259.jpg"
						},
					IsStatic = true,
					Name = "floor",
					Density = Density.Aluminum
				});
			//а этой кисти передано непосредственно само изображение.
			PlaneImageBrush imageBrush = PlaneImageBrush.FromResource(() => Resources.testtexture);
			var mainBody = new Box
				{
					XSize = 30,
					YSize = 30,
					ZSize = 30,
					Location = new Frame3D(-100, 0, 0),
					//эту сторону покрасим цветом
					Back = new SolidColorBrush {Color = Color.Red},
					//а остальным назначим кисть с исзображением.
					Front = imageBrush,
					Left = imageBrush,
					Right = imageBrush,
					Top = imageBrush,
					Bottom = imageBrush,
					IsMaterial = true,
					FrictionCoefficient = 1,
					Name = "MainBody",
					Density = Density.Aluminum,
					//IsStatic = true
					//				Velocity = new Frame3D(10, 0, 0),				
				};
			root.Add(mainBody);
			CreateGraphics(root);
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
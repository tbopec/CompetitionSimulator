using System.Drawing;
using AIRLab.Mathematics;
using Eurosim.Core;
using Eurosim.Graphics;
using Eurosim.Graphics.DirectX;
using Eurosim.Graphics.Properties;

namespace Eurosim.BodyAPIDemo
{
	internal static class Step1
	{
		public static void Main(string[] args)
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
							Velocity = new Frame3D(10,0,0)
			         		//IsMaterial=false //это необязательно. 
			         		//дефолтно этот ящик будет нематериальным и не сможет участвовать в столкновениях.
			         	});

			for (int i = 0; i < 3; i++)
				root.Add(new Box
				         	{
				         		XSize = 10,
				         		YSize = 10,
				         		ZSize = 10,
				         		Location = new Frame3D(10, 0, 5 + 12*i),
				         		DefaultColor = Color.FromArgb(128, Color.Blue),
				         		//А можно делать тела материальными
				         		IsMaterial = true,
				         		Density = Density.Wood,
				         		FrictionCoefficient = 0.5
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
			         	});

			//а этой кисти передано непосредственно само изображение.
			var imageBrush = PlaneImageBrush.FromResource(() => Resources.testtexture);
			root.Add(new Box
						{
							XSize = 30,
							YSize = 30,
							ZSize = 30,
							Location = new Frame3D(-100,0,0),
							//эту сторону покрасим цветом
							Back = new SolidColorBrush { Color = Color.Red },
							//а остальным назначим кисть с исзображением.
							Front = imageBrush,
							Left = imageBrush,
							Right = imageBrush,
							Top = imageBrush,
							Bottom = imageBrush,
						});
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
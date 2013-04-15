using System.Drawing;
using System.Linq;
using AIRLab.Mathematics;
using Eurosim.Core;
using Eurosim.Graphics.Properties;
using GemsHunt.Library;

namespace Eurosim.MovingDemo
{
	public class MovingDemoProcess : AbstractBaseProcess
	{
		public static void Main(string[] args)
		{
			Instance = new MovingDemoProcess();
			Instance.RunInBackgroundThread();
		}
		public MovingDemoProcess()
		{
			new KeyboardController(Forms.First(), Robot);
		}

		protected override void InitializeBodies(Body root)
		{
			//можно создавать тела, имеющие определенную форму - это параллелепипед
			root.Add(new Box
				{
					XSize = 10,
					YSize = 10,
					ZSize = 10,
					Location = new Frame3D(50, 50, 5),
					DefaultColor = Color.DarkGoldenrod,
					IsMaterial = true,
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
			Robot = new Robot2013(Root)
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
				};
			root.Add(Robot);
			Robot.Add(new Box(10, 50, 10)
				{
					Location = new Frame3D(15, 20, 0),
					DefaultColor = Color.BlanchedAlmond,
					IsMaterial = true
				});
		}

		public static MovingDemoProcess Instance { get; private set; }
		public Robot2013 Robot;


	}
}
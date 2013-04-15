using System;
using System.Drawing;
using AIRLab.Mathematics;
using Eurosim.Core;

namespace GemsHunt.Library
{
	//TODO(для Serj) World не должен быть наследником от Body!
	[Serializable]
	public class World : Body
	{
		public void FillRoot()
		{
			Add(new Box
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
				});
			CreateRombs(this);
			CreatePile(this);
			CreateTreasure(this);
			RobotLeft = new Robot2013(this)
				{
					XSize = 15,
					YSize = 15,
					ZSize = 15,
					Location = new Frame3D(-130, 80, 3),
					//эту сторону покрасим цветом
					DefaultColor = Color.Blue,
					Back = new SolidColorBrush {Color = Color.LightSkyBlue},
					//а остальным назначим кисть с исзображением.
					//Front = imageBrush,
					//Left = imageBrush,
					//Right = imageBrush,
					//Top = imageBrush,
					//Bottom = imageBrush,
					IsMaterial = true,
					Density = Density.Iron,
					FrictionCoefficient = 4,
					Name = "MainBody",
					//IsStatic = true,
//				Velocity = new Frame3D(10, 0, 0),				
				};
			RobotRight = new Robot2013(this)
				{
					XSize = 15,
					YSize = 15,
					ZSize = 15,
					Location = new Frame3D(130, 80, 3, Angle.Zero, Angle.Pi, Angle.Zero),
					DefaultColor = Color.Green,
					Back = new SolidColorBrush {Color = Color.LightSeaGreen},
					IsMaterial = true,
					Density = Density.Iron,
					FrictionCoefficient = 4,
					Name = "MainBody2",
				};
			Add(RobotLeft);
			Add(RobotRight);
		}

		public Robot2013 RobotLeft;
		public Robot2013 RobotRight;

		private void CreateRombs(Body root)
		{
			root.Add(new Box
				{
					XSize = 15,
					YSize = 15,
					ZSize = 0.5,
					Location = new Frame3D(-130, 30, 3, Angle.Zero, Angle.FromGrad(45), Angle.Zero),
					DefaultColor = Color.Cyan,
					//IsMaterial = true, //это необязательно. 
					IsStatic = true,
					Name = "RombLeft",
					//FrictionCoefficient = 1 // можно включить трение
					//дефолтно этот ящик будет нематериальным и не сможет участвовать в столкновениях.
				});
			root.Add(new Box
				{
					XSize = 15,
					YSize = 15,
					ZSize = 0.5,
					Location = new Frame3D(130, 30, 3, Angle.Zero, Angle.FromGrad(45), Angle.Zero),
					DefaultColor = Color.Cyan,
					IsStatic = true,
					Name = "RombRight",
				});
			var rand = new Random();
			int RandRombLeft = rand.Next(1, 4);
			int RandRombRight = rand.Next(1, 4);
			root.Add(new Box
				{
					XSize = 15,
					YSize = 15,
					ZSize = 0.5,
					Location = new Frame3D(-130, 30 - RandRombLeft * 35, 3, Angle.Zero, Angle.FromGrad(45), Angle.Zero),
					DefaultColor = Color.Cyan,
					IsStatic = true,
					Name = "RandomRombLeft",
				});
			root.Add(new Box
				{
					XSize = 15,
					YSize = 15,
					ZSize = 0.5,
					Location = new Frame3D(130, 30 - RandRombRight * 35, 3, Angle.Zero, Angle.FromGrad(45), Angle.Zero),
					DefaultColor = Color.Cyan,
					IsStatic = true,
					Name = "RandomRombRight",
				});
		}

		private void CreatePile(Body root)
		{
			Add(new Cylinder
				{
					RTop = 15,
					RBottom = 15,
					Height = 40,
					Location = new Frame3D(0, 0, 3),
					DefaultColor = Color.Gray,
					IsMaterial = true,
					IsStatic = true,
					Name = "Cylinder",
					FrictionCoefficient = 1
				});
			double radius = 80;
			for(int i = 0; i < 6; i++)
			{
				Angle AngleCircle = Angle.FromGrad(60 * i);
				Add(new Cylinder
					{
						RTop = 5,
						RBottom = 5,
						Height = 40,
						Location = new Frame3D(radius * Math.Sin(AngleCircle.Radian), radius * Math.Cos(AngleCircle.Radian), 3),
						DefaultColor = Color.Gray,
						IsMaterial = true,
						IsStatic = true,
						Name = "Cylinder",
						FrictionCoefficient = 1
					});
			}
		}

		private void CreateTreasure(Body root)
		{
			root.Add(new Box
				{
					XSize = 10,
					YSize = 10,
					ZSize = 10,
					Location = new Frame3D(50, 80, 3),
					DefaultColor = Color.YellowGreen,
					IsMaterial = true,
					Name = "Emerald",
					FrictionCoefficient = 1
				});
			root.Add(new Box
				{
					XSize = 10,
					YSize = 10,
					ZSize = 10,
					Location = new Frame3D(-50, 80, 3),
					DefaultColor = Color.YellowGreen,
					IsMaterial = true,
					Name = "Emerald",
					FrictionCoefficient = 1
				});
			root.Add(new Box
				{
					XSize = 10,
					YSize = 10,
					ZSize = 10,
					Location = new Frame3D(50, -80, 3),
					DefaultColor = Color.Red,
					IsMaterial = true,
					Name = "Ruby",
					FrictionCoefficient = 1
				});
			root.Add(new Box
				{
					XSize = 10,
					YSize = 10,
					ZSize = 10,
					Location = new Frame3D(-50, -80, 3),
					DefaultColor = Color.Red,
					IsMaterial = true,
					Name = "Ruby",
					FrictionCoefficient = 1
				});
			double radius = 60;
			for(int k = -1; k < 2; k += 2)
			{
				for(int i = 0; i < 6; i++)
				{
					Angle AngleCircle = Angle.FromGrad(15 + 30 * i);
					/*Add(new Box
                    {
                        XSize = 10,
                        YSize = 10,
                        ZSize = 10,
                        Location = new Frame3D(radius * k* Math.Sin(AngleCircle.Radian), radius * Math.Cos(AngleCircle.Radian), 3),
                        DefaultColor = Color.White,
                        IsMaterial = true,
                        Name = "Ruby",
                        FrictionCoefficient = 1

                    });*/
				}
			}
		}
	}
}
using System;
using System.Collections.Generic;
using System.Drawing;
using AIRLab.Mathematics;
using Eurosim.Core;

namespace GemsHunt.Library
{
	//TODO(для Serj) World не должен быть наследником от Body!
	[Serializable]
	public class World : Body
	{

        const double RobotFrictionCoefficient = 8;
        const double BoxFrictionCoefficient = 6;


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
							FilePath = "ice.jpg"
						},
					IsStatic = true,
					Name = "floor",
				});
		    CreateBorders();
		    CreateRombs(this);
			CreatePile(this);
			CreateTreasure(this);
			RobotLeft = new Robot2013(this)
				{
					XSize = 17,
					YSize = 17,
					ZSize = 17,
					Location = new Frame3D(-130, 80, 3),
					//эту сторону покрасим цветом
                    DefaultColor = Color.Green,
                    Back = new SolidColorBrush { Color = Color.LightSeaGreen },
					//а остальным назначим кисть с исзображением.
					//Front = imageBrush,
					//Left = imageBrush,
					//Right = imageBrush,
					//Top = imageBrush,
					//Bottom = imageBrush,
					IsMaterial = true,
					Density = Density.Iron,
					FrictionCoefficient = RobotFrictionCoefficient,
					Name = "RobotLeft",
					//IsStatic = true,
//				Velocity = new Frame3D(10, 0, 0),				
				};
			RobotRight = new Robot2013(this)
				{
                    XSize = 17,
                    YSize = 17,
                    ZSize = 17,
					Location = new Frame3D(130, 80, 3, Angle.Zero, Angle.Pi, Angle.Zero),
					DefaultColor = Color.Blue,
					Back = new SolidColorBrush {Color = Color.LightSkyBlue},
					IsMaterial = true,
					Density = Density.Iron,
					FrictionCoefficient = RobotFrictionCoefficient,
                    Name = "RobotRight",
				};
			Add(RobotLeft);
			Add(RobotRight);
		}

	    private void CreateBorders()
	    {
	        Color wallsColor = Color.FromArgb(50, 0, 0, 0);
	        for(int i = 0; i < 4; ++i)
	        {
	            var sizeX = i / 2 == 0 ? 303 : 3;
	            var sizeY = i / 2 == 1 ? 203 : 3;
                var lX = i / 2 == 0 ? 203 : 3;
                var lY = i / 2 == 1 ? 303 : 3;
	            var pos = i % 2 == 0 ? 1 : -1;
	            Add(new Box
	                {
	                    XSize = sizeX,
	                    YSize = sizeY,
	                    ZSize = 3,
	                    DefaultColor = wallsColor,
	                    IsStatic = true,
	                    Name = "bordertop",
	                    IsMaterial = true,
	                    Location = new Frame3D(
                            pos * lY/2,
                            pos * lX / 2,
                            3)
	                });
	        }
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
					Name = "RombLeftt",
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
			int RandRombLeftt = rand.Next(1, 4);
			int RandRombRight = rand.Next(1, 4);
			root.Add(new Box
				{
					XSize = 15,
					YSize = 15,
					ZSize = 0.5,
					Location = new Frame3D(-130, 30 - RandRombLeftt * 35, 3, Angle.Zero, Angle.FromGrad(45), Angle.Zero),
					DefaultColor = Color.Cyan,
					IsStatic = true,
                    Name = "RombLefttRandom",
				});
			root.Add(new Box
				{
					XSize = 15,
					YSize = 15,
					ZSize = 0.5,
					Location = new Frame3D(130, 30 - RandRombRight * 35, 3, Angle.Zero, Angle.FromGrad(45), Angle.Zero),
					DefaultColor = Color.Cyan,
					IsStatic = true,
                    Name = "RombRightRandom",
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
					DefaultColor = Color.DarkGreen,
					IsMaterial = true,
					IsStatic = true,
					Name = "Cylinder",
				});
			double radius = 75;
			for(int i = 0; i < 6; i++)
			{
				Angle AngleCircle = Angle.FromGrad(60 * i);
				Add(new Cylinder
					{
						RTop = 3,
						RBottom = 3,
						Height = 40,
						Location = new Frame3D(radius * Math.Sin(AngleCircle.Radian), radius * Math.Cos(AngleCircle.Radian), 3),
						DefaultColor = Color.Gray,
						IsMaterial = true,
						IsStatic = true,
						Name = "Cylinder",
					});
			}
		}

		private void CreateTreasure(Body root)
		{
            root.Add(new Box
                {
                    XSize = 8,
                    YSize = 8,
                    ZSize = 8,
                    Location = new Frame3D(50, 80, 3),
                    DefaultColor = Color.YellowGreen,
                    IsMaterial = true,
                    Name = "Emerald",
                    FrictionCoefficient = BoxFrictionCoefficient,
                    Density = Density.Aluminum,
                });
            root.Add(new Box
                {
                    XSize = 8,
                    YSize = 8,
                    ZSize = 8,
                    Location = new Frame3D(-50, 80, 3),
                    DefaultColor = Color.YellowGreen,
                    IsMaterial = true,
                    Name = "Emerald",
                    FrictionCoefficient = BoxFrictionCoefficient,
                    Density = Density.Aluminum,
                });
  			root.Add(new Box
				{
					XSize = 8,
					YSize = 8,
					ZSize = 8,
					Location = new Frame3D(50, -80, 3),
					DefaultColor = Color.Red,
					IsMaterial = true,
					Name = "Ruby",
                    FrictionCoefficient = BoxFrictionCoefficient,
                    Density = Density.Aluminum,
				});
		    root.Add(new Box
				{
					XSize = 8,
					YSize = 8,
					ZSize = 8,
					Location = new Frame3D(-50, -80, 3),
					DefaultColor = Color.Red,
					IsMaterial = true,
					Name = "Ruby",
                    FrictionCoefficient = BoxFrictionCoefficient,
                    Density = Density.Aluminum
				});
			double radius = 60;
            var rand = new Random();

			for(int k = -1; k < 2; k += 2)
			{
                var gems = new List<Tuple<string, Color>>
                    {
                        new Tuple<string, Color>("Ruby", Color.Red),
                        new Tuple<string, Color>("Emerald", Color.YellowGreen),
                        new Tuple<string, Color>("Diamond", Color.FromArgb(10,255,255,255)),
                        new Tuple<string, Color>("Diamond", Color.FromArgb(10,255,255,255)),
                        null,
                        null,
                    };
				for(int i = 0; i < 6; i++)
				{
				    var ind = rand.Next(gems.Count);
				    var gem = gems[ind];
                    gems.RemoveAt(ind);
				    if(gem == null) continue;
					Angle AngleCircle = Angle.FromGrad(15 + 30 * i);
					Add(new Box
                    {
                        XSize = 8,
                        YSize = 8,
                        ZSize = 8,
                        Location = new Frame3D(radius * k* Math.Sin(AngleCircle.Radian), radius * Math.Cos(AngleCircle.Radian), 3),
                        DefaultColor = gem.Item2,
                        IsMaterial = true,
                        Name = gem.Item1,
                        FrictionCoefficient = BoxFrictionCoefficient

                    });
				}
			}
		}
	}
}
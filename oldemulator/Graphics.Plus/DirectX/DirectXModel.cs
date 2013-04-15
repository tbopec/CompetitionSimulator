using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using Eurosim.Core;
using SlimDX;
using SlimDX.Direct3D9;

namespace Eurosim.Graphics.DirectX
{
	///<summary>
	///Модель для отрисовки. Содержит Mesh, текстуры и материал(ы).
	/// </summary>
	public class DirectXModel : IDisposable
	{
		public DirectXModel()
		{
			ModifierMatrix = Matrix.Identity;
		}

		/// <summary>
		/// Загрузить модель из файла используя FilePath
		/// </summary>
		public static DirectXModel FromFile(string modelName, Device device)
		{
			if(device == null)
				throw new Exception("Must have a Device to create model");
			var drmodel = new DirectXModel();
			using (var stream = GetResourceStream(modelName + FileExtension))
			{
				if(stream != null)
					drmodel.Mesh = Mesh.FromStream(device, stream, MeshFlags.Managed);
				else if(modelName == "teapot")
					drmodel = MakeTeapot(device);
				else
					throw new FileNotFoundException("Model File Not Found!");
			}
			for(int i = 0; i < drmodel.Mesh.GetMaterials().Length; i++)
			{
				ExtendedMaterial mat = drmodel.Mesh.GetMaterials()[i];
				if(string.IsNullOrEmpty(mat.TextureFileName)) 
					continue;
				using (var textureStream = GetResourceStream(mat.TextureFileName))
					drmodel.Textures.Add(i, Texture.FromStream(device, textureStream));
				
			}
			return drmodel;
		}

		public void Dispose()
		{
			Mesh.Dispose();
			foreach(var texture in Textures)
				texture.Value.Dispose();
		}

		/// <summary>
		/// Cоздать модель из примитивного Shape-а
		/// </summary>
		/// <param name="body"></param>
		/// <param name="device"> </param>
		/// <returns></returns>
		public static DirectXModel FromShape(PrimitiveBody body, Device device)
		{
			Shape shape = body.Shape;
			const int SLICES = 32;
			const int STACKS = 8;
			if(device == null)
				throw new Exception("Must have a Device to create model");
			var drmodel = new DirectXModel();
			if(shape as BallShape != null)
			{
				var ball = shape as BallShape;
				drmodel.Mesh = Mesh.CreateSphere(device, (float)ball.Radius, SLICES, SLICES);
				drmodel.ModifierMatrix = Matrix.Translation(0, 0, (float)ball.Radius);
			}
			else if(shape as BoxShape != null)
			{
				var box = (BoxShape)shape;
				drmodel.Mesh = Mesh.CreateBox(device, (float)box.Xsize, (float)box.Ysize, (float)box.Zsize);
				drmodel.ModifierMatrix = Matrix.Translation(0, 0, (float)(box.Zsize / 2));
			}
			else if(shape as CyllinderShape != null)
			{
				var cyl = (CyllinderShape)shape;
				drmodel.Mesh = Mesh.CreateCylinder(device, (float)cyl.Rbottom,
				                                   (float)cyl.Rtop, (float)cyl.Height, SLICES, STACKS);
				drmodel.ModifierMatrix = Matrix.Translation(0, 0, (float)(cyl.Height / 2));
			}
			else if(shape as RectangleShape != null)
			{
				var rect = (RectangleShape)shape;
				drmodel.Mesh = Mesh.CreateBox(device, (float)rect.Xsize, (float)rect.Ysize, 0.01f);
			}
			drmodel.Mesh.SetMaterials(new[]
			                          	{
			                          		new ExtendedMaterial
			                          			{
			                          				MaterialD3D = new Material
			                          				              	{
			                          				              		Ambient = body.Color,
			                          				              		Diffuse = body.Color, Specular = Color.White, Power = 15f
			                          				              	}
			                          			}
			                          	});
			return drmodel;
		}

		/// <summary>
		///Используем чтобы привести загруженную модель к нужному масштабу, повернуть 
		/// </summary>
		public Matrix ModifierMatrix { get; private set; }

		public Mesh Mesh { get; private set; }
		public readonly Dictionary<int, Texture> Textures = new Dictionary<int, Texture>();

		public const string FileExtension = ".x";


		/// <summary>
		/// Загрузить ресурс; возвращает поток содержащий модель
		/// </summary>
		/// <param name="filePath">Путь относительно папки models</param>
		/// <returns></returns>
		private static Stream GetResourceStream(string filePath)
		{
			Assembly asm = Assembly.GetExecutingAssembly();
			Stream stream = asm.GetManifestResourceStream("Eurosim.Graphics.Models." + filePath);
			//if (stream == null)
			//	throw new Exception("Resource not found");
			return stream;
		}
		/// <summary>	
		/// Тестовый чайник
		/// </summary>
		/// <param name="device"> </param>
		private static DirectXModel MakeTeapot(Device device)
		{
			var m = new DirectXModel();
			m.Mesh = Mesh.CreateTeapot(device);
			m.Mesh.SetMaterials(new[]
			{
				new ExtendedMaterial
			     {
					MaterialD3D = new Material
						{
							Diffuse = Color.Red,
							Ambient = Color.Red,
							Specular = Color.Red,
							Power = 50
						}
				 }
			});
			m.ModifierMatrix = Matrix.Scaling(10, 10, 10);
			return m;
		}
	}
}
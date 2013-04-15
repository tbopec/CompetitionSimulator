using System;
using System.Windows.Media.Media3D;
using Eurosim.Core;
using SolidColorBrush = System.Windows.Media.SolidColorBrush;

namespace Eurosim.Graphics.WPF
{
	internal class WPFModel : IDisposable
	{
		public WPFModel(Model3DGroup mainModelGroup)
		{
			MainModelGroup = mainModelGroup;
		}

		public void Dispose()
		{
		}

		public void AddToScreen()
		{
			MainModelGroup.Children.Add(RealWPFModel);
			OnScreen = true;
		}

		public void RemoveFromScreen()
		{
			MainModelGroup.Children.Remove(RealWPFModel);
			OnScreen = false;
		}

		public static bool TryCreate(Body body, Model3DGroup mainModelGroup, out WPFModel wpfModel)
		{
			GeometryModel3D geometry;
			bool res = GeometryFactory.TryGetResult(body, out geometry);
			if(res)
			{
				wpfModel = new WPFModel(mainModelGroup)
				           	{
				           		RealWPFModel = geometry
				           	};
			}
			else
				wpfModel = null;
			return res;
		}

		public bool OnScreen { get; private set; }
		public Model3D RealWPFModel { get; private set; }
		public Model3DGroup MainModelGroup { get; private set; }
		private static readonly WPFGeometryFactory GeometryFactory = new WPFGeometryFactory();
	}

	internal class WPFGeometryFactory : ModelFactory<GeometryModel3D>
	{
		public override void Visit(Box visitable)
		{
			InternalResult = new GeometryModel3D(Primitives.CreateBoxMesh(visitable.XSize,
			                                                              visitable.YSize, visitable.ZSize, 8), GetMaterial(visitable));
		}

		public override void Visit(Ball visitable)
		{
			InternalResult = new GeometryModel3D(Primitives.CreateSphereMesh(visitable.Radius, 32), GetMaterial(visitable));
		}

		public override void Visit(Cylinder cylinder)
		{
			InternalResult = new GeometryModel3D(Primitives.CreateCylinderMesh(cylinder.RBottom,
				cylinder.RTop, cylinder.Height), GetMaterial(cylinder));
		}

		public override void Visit(Body visitable)
		{
		}

		private static Material GetMaterial(Body visitable)
		{
			return new DiffuseMaterial(new SolidColorBrush(visitable.DefaultColor.ToWPFColor()));
		}

	}
}
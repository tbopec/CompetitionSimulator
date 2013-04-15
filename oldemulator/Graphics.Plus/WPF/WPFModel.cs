using System;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Eurosim.Core;

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

		public static WPFModel FromShape(PrimitiveBody body, Model3DGroup mainModelGroup)
		{
			if(body.Shape == null)
				return null;
			var wm = new WPFModel(mainModelGroup) {RealWPFModel = new GeometryModel3D()};
			var material = new DiffuseMaterial(new SolidColorBrush(body.Color.ToWPFColor()));
			Shape shape = body.Shape;
			if(shape as BoxShape != null)
			{
				var box = shape as BoxShape;
				wm.RealWPFModel = new GeometryModel3D(Primitives.CreateBoxMesh(box.Xsize, box.Ysize, box.Zsize, 8), material);
			}
			else if(shape as CyllinderShape != null)
			{
				var cyl = shape as CyllinderShape;
				wm.RealWPFModel = new GeometryModel3D(Primitives.CreateCylinderMesh(cyl.Rtop, cyl.Rbottom, cyl.Height), material);
			}
			else if(shape as BallShape != null)
			{
				var sph = shape as BallShape;
				wm.RealWPFModel = new GeometryModel3D(Primitives.CreateSphereMesh(sph.Radius, 32), material);
			}
			return wm;
		}

		public bool OnScreen { get; private set; }
		public Model3D RealWPFModel { get; private set; }
		public Model3DGroup MainModelGroup { get; private set; }
	}
}
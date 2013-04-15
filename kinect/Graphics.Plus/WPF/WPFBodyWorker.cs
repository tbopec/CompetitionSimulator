using System.Collections.Generic;
using System.Windows.Media.Media3D;
using Eurosim.Core;

namespace Eurosim.Graphics.WPF
{
	internal class WPFBodyWorker : DrawingBodyWorker<WPFModel, Matrix3D>
	{
		public WPFBodyWorker(Model3DGroup mainModelGroup)
		{
			_mainModelGroup = mainModelGroup;
		}

		public override void UpdateModels(Body root, Body doNotDraw)
		{
			base.UpdateModels(root, doNotDraw);
			_modelsToRemove.ForEach(x => x.RemoveFromScreen());
			_modelsToRemove.Clear();
			WPFModel mod;
			if(Models.TryGetValue(root, out mod) && !mod.OnScreen)
				mod.AddToScreen();
		}

		protected override bool TryCreateModelForBody(Body body, out WPFModel model)
		{
			bool res = WPFModel.TryCreate(body, _mainModelGroup, out model);
			body.ChildRemoved += ChildRemovedFromTree;
			return res;
		}

		protected override void AddModel(Body body, WPFModel mod)
		{
			base.AddModel(body, mod);
			mod.AddToScreen();
		}

		protected override void DrawModel(WPFModel model)
		{
		}

		protected override Matrix3D ApplyTransformMatrix(Body root)
		{
			var transform = new Transform3DGroup(); //transform от данного тела
			transform.Children.Add(
				new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0, 0, 1), root.Location.Yaw.Grad)));
			transform.Children.Add(new TranslateTransform3D(root.Location.ToWPFVector()));
			_transformMatrix = transform.Value * _transformMatrix;
			WPFModel model;
			if(Models.TryGetValue(root, out model))
				model.RealWPFModel.Transform = new MatrixTransform3D(_transformMatrix);
			return _transformMatrix;
		}

		protected override void ResetTransformMatrix(Matrix3D matrix)
		{
			_transformMatrix = matrix;
		}

		private void ChildRemovedFromTree(Body child)
		{
			foreach(Body child2 in child.GetSubtreeChildrenFirst())
			{
				WPFModel model;
				if(Models.TryGetValue(child2, out model))
				{
					_modelsToRemove.Add(model);
					Models.Remove(child2);
				}
			}
		}

		private readonly List<WPFModel> _modelsToRemove = new List<WPFModel>();
		private Matrix3D _transformMatrix;
		private readonly Model3DGroup _mainModelGroup;
	}
}
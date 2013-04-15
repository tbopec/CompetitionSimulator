using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Media3D;
using Eurosim.Core;

namespace Eurosim.Graphics.WPF
{
	internal class WPFBodyWorker : DrawingBodyWorker<WPFModel, Matrix3D>
	{
		public WPFBodyWorker(Model3DGroup mainModelGroup)
		{
			_mainModelGroup = mainModelGroup;
			Body.Removed += RemoveModel;
		}

		protected override bool TryCreateModelForBody(PrimitiveBody primBody, out WPFModel model)
		{
			model = WPFModel.FromShape(primBody, _mainModelGroup);
			return true;
		}

		protected override void AddModel(WPFModel mod, PrimitiveBody primBody)
		{
			base.AddModel(mod, primBody);
			mod.AddToScreen();
		}

		public override void UpdateModels(Body root, Body doNotDraw)
		{
			base.UpdateModels(root, doNotDraw);
			foreach(var model in _modelsToRemove.ToList())
			{
				_modelsToRemove.Remove(model);
				model.RemoveFromScreen();
			}
			WPFModel mod;
			if(root is PrimitiveBody && Models.TryGetValue(root as PrimitiveBody, out mod) &&
			   mod.OnScreen == false)
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
			if(root is PrimitiveBody)
			{
				var pRoot = root as PrimitiveBody;
				WPFModel model;
				if(Models.TryGetValue(pRoot, out model))
					model.RealWPFModel.Transform = new MatrixTransform3D(_transformMatrix);
			}
			return _transformMatrix;
		}

		protected override void ResetTransformMatrix(Matrix3D matrix)
		{
			_transformMatrix = matrix;
		}

		private void RemoveModel(Body child, Body oldParent, Body newParent)
		{
			if(newParent != null) 
				return;
			foreach(PrimitiveBody child2 in child.GetSubtreeChildrenFirst().OfType<PrimitiveBody>())
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
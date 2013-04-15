using System.Collections.Generic;
using System.Linq;
using AIRLab.Mathematics;
using Eurosim.Core;
using Matrix = System.Drawing.Drawing2D.Matrix;

namespace Eurosim.Graphics.Winforms
{	
	public sealed class WinformsScene : DrawingBodyWorker<WinformsModel, Matrix>
	{
		public WinformsScene(Body root)
		{
			Models = new Dictionary<PrimitiveBody, WinformsModel>(new WinformsComparer());
			InitializeModels(root);
		}

		public override void UpdateModels(Body root, Body doNotDraw)
		{
			if (root == null || root.Equals(doNotDraw))
				return;
			lock (root.LockObject)
			{
				var bodies = new List<PrimitiveBody>
					(root.GetSubtreeChildrenFirst().OfType<PrimitiveBody>());
				bodies.Sort((a, b) => a.Location.Z.CompareTo(b.Location.Z));
				foreach (PrimitiveBody body in bodies)
				{
					Matrix tr = Graphics.Transform;
					Graphics.MultiplyTransform(GetMatrix(body.GetAbsoluteLocation()));
					WinformsModel model;
					if (Models.TryGetValue(body, out model))
						model.Draw(Graphics);
					else
						InitializeModels(body);
					Graphics.Transform = tr;
				}
			}
		}

		public System.Drawing.Graphics Graphics { get; set; }

		protected override bool TryCreateModelForBody(PrimitiveBody primBody, out WinformsModel model)
		{
			model = !string.IsNullOrEmpty(primBody.TopViewFileName)
			        	? WinformsModel.FromImage(primBody)
			        	: WinformsModel.FromShape(primBody);
			return true;
		}

		private static Matrix GetMatrix(Frame3D loc)
		{
			var m = new Matrix();
			m.Translate((float)loc.X, -(float)loc.Y);
			m.Rotate(-(float)loc.Yaw.Grad);
			return m;
		}

		private class WinformsComparer : IEqualityComparer<PrimitiveBody>
		{
			public bool Equals(PrimitiveBody primitiveBody, PrimitiveBody other)
			{
				if (ReferenceEquals(null, other)) return false;
				if (ReferenceEquals(primitiveBody, other)) return true;
				return Equals(other.Shape, primitiveBody.Shape) 
					&& other.Color.RgbEquals(primitiveBody.Color)
					&& Equals(other.TopViewFileName, primitiveBody.TopViewFileName);
			}

			public int GetHashCode(PrimitiveBody primitiveBody)
			{
				unchecked
				{
					int result = primitiveBody.Color.GetHashCode();
					result = (result * 397) ^ (primitiveBody.Shape != null ? primitiveBody.Shape.GetHashCode() : 0);
					result = (result * 397) ^ (primitiveBody.TopViewFileName != null ? primitiveBody.TopViewFileName.GetHashCode() : 0);
					return result;
				}
			}
		}
	}
}
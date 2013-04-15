using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Eurosim.Core;
using SlimDX;
using SlimDX.Direct3D9;

namespace Eurosim.Graphics.DirectX
{
	/// <summary>
	/// Сцена в DirectX - трехмерное представление дерева тел
	/// </summary>
	public sealed partial class DirectXScene : DrawingBodyWorker<DirectXModel, Matrix>
	{
		public DirectXScene(Body root, Body floor)
		{
			RootBody = root;
			Floor = floor;//TODO. get rid of floor by determining intersection
			List<Light> lights =
				SceneConfig.Lights.Select(lightSetting => lightSetting.ToDirectXLight()).ToList();
			DeviceWorker = new DeviceWorker();
			DeviceWorker.Disposing += Dispose;
			Effect = new DefaultEffect(DeviceWorker.Device, lights);
			DeviceWorker.AfterReset += Effect.Reset;
			Models = new Dictionary<PrimitiveBody, DirectXModel>(new BodyComparer());
			_stopwatch.Start();
			InitializeModels(root);
			_stopwatch.Stop();
			LogInfo("Models initialized in {0} ms", _stopwatch.ElapsedMilliseconds);
		}

		public DirectXScene(Body rootBody):this(rootBody, null)
		{
		}

		private void Dispose()
		{
			Effect.Dispose();
			DisposeModels();
			LogInfo("Scene disposed");
		}

		public Effect Effect { get; private set; }
		internal DeviceWorker DeviceWorker { get; private set; }
		public Body RootBody { get; private set; }
		public Body Floor { get; private set; }

		protected override bool TryCreateModelForBody(PrimitiveBody primBody, out DirectXModel model)
		{
			model = null;
			if (!string.IsNullOrEmpty(primBody.ModelFileName))
			{
				model = DirectXModel.FromFile(primBody.ModelFileName, DeviceWorker.Device);
				return true;
			}
			if (primBody.Shape != null)
			{
				model = DirectXModel.FromShape(primBody, DeviceWorker.Device);
				return true;
			}
			return false;
		}


		protected override void DrawModel(DirectXModel model)
		{
			Effect.DrawModel(model);
		}

		protected override void ResetTransformMatrix(Matrix m)
		{
			Effect.WorldTransform = m;
		}

		protected override Matrix ApplyTransformMatrix(Body root)
		{
			Effect.MultiplyWorldTransform(root.GetRelativeLocationMatrix());
			return Effect.WorldTransform;
		}

		private static void LogInfo(string message, params object[] args)
		{
			Console.WriteLine(message, args);
		}

		private readonly Stopwatch _stopwatch = new Stopwatch();
		
		/// <summary>
		/// Comparer. В графике тела считаем одинаковыми если у них одинаковые 
		/// Shape, Color и имена модели.
		/// </summary>
		private class BodyComparer : IEqualityComparer<PrimitiveBody>
		{
			public bool Equals(PrimitiveBody primitiveBody, PrimitiveBody other)
			{
				if (ReferenceEquals(null, other)) return false;
				if (ReferenceEquals(primitiveBody, other)) return true;
				return (other.Shape != null && other.Shape.Equals(primitiveBody.Shape)) &&
				       other.Color.RgbEquals(primitiveBody.Color) &&
				       Equals(other.ModelFileName, primitiveBody.ModelFileName);
			}

			public int GetHashCode(PrimitiveBody pb)
			{
				unchecked
				{
					int result = (pb.Shape != null ? pb.Shape.GetHashCode() : 0);
					result = (result*397) ^ pb.Color.GetHashCode();
					result = (result*397) ^ (pb.ModelFileName != null ? pb.ModelFileName.GetHashCode() : 0);
					return result;
				}
			}
		}
	}
}
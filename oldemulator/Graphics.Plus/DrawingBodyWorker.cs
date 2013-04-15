using System;
using System.Collections.Generic;
using Eurosim.Core;

namespace Eurosim.Graphics
{
	public abstract class DrawingBodyWorker<TModel, TTransform> where TModel : IDisposable
	{
		/// <summary>
		/// По корневому телу создаем графические модели для всех тел
		/// Первый раз вызываем для Drawer.RootBody
		/// </summary>
		/// <param name="root">Текущее корневое тело</param>
		public virtual void InitializeModels(Body root)
		{
			if(root is PrimitiveBody)
			{
				var primBody = root as PrimitiveBody;
				TModel mod;
				if(!Models.ContainsKey(primBody) &&
				   TryCreateModelForBody(primBody, out mod))
					AddModel(mod, primBody);
			}
			lock(root.LockObject)
				foreach(Body body in root.Nested)
					InitializeModels(body);
		}

		/// <summary>
		/// Рисуем модели, используя Location соответствующих им тел.
		/// Первый раз вызываем для Drawer.RootBody, затем рекурсивно.
		/// </summary>
		/// <param name="root">Текущее корневое тело</param>
		/// <param name="doNotDraw">Список тел, которые НЕ отрисовываются</param>
		public virtual void UpdateModels(Body root, Body doNotDraw)
		{
			if(root == null || root.Equals(doNotDraw))
				return;
			TTransform m = ApplyTransformMatrix(root);
			if(root is PrimitiveBody)
			{
				var primRoot = root as PrimitiveBody;
				TModel model;
				if(Models.TryGetValue(primRoot, out model))
					DrawModel(model);
				else
					InitializeModels(primRoot);
			}
			lock(root.LockObject)
				foreach(Body body in root.Nested)
				{
					UpdateModels(body, doNotDraw);
					ResetTransformMatrix(m);
				}
		}

		public void UpdateModels(Body root)
		{
			UpdateModels(root, null);
		}

		/// <summary>
		/// Избавляемся от ресурсов выделенных для моделей
		/// </summary>
		public void DisposeModels()
		{
			foreach (var pair in Models)
				pair.Value.Dispose();
		}

		public Dictionary<PrimitiveBody, TModel> Models = new Dictionary<PrimitiveBody, TModel>();

		protected abstract bool TryCreateModelForBody(PrimitiveBody primBody,
		                                              out TModel model);

		protected virtual void AddModel(TModel mod, PrimitiveBody primBody)
		{
			Models.Add(primBody, mod);
		}

		protected virtual void DrawModel(TModel model)
		{
		}

		protected virtual TTransform ApplyTransformMatrix(Body root)
		{
			return default(TTransform);
		}

		protected virtual void ResetTransformMatrix(TTransform matrix)
		{
		}
	}
}
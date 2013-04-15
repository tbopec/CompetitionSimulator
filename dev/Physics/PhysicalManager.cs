using System;
using System.Collections.Generic;
using Eurosim.Core;
using Eurosim.Core.Physics;
using Eurosim.Physics.Farseer;
using System.Linq;

namespace Eurosim.Physics
{
	public enum PhysicalEngines { No, Farseer, Bepu }

	/// <summary>
	/// Статический класс, работающий с соответсвующим движком. 
	/// </summary>
	public static class PhysicalManager
	{
		/// <summary>
		/// Какой физ. движок используется в данный момент.
		/// </summary>
		private static PhysicalEngines _currentEngine = PhysicalEngines.No;

		/// <summary>
		/// Какой физ. движок используется в данный момент.
		/// </summary>
		public static PhysicalEngines CurrentEngine { get { return _currentEngine; } }

		/// <summary>
		/// Является ли текущий движок 2х мерным. 
		/// </summary>
		public static bool Is2d { get { return (_currentEngine == PhysicalEngines.Farseer); } }

		private static IWorld _world = null;

		private static readonly Dictionary<Body, IPhysical> BodiesToPhysical = new Dictionary<Body, IPhysical>(32); 
		private static readonly BodyCreatorVisitor BodyCreatorVisitor = new BodyCreatorVisitor();

		/// <summary>
		/// Инициализация движка
		/// </summary>
		/// <param name="physicalEngine">Движок, который будет использован</param>
		/// <param name="world">Мир этого движка</param>
		/// <param name="root">Мир тел</param>
		public static void InitializeEngine(PhysicalEngines physicalEngine, IWorld world, Body root)
		{
			_currentEngine = physicalEngine;
			_world = world;

			root.ChildAdded += BodyChildAdded;
			root.ChildRemoved += BodyChildRemoved;

			foreach (var body in root.GetSubtreeChildrenFirst())
			{
				if (body != root)
					AcquireBody(body);
			}
		}

		/// <summary>
		/// Произвести операцию в физ. движке и обновить положения тел
		/// </summary>
		/// <param name="dt"></param>
		/// <param name="root"></param>
		public static void MakeIteration(double dt, Body root)
		{
			_world.MakeIteration(dt, root);

			foreach (KeyValuePair<Body, IPhysical> pair in BodiesToPhysical)
			{
				if (pair.Value == null || FindFirstMaterialParent(pair.Key) != null) //todo remove FindFirstMaterialParent, optimize
					continue;

				var body = pair.Key;
				var physical = pair.Value;

				physical.SuprressPropertyChangedEvents = true;
				body.Location = physical.Location;
				body.Velocity = physical.Velocity;
				physical.SuprressPropertyChangedEvents = false;
			}
		}

		//---------------------------------------------------------------------

		#region Saving bodies		

		/// <summary>
		/// Повесим на тело нужные event-ы, создадим физическое тело и запомним его
		/// </summary>
		/// <param name="body"></param>
		private static void AcquireBody(Body body)
		{
			body.ChildAdded += BodyChildAdded;
			body.ChildRemoved += BodyChildRemoved;

			if (!body.IsMaterial)
			{
				SaveBody(body, null);
				return;
			}

			body.PropertyChanged += BodyPropertyChanged;			
			body.AcceptVisitor(BodyCreatorVisitor);
		}

		/// <summary>
		/// Свяжем тело и физическое тело
		/// </summary>
		internal static void SaveBody(Body body, IPhysical physical)
		{
			if (BodiesToPhysical.ContainsKey(body))
				BodiesToPhysical[body] = physical;
			else
				BodiesToPhysical.Add(body, physical);
		}

		/// <summary>
		/// Найти первого материального предка
		/// </summary>
		/// <param name="body">тело, чей предок ищем</param>
		/// <returns>материальный предок или null, если такого нет</returns>
		private static Body FindFirstMaterialParent(Body body)
		{
			return body.GetParents().FirstOrDefault(x => x.IsMaterial);
		}

		#endregion

		#region	Events

		private static void BodyChildAdded(Body body)
		{
			AcquireBody(body);
			Body materialParent = FindFirstMaterialParent(body);

			if (materialParent != null && BodiesToPhysical.ContainsKey(materialParent))
			{
				BodiesToPhysical[materialParent].Attach(BodiesToPhysical[body]);
			}
		}

		private static void BodyChildRemoved(Body body)
		{
			if (!BodiesToPhysical.ContainsKey(body))
				return;
			BodiesToPhysical[body].IsMaterial = false; //todo delete from actual physical world

			Body materialParent = FindFirstMaterialParent(body);

			if (materialParent != null && BodiesToPhysical.ContainsKey(materialParent))
			{
				BodiesToPhysical[materialParent].Detach(BodiesToPhysical[body]);
			}

			body.ChildAdded -= BodyChildAdded;
			body.ChildRemoved -= BodyChildRemoved;
			body.PropertyChanged -= BodyPropertyChanged;

			BodiesToPhysical.Remove(body);
		}

		private static void BodyPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			Body body = (Body)sender;

			if (!BodiesToPhysical.ContainsKey(body))
				return;

			IPhysical physical = BodiesToPhysical[body];
			
			if (physical.SuprressPropertyChangedEvents)
				return;

			switch (e.PropertyName)
			{
				case Body.MaterialPropertyName:
					physical.IsMaterial = body.IsMaterial;
					break;
				case Body.FrictionPropertyName:
					physical.FrictionCoefficient = body.FrictionCoefficient;
					break;
				case Body.VelocityPropertyName:
					physical.Velocity = body.Velocity;
					break;				
			}
		}

		#endregion

		#region Setting settings

		/// <summary>
		/// Настроит тело PhysicalModel в соответствии с телом PhysicalPrimitiveBody
		/// </summary>	
		public static void SetSettings(Body body, IPhysical physical)
		{
			physical.IsStatic = body.IsStatic;
			physical.Location = body.Location;
			physical.Mass = body.Volume / 1000 * ((body.Density == null) ? 1 : body.Density.Value);
			physical.FrictionCoefficient = body.FrictionCoefficient;
			physical.Id = body.Id;
			physical.Velocity = body.Velocity;
		}

		#endregion
		
		#region Making primitives

		public static IPhysical MakeBox(double xsize, double zsize, double ysize)
		{
			return _world.MakeBox(xsize, zsize, ysize);
		}

		public static IPhysical MakeCyllinder(double rbottom, double rtop, double height)
		{
			return _world.MakeCyllinder(rbottom, rtop, height);
		}

		#endregion
	}
}

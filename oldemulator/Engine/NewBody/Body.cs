using System.Collections.Generic;
using System.Linq;
using AIRLab.Mathematics;

namespace Eurosim.Core
{
	/// <summary>
	/// Абстрактное, совершенно произвольное тело
	/// </summary>
	public abstract class Body
	{
		public bool IsWorld
		{
			get { return this is WorldBody; }
		}

		/// <summary>
		/// true, если это  коллекция
		/// </summary>
		public bool IsCollection { get; protected set; }

		/// <summary>
		/// true, если это тело, состоящее из твердо скрепленных друг с другом частей
		/// </summary>
		public bool IsPrimaryBody { get; protected set; }

		/// <summary>
		/// Lock для коллекции Nested 
		/// </summary>
		public readonly object LockObject = new object();

		public int Id { get; private set; }
		private static int _idCounter;

		protected Body()
		{
			Id = _idCounter++;
			Location = new Frame3D();
			Velocity = new Frame3D();

		}

		public virtual Frame3D Location { get; set; }

		public virtual Frame3D Velocity { get; set; }

		public string Name { get; set; }

		public Body Parent { get; internal set; }

		public WorldBody World { get; protected internal set; }

		public abstract IEnumerable<Body> Nested { get; }

		protected internal virtual void RemoveChild(Body body)
		{
		}

		/// <summary>
		/// Тело добавляется в мир
		/// </summary>
		protected internal virtual void BodyAdded()
		{
		}

		/// <summary>
		/// Тело удаляется из мира
		/// </summary>
		protected internal virtual void BodyDeleting()
		{
		}

		//Метод с тяжелой блокировкой. Посмотреть, что не огребаем с производительностью.
		public IEnumerable<Body> GetSubtreeChildrenFirst()
		{
			var stack = new Stack<Body>();
			stack.Push(this);
			SubtreeInternal(this, stack);
			return stack;
		}

		private static void SubtreeInternal(Body root, Stack<Body> stack)
		{
			lock (root.LockObject)
				foreach (var body in root.Nested)
				{
					stack.Push(body);
					SubtreeInternal(body, stack);
				}
		}

		public IEnumerable<Body> GetParents()
		{
			var c = Parent;
			while (c != null)
			{
				yield return c;
				c = c.Parent;
			}
		}

		/// <summary>
		/// Возвращает абсолютные координаты тела
		/// </summary>
		public Frame3D GetAbsoluteLocation()
		{
			var loc = new Frame3D();
			var body = this;
			while (body.Parent != null)
			{
				loc = body.Location.Apply(loc);
				body = body.Parent;
			}
			return loc;
		}

		internal virtual void JoinMe(IPhysicalBody pb, Frame3D realLocation)
		{
		}

		internal virtual void DetachMe(IPhysicalBody pb)
		{
		}

		public static event BodyEventHandler Added;
		public static event BodyEventHandler Removed;

		public delegate void BodyEventHandler(Body child, Body oldParent, Body newParent);

		public virtual void Update()
		{
			Location += Velocity;
		}

		//TODO. Просто рак. Без комментариев.
		protected void RaiseAdded(Body child, Body oldParent, Body newParent)
		{
			if (Added != null)
				Added(child, oldParent, newParent);
		}

		protected void RaiseRemoved(Body child, Body oldParent, Body newParent)
		{
			if (Removed != null)
				Removed(child, oldParent, newParent);
		}
	}
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AIRLab.Mathematics;

namespace Eurosim.Core
{
	//TODO(MK): Избавиться от торчащего наружу лока и при случае подумать над тем, чтобы сделать класс copy-on-write.
    public class BodyCollection<T> : Body, IEnumerable<T>
		where T : Body
    {
        readonly HashSet<T> nested = new HashSet<T>();

        public BodyCollection()
        {
            IsCollection = true;
            IsPrimaryBody=false;
        }

        protected internal override void RemoveChild(Body body)
        {
            ChildRemoving((T)body);
            foreach (var b in body.GetSubtreeChildrenFirst())
                b.BodyDeleting();
            body.Parent = null;
            nested.Remove((T)body);
            foreach (var e in body.GetSubtreeChildrenFirst())
                e.World = null;
        }

        protected virtual void ChildAdded(T child) { }
        protected virtual void ChildRemoving(T child) { }

        public virtual void Add(T body)
        {
            lock (LockObject)
            {
                var oldParent = body.Parent;
                if (body.Parent != null)
                    body.Parent.RemoveChild(body);
                nested.Add(body);
                body.Parent = this;

                foreach (var e in body.GetSubtreeChildrenFirst())
                {
                    e.World = World;
                    if (World != null)
                        e.BodyAdded();
                }
                ChildAdded(body);
                RaiseAdded(body, oldParent, this);
                if (NestedChanged != null)
                    NestedChanged(body, true);
            }
        }

		public virtual void Remove(T body)
        {
            lock (LockObject)
                RemoveInternal(body);
        }

    	private void RemoveInternal(T body)
    	{
    		RemoveChild(body);
    		RaiseRemoved(body, this, null);
    		if (NestedChanged != null)
    			NestedChanged(body, false);
    	}

    	/// <summary>
    	/// Очистить коллекцию(циклом удалить из нее все тела)
    	/// </summary>
    	public void Clear()
    	{
    		lock (LockObject)
    		{
    			var list = new List<T>(nested);
    			foreach (var child in list)
    				RemoveInternal(child);
    		}
    	}


    	/// <summary>
		/// Присоединит примитивное тело к PrimaryBody, к которому присоединена эта колекция, и посчитает оффсет тела от location этой коллекции.
		/// </summary>		
		internal override void JoinMe(IPhysicalBody pb, Frame3D realLocation)
		{
			// Так как это тело не PrimaryBody, то прокидываем наверх.
			if (Parent != null)
			{
				Parent.JoinMe(pb, Location.Apply(realLocation));
			}
		}

		/// <summary>
		/// Отсодеинит примитивное тело от PrimaryBody, к которому присоединена эта колекция. 
		/// </summary>		
		internal override void DetachMe(IPhysicalBody pb)
		{
			// Так как это тело не PrimaryBody, то прокидываем наверх.
			if (Parent != null)
			{
				Parent.DetachMe(pb);
			}
		}

        public override IEnumerable<Body> Nested
        {
            get { return nested; }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return nested.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return nested.GetEnumerator();
        }

        public delegate void NestedChangedHandler(Body child, bool added);
        public event NestedChangedHandler NestedChanged;
    }

}

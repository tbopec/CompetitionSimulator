using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eurosim.Core.Physics;

namespace Eurosim.Core
{
    public interface IPhysicalBody
    {
		IPhysical PhysicalModel { get; }
		void UpdateLocation();
        event CollisionHandler Collision;
    }
    public delegate void CollisionHandler(IPhysicalBody a, PhysicalPrimitiveBody b); 
}

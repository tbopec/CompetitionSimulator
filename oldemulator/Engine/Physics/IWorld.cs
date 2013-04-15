using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eurosim.Core;

namespace Eurosim.Core.Physics
{
    public interface IWorld
	{
		/// <summary>
		/// Просчёт в физического мира, изменившегося на время dt
		/// </summary>
		void MakeIteration(double dt, BodyCollection<Body> root);

		IPhysical MakeBox(double xsize, double ysize, double zsise);
		IPhysical MakeCyllinder(double rbottom, double rtop, double height);
	}
}

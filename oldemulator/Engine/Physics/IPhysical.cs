using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AIRLab.Mathematics;

namespace Eurosim.Core.Physics
{
	/// <summary>
	/// Объект физического движка.
	/// </summary>
	public interface IPhysical
	{
		int Id { get; set; }
		bool IsMaterial { get; set; }
		bool IsStatic { get; set; }
		bool FloorFrictionEnabled { get; set; } 
		double Mass { get; set; }
		double FrictionCoefficient { get; set; }
		Frame3D Location { get; set; }
		Frame3D Velocity { get; set; }		 
		//PhysicalPrimitive PhysicalPrimitive { get; set; }
		/// <summary>
		/// Присоеднит переданный объект, используя его Location. Для упрощения можно отключить трение с полом.
		/// </summary>		
		void JoinWith(IPhysical body, bool joinWithFloorFriction = true);
		/// <summary>
		/// Присоеднит переданный объект, используя переданный Location. Для упрощения можно отключить трение с полом.
		/// </summary>		
		void JoinWith(IPhysical body, Frame3D realLocation, bool joinWithFloorFriction = true);
		void Detach(IPhysical body);

		/// <summary>
		/// Для объектов 3д движков, чтобы они двигались только в 2д. (Чтоб робот не кувыркался)  
		/// </summary>
		bool ActAs2d { get; set; }

        /// <summary>
        /// Ссылка на соответствующее тело эмулятора
        /// </summary>
	    PhysicalPrimitiveBody Body { get; set; }
	}


}

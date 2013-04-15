namespace Eurosim.Core.Physics
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
		static private PhysicalEngines _currentEngine = PhysicalEngines.No;

		/// <summary>
		/// Какой физ. движок используется в данный момент.
		/// </summary>
		static public PhysicalEngines CurrentEngine { get { return _currentEngine; } }

		/// <summary>
		/// Является ли текущий движок 2х мерным. 
		/// </summary>
		static public bool Is2d { get { return (_currentEngine == PhysicalEngines.Farseer); } }

		static private IWorld _world = null;

		static public void InitializeEngine(PhysicalEngines pe, IWorld wo)
		{
			_currentEngine = pe;
		    _world = wo;
		}

		static public void MakeIteration(double dt, BodyCollection<Body> root)
		{
			_world.MakeIteration(dt, root);
			//TODO сюда updateAll
		}

		
        #region Setting settings
		/// <summary>
		/// Настроит тело PhysicalModel в соответствии с телом PhysicalPrimitiveBody
		/// </summary>	
		static public void SetSettings(PhysicalPrimitiveBody body)
        {
			if (body.Shape == null)
				return;

            body.PhysicalModel = body.Shape.GetPhysicalModel();
			// Тут не устанавливается IsMaterial, т.к. изначально тело не материально. Оно станет материально при добавлении в World.
			body.PhysicalModel.IsStatic = body.IsStatic;
            body.PhysicalModel.Location = body.Location;
            body.PhysicalModel.Mass = body.Mass;
            body.PhysicalModel.FrictionCoefficient = body.FrictionCoefficient;
            body.PhysicalModel.Id = body.Id;
		}

		#endregion
        
		#region Making primitives

		static public IPhysical MakeBox(double xsize, double zsize, double ysize)
		{
			return _world.MakeBox(xsize, zsize, ysize);
		}

		static public IPhysical MakeCyllinder(double rbottom, double rtop, double height)
		{
			return _world.MakeCyllinder(rbottom, rtop, height);
		}

		#endregion

	}
}

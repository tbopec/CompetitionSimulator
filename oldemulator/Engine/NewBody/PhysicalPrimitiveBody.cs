using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Eurosim.Core.Physics;
using AIRLab.Mathematics;
namespace Eurosim.Core
{
    public class PhysicalPrimitiveBody : PrimitiveBody, IPhysicalBody
    {          
		/// <summary>
		/// Физическое тело
		/// </summary>
		public IPhysical PhysicalModel { get; set; }

		/// <summary>
		/// Общие действия при создании объекта.
		/// </summary>		
		private void Constructing()
		{
            PhysicalManager.SetSettings(this);
			/*PhysicalModel = shape.GetPhysicalModel();
            PhysicalModel.IsStatic = IsStatic;
            PhysicalModel.Location = Location;
            PhysicalModel.Mass = Mass;
            PhysicalModel.FrictionCoefficient = FrictionCoefficient;
            PhysicalModel.Id = Id;*/
			PhysicalModel.IsMaterial = false; // Изначально тело не материально. Оно станет материально при добавлении в World.
		    PhysicalModel.Body = this;
		}

		public PhysicalPrimitiveBody() {}

	    public PhysicalPrimitiveBody(Shape shape, string modelName)
		    : base(shape, modelName)
	    {
		    Constructing();
	    }

	    public PhysicalPrimitiveBody(Shape shape, Color color)
		    : base(shape, color)
	    {
		    Constructing();
	    }

	    public PhysicalPrimitiveBody(Shape shape, Color color, string modelname)
		    : base(shape, color, modelname)
	    {
		    Constructing();
	    }

	    public PhysicalPrimitiveBody(Shape shape, Color color, string modelname,
	                                 string topViewFileName)
		    : base(shape, color, modelname, topViewFileName)
	    {
		    Constructing();
	    }

    	/// <summary>
		/// Обновит свой Location из PhysicalModel
		/// </summary>
		public void UpdateLocation()
		{
            if (PhysicalModel != null)
            {
                _location = PhysicalModel.Location;
            }
		}

        public event CollisionHandler Collision;
        public void RaiseCollisionEvent(PhysicalPrimitiveBody other)
        {
            if (Collision != null)
                Collision(this, other);
            foreach (var parent in GetParents().OfType<PhysicalPrimitiveBody>())
                parent.RaiseCollisionEvent(other);

        }

        protected internal override void BodyAdded()
        {
			//добавляется в мир
			if (Parent != null)
				Parent.JoinMe(this, Location);

	        if (PhysicalModel != null)
				PhysicalModel.IsMaterial = _isMaterial;
        }

        protected internal override void BodyDeleting()
        {
            //удаляется из мира
			if (Parent != null)
				Parent.DetachMe(this);

			if (PhysicalModel != null)
				PhysicalModel.IsMaterial = false;
        }

		//--------------------------------------------------------------------------

		internal override void JoinMe(IPhysicalBody pb, Frame3D realLocation)
		{
			if (this.PhysicalModel != null && pb.PhysicalModel != null && pb.PhysicalModel != this.PhysicalModel)
				PhysicalModel.JoinWith(pb.PhysicalModel, realLocation, true);
		}

		internal override void DetachMe(IPhysicalBody pb)
		{
			if (this.PhysicalModel != null && pb.PhysicalModel != null && pb.PhysicalModel != this.PhysicalModel)
				PhysicalModel.Detach(pb.PhysicalModel);
		}

		//--------------------------------------------------------------------------

		public override void Update()
		{
			
		}

		//--------------------------------------------------------------------------

		//для физики
		#region Свойства

	    private Frame3D _velocity;
		public override Frame3D Velocity
		{
			get
			{
				//if (PhysicalModel != null) //TODO лучше с этим или без?
				//    _location = PhysicalModel.Location;
				return _velocity;
			}
			set
			{
				_velocity = value;
				if (PhysicalModel != null)
					PhysicalModel.Velocity = value;
			}
		}

		private Frame3D _location;
		/// <summary>
		/// Положение данного примитивного тела. 
		/// Если оно является частью ComplexBody, то это его координаты относительно главного тела CompexBody.
		/// </summary>
		public override Frame3D Location
		{
			get
			{
				//if (PhysicalModel != null) //TODO лучше с этим или без?
				//    _location = PhysicalModel.Location;
				return _location;
			}
			set
			{
				_location = value;
				if (PhysicalModel != null)
					PhysicalModel.Location = value;
			}
		}

		private bool _isMaterial = true;
		public bool IsMaterial
		{
			get
			{
				return _isMaterial;
			}
            set
            {
                if (PhysicalModel == null)
                    return;
                //Костыль. если тело было нематериальным, но стало, Location нужно обновить. М.К.
                if (_isMaterial == false && value)
                    PhysicalModel.Location = GetAbsoluteLocation();
                _isMaterial = value;
                PhysicalModel.IsMaterial = value;
            }
		}

		private bool _isStatic;
		public bool IsStatic
		{
			get
			{
				return _isStatic;
			}
			set
			{
				_isStatic = value;
				if (PhysicalModel != null)
					PhysicalModel.IsStatic = value;
			}
		}

		private double _mass;
		public double Mass
		{
			get
			{
				return _mass;
			}
			set
			{
				_mass = value;
				if (PhysicalModel != null)
					PhysicalModel.Mass = value;
			}
		}

		private double _frictionCoefficient;
		public double FrictionCoefficient
		{
			get
			{
				return _frictionCoefficient;
			}
			set
			{
				_frictionCoefficient = value;
				if (PhysicalModel != null)
					PhysicalModel.FrictionCoefficient = value;
			}
		}

		#endregion
    }
}

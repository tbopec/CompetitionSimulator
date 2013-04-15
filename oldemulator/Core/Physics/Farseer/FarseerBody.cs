﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FBody = FarseerPhysics.Dynamics.Body;
using Microsoft.Xna.Framework;
using AIRLab.Mathematics;
using FarseerPhysics.Dynamics.Joints;
using Eurosim.Core.Physics;
using FarseerPhysics.Collision;

namespace Eurosim.Core.Physics.FarseerWrap
{
	/// <summary>
	/// Обёртка для тела из движка Farseer.
	/// </summary>
	class FarseerBody : IPhysical
	{
		private static Dictionary<FBody,FarseerBody> FBodyToFarseerBody =new Dictionary<FBody, FarseerBody>();
		
		#region General properties
		/*
		private CollisionHandler _onCollision;
		/// <summary>
		/// При коллизии будет вызван данный делегат и ему будут переданны id столкнувшихся объектов.
		/// </summary>
		public CollisionHandler OnCollision 
		{
			get
			{
				return _onCollision;
			}
			set
			{
				_onCollision = value;
				RealBody.OnCollision += realBody_OnCollision;
			}
		}
		*/
		public bool ActAs2d { get; set; }
		/// <summary>
		/// Тело эмулятора
		/// </summary>
		public PhysicalPrimitiveBody Body { get; set; }


//and it acts!

		public int Id
		{
			get
			{
				return RealBody.BodyId;
			}
			set
			{
				RealBody.BodyId = value;
			}
		}

		private bool _isMaterial = false;
		public bool IsMaterial
		{
			get
			{
				if (RealBody != null)
				{
					_isMaterial = RealBody.Enabled;
				}
				return _isMaterial;
			}
			set
			{
				_isMaterial = value;
				if (RealBody != null)
				{
					RealBody.Enabled = value;
				}
			}
		}

		private bool _isStatic = false;
		public bool IsStatic
		{
			get
			{
				if (RealBody != null)
				{
					_isStatic = (RealBody.BodyType == FarseerPhysics.Dynamics.BodyType.Static);
				}
				return _isStatic;
			}
			set
			{
				_isStatic = value;
				if (RealBody != null)
				{
					if (value)
						RealBody.BodyType = FarseerPhysics.Dynamics.BodyType.Static;
					else RealBody.BodyType = FarseerPhysics.Dynamics.BodyType.Dynamic;
				}
				//RealBody.IsStatic = value;
			}
		}

		private const double _minMass = 1e-3;
		public double Mass
		{
			get
			{
				return RealBody.Mass;
			}
			set
			{
				if (RealBody != null)
					RealBody.Mass = (float)((value < _minMass) ? _minMass : value);
			}
		}

		private bool _floorFricitionEnabled = true;
		public bool FloorFrictionEnabled
		{
			get
			{
				return _floorFricitionEnabled;
			}
			set 
			{
				_floorFricitionEnabled = value;
				if (RealBody != null)
				{
					if (!value)
					{
						RealBody.LinearDamping = 0;
						RealBody.AngularDamping = 0;
					}
					else
					{
						RealBody.LinearDamping = (float)_frictionCoefficient;
						RealBody.AngularDamping = (float)_frictionCoefficient;
					}
				}

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
				if (FloorFrictionEnabled)
				{
					if (RealBody != null)
					{
						RealBody.LinearDamping = (float)_frictionCoefficient;
						RealBody.AngularDamping = (float)_frictionCoefficient;
						RealBody.Friction = (float)_frictionCoefficient; //TODO side friction, floor friction
					}
				}
			}
		}

		private Frame3D _location;
		public Frame3D Location
		{
			get
			{
				if (RealBody != null)
				{
					_location = FarseerConverter.GetFrame(RealBody, _location);
				}
				return _location;
			}

			set
			{
				_location = value;
				if (RealBody != null)
				{
					FarseerConverter.PutInFrame(RealBody, value);
				}
			}
		}

		private Frame3D _velocity;
		public Frame3D Velocity
		{
			get
			{
				return _velocity;
			}
			set
			{
				_velocity = value;
				if (RealBody != null)
				{
					//RealBody.IsStatic = false;
					// Повернём на нужный угол
					RealBody.AngularVelocity = (float)(value.Yaw.Radian);

					// Едем через скорость
					Vector2 v = FarseerConverter.ToSimUnits(FarseerConverter.Frame3DToVector2(value));
					RealBody.LinearVelocity = v;
					//System.Diagnostics.Debug.WriteLine("");
				}
			}
		}

		#endregion

		//--------------------------------------------------------------------------

		#region Joints

		private List<Joint> _joints=new List<Joint>(); //todo убрать это, в фарсире есть jointList

		public IEnumerable<Joint> Joints
		{
			get { return _joints; }
		}

		/// <summary>
		/// Присоединит переданный объект, используя его Location.
		/// </summary>		
		public void JoinWith(IPhysical body, bool joinWithFriction = true)
		{
			JoinWith(body, body.Location, joinWithFriction);
		}

		/// <summary>
		/// Присоединит переданный объект, используя переданный Location.
		/// </summary>		
		public void JoinWith(IPhysical body, Frame3D realLocation, bool joinWithFriction = true)
		{
			if (!joinWithFriction)
			{
				body.FloorFrictionEnabled = false;
			}

			var originalLocation = realLocation;
			var rotated = FarseerConverter.Rotate2D(realLocation, this.Location.Yaw);

			// Передвинем тело к тому, к которому присоединяем. Без этого фарсир может переместить оба тела, будет косяк.
			body.Location = this.Location + new Frame3D(rotated.X, rotated.Y, rotated.Z);

			// Присоединяем с таким offset-ом, как если бы не передвигали, так как фарсиру нужно локальное смещение.
			// Пересчёт в юниты фарсира происходит внутри MakeWeldJoint.
			Joint j = FarseerWorld.MakeWeldJoint(RealBody, (body as FarseerBody).RealBody, originalLocation);
			//PhysicalManager.MakeIteration(1.0001, null);

			//j.CollideConnected = false;
			_joints.Add(j);
			(body as FarseerBody)._joints.Add(j);
		}

		/// <summary>
		/// Отсоединит переданный объект, пересчитает его Location так, чтобы он был абсолютным.
		/// </summary>		
		public void Detach(IPhysical body)
		{
			//FarseerWorld.MakeWeldJoint(this.RealBody, (body as FarseerBody).RealBody);
			// Ищем в списке jointов

			var fb = (FarseerBody) body;
			fb.FloorFrictionEnabled = true;
			foreach (var j in _joints) //ищем нужный joint
			{
				if ((j.BodyA == RealBody && j.BodyB == fb.RealBody) ||
					(j.BodyB == RealBody && j.BodyA == fb.RealBody))
				{
					j.Enabled = false;
					FarseerWorld.RemoveJoint(j);
					_joints.Remove(j); //Удалять joint из списка нужно у обоих тел! М.К.
					fb._joints.Remove(j);
					return;
				}
			}
		}

		#endregion

		//--------------------------------------------------------------------------

		/// <summary>
		/// Настоящее тело движка фарсира.  
		/// </summary>
		public FBody RealBody { get; set; }

		//public FarseerBody()
		//{ }

		public FarseerBody(FBody realBody)
		{
			RealBody = realBody;
			//RealBody.BodyId = -1;//WTF???
			FBodyToFarseerBody.Add(RealBody, this);
			RealBody.OnCollision += (x, y, z) =>
										{
											Body.RaiseCollisionEvent(FBodyToFarseerBody[y.Body].Body);
											return true;
										};
		}
	}
}

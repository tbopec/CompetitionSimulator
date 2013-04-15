using System;
using System.Collections.Generic;

namespace Eurosim.Core
{
	///Ball - это тело, у которого форма шара
	///К Ball по-прежнему можно аттачить тела, как и ко всему
	[Serializable]
	public class Ball : Body
	{
		public override void AcceptVisitor(IBodyVisitor visitor)
		{
			visitor.Visit(this);
		}

		public override string ToString()
		{
			return string.Format("Radius: {0}", _radius);
		}

		public double Radius
		{
			get { return _radius; }
			set
			{
				CheckGreaterOrZero(value);
				SetField(ref _radius, value, RadiusPropertyName);
			}
		}

        public override Dictionary<string, object> getCreateJSON()
        {
            Dictionary<string, object> result = base.getCreateJSON();

            result.Add("radius", _radius);

            return result;
        }
        public override string getName()
        {
            return "Ball";
        }

		public override double Volume { get { return 4.0 / 3.0 * Math.PI * Math.Pow(Radius, 3.0); } }
		public const string RadiusPropertyName = "Radius";
		private double _radius;
	}
}
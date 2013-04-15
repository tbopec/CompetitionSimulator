using System;
using System.Drawing;
using Eurosim.Core.Physics;

namespace Eurosim.Core
{
	[Serializable]
	public abstract class Shape
	{
		public abstract IPhysical GetPhysicalModel();

//		public abstract bool IntersectsWithFloor(); 

		//TODO. Should this be here??
		public abstract Size GetBoundingRect();
	}

	[Serializable]
	public class BoxShape : Shape
	{
		public BoxShape(double xsize, double ysize, double zsize)
		{
			Xsize = xsize;
			Ysize = ysize;
			Zsize = zsize;
		}

		public override IPhysical GetPhysicalModel()
		{
			return PhysicalManager.MakeBox(Xsize, Ysize, Zsize);
		}

		public override Size GetBoundingRect()
		{
			return new Size((int) Xsize, (int) Ysize);
		}

//		public override bool IntersectsWithFloor()
//		{
//			
//		}

		public readonly double Zsize;
		public readonly double Xsize;
		public readonly double Ysize;

		#region Equality members

		public bool Equals(BoxShape other)
		{
			if(ReferenceEquals(null, other)) return false;
			if(ReferenceEquals(this, other)) return true;
			return other.Zsize.Equals(Zsize) && other.Ysize.Equals(Ysize) && other.Xsize.Equals(Xsize);
		}

		public override bool Equals(object obj)
		{
			if(ReferenceEquals(null, obj)) return false;
			if(ReferenceEquals(this, obj)) return true;
			if(obj.GetType() != typeof(BoxShape)) return false;
			return Equals((BoxShape)obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				int result = Zsize.GetHashCode();
				result = (result * 397) ^ Ysize.GetHashCode();
				result = (result * 397) ^ Xsize.GetHashCode();
				return result;
			}
		}

		#endregion
	}

	[Serializable]
	public class BallShape : Shape
	{
		public BallShape(int radius)
		{
			Radius = radius;
		}

		public override IPhysical GetPhysicalModel()
		{
			return PhysicalManager.MakeCyllinder(Radius, Radius, Radius);
		}

		public override Size GetBoundingRect()
		{
			return new Size((int) Radius,(int) Radius);
		}

		public readonly double Radius;

		#region Equality members

		public bool Equals(BallShape other)
		{
			if(ReferenceEquals(null, other)) return false;
			if(ReferenceEquals(this, other)) return true;
			return other.Radius.Equals(Radius);
		}

		public override bool Equals(object obj)
		{
			if(ReferenceEquals(null, obj)) return false;
			if(ReferenceEquals(this, obj)) return true;
			if(obj.GetType() != typeof(BallShape)) return false;
			return Equals((BallShape)obj);
		}

		public override int GetHashCode()
		{
			return Radius.GetHashCode();
		}

		#endregion
	}

	[Serializable]
	public class CyllinderShape : Shape
	{
		public CyllinderShape(double rtop, double rbottom, double height)
		{
			Rtop = rtop;
			Rbottom = rbottom;
			Height = height;
		}

		public override IPhysical GetPhysicalModel()
		{
			return PhysicalManager.MakeCyllinder(Rbottom, Rtop, Height);
		}

		public override Size GetBoundingRect()
		{
			return new Size((int) Rbottom, (int) Rbottom);
		}

		public readonly double Rtop;
		public readonly double Rbottom;
		public readonly double Height;

		#region Equality members

		public bool Equals(CyllinderShape other)
		{
			if(ReferenceEquals(null, other)) return false;
			if(ReferenceEquals(this, other)) return true;
			return other.Rtop.Equals(Rtop) && other.Rbottom.Equals(Rbottom) && other.Height.Equals(Height);
		}

		public override bool Equals(object obj)
		{
			if(ReferenceEquals(null, obj)) return false;
			if(ReferenceEquals(this, obj)) return true;
			if(obj.GetType() != typeof(CyllinderShape)) return false;
			return Equals((CyllinderShape)obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				int result = Rtop.GetHashCode();
				result = (result * 397) ^ Rbottom.GetHashCode();
				result = (result * 397) ^ Height.GetHashCode();
				return result;
			}
		}

		#endregion
	}

	[Serializable]
	public class RectangleShape : Shape
	{
		public RectangleShape(double xsize, double ysize)
		{
			Xsize = xsize;
			Ysize = ysize;
		}

		public override IPhysical GetPhysicalModel()
		{
			return PhysicalManager.MakeBox(Xsize, Ysize, 0);
		}

		public override Size GetBoundingRect()
		{
			return new Size((int) Xsize,(int) Ysize);
		}

		public readonly double Xsize;
		public readonly double Ysize;

		#region Equality members

		public bool Equals(RectangleShape other)
		{
			if(ReferenceEquals(null, other)) return false;
			if(ReferenceEquals(this, other)) return true;
			return other.Xsize.Equals(Xsize) && other.Ysize.Equals(Ysize);
		}

		public override bool Equals(object obj)
		{
			if(ReferenceEquals(null, obj)) return false;
			if(ReferenceEquals(this, obj)) return true;
			if(obj.GetType() != typeof(RectangleShape)) return false;
			return Equals((RectangleShape)obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (Xsize.GetHashCode() * 397) ^ Ysize.GetHashCode();
			}
		}

		#endregion
	}
}
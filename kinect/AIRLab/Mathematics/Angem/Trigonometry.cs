using M = System.Math;

namespace AIRLab.Mathematics
{
	public static partial class Angem
	{
		public static double Sin(Angle angle)
		{
			return M.Sin(angle.Radian);
		}

		public static double Cos(Angle angle)
		{
			return M.Cos(angle.Radian);
		}

		public static double Tg(Angle angle)
		{
			return M.Tan(angle.Radian);
		}

		public static Angle Asin(double value)
		{
			return Angle.FromRad(M.Asin(value));
		}

		public static Angle Acos(double value)
		{
			return Angle.FromRad(M.Acos(value));
		}

		public static Angle Atan(double value)
		{
			return Angle.FromRad(M.Atan(value));
		}

		public static Angle Atan2(double y, double x)
		{
			return Angle.FromRad(M.Atan2(y, x));
		}
	}
}

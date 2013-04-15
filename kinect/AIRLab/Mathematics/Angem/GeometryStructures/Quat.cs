using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIRLab.Mathematics
{
	/// <summary>
	/// Пока нет своих кватернионов, есть такая структура. 
	/// </summary>
	public struct Quat
	{
		public double X, Y, Z, W;
		public Quat(double x, double y, double z, double w)
		{
			X = x;
			Y = y;
			Z = z;
			W = w;
		}
	}
}

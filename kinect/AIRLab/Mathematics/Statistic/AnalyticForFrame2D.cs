using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIRLab.Mathematics.Statistic
{
	public class AnalyticForFrame2D
	{
		public static Frame2D GetMean (IEnumerable<Frame2D> sample)
		{
			var fixSample = sample.ToArray();

			var x = fixSample.Average(element => element.X);
			var y = fixSample.Average(element => element.Y);
			var angle = fixSample.Average(element => element.Angle.Radian);
			return new Frame2D(x,y,Angle.FromRad(angle));
		}
		public static Frame2D GetVariance(IEnumerable<Frame2D> sample)
		{
			var fixSample = sample.ToArray();
			var mean = GetMean(fixSample);

			var x = Math.Pow(fixSample.Sum(element => element.X - mean.X),2)/fixSample.Length;
			var y = Math.Pow(fixSample.Sum(element => element.Y - mean.Y), 2) / fixSample.Length;
			var angle = Math.Pow(fixSample.Sum(element => element.Angle.Radian - mean.Angle.Radian), 2) / fixSample.Length;
			return new Frame2D(x, y, Angle.FromRad(angle));
		}
	}
}

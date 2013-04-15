using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eurosim.Core.Replay
{
	[Serializable]
	public class ScoreAtTime
	{
		public double Time;
		public int[] Scores;

		public ScoreAtTime(double time, int[] scores)
		{
			Time = time;
			Scores = new int[scores.Length];
			scores.CopyTo(Scores, 0);
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eurosim.Core.Replay
{
	public class ScoreSaver
	{
		public ScoreSaver() { }

		public List<ScoreAtTime> ScoresAtTime = new List<ScoreAtTime>(64);

		private double _totalTime;

		public void SaveScores(double dt, int[] scores)
		{
			_totalTime += dt;

			if (ScoresAtTime.Count == 0) 
			{
				ScoresAtTime.Add(new ScoreAtTime(_totalTime, scores)); 
				return;
			}

			int[] lastScores = ScoresAtTime.Last().Scores;

			if (scores.Length != lastScores.Length)
			{
				ScoresAtTime.Add(new ScoreAtTime(_totalTime, scores)); 
				return;
			}

			for (int i = 0; i < scores.Length; i++)
			{
				if (scores[i] != lastScores[i])
				{
					ScoresAtTime.Add(new ScoreAtTime(_totalTime, scores));
					return;
				}
			}
		}

	}
}

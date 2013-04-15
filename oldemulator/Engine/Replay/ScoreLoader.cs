using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eurosim.Core.Replay
{
    public class ScoreLoader
    {
        private ScoreCollection _result;
        public ScoreLoader(List<ScoreAtTime> scores, List<Score> penalties, ScoreCollection resultscores) 
        {
            _scores = scores;
            _penalties = penalties;
            _result = resultscores;
            penalties.Sort((x, y) => x.Time.CompareTo(y.Time));
            //_result = new ScoreCollection(scores[0].Scores.Length);
            for (int i = 0; i < scores[0].Scores.Length; i++)
                _result.TempSum[i] = 0;
        }

        private List<ScoreAtTime> _scores;
        private List<Score> _penalties;
        private double _totalTime = 0;

        //ScoreCollection _scoreCollection;

        public void UpdateScores(double dt)
        {
            _totalTime += dt;
            var sc = _scores.FirstOrDefault();
            if (sc!=null && sc.Time<_totalTime)
            {
                _result.TempSum = sc.Scores;
                _scores.Remove(sc);
            }
            var pen=_penalties.FirstOrDefault();
            if (pen!=null && pen.Time < _totalTime)
            {
                _result.Penalties.Add(pen);
                _penalties.Remove(pen);
            }
            //return _scoreCollection;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eurosim.Core
{
	[Serializable]
    public class Score
    {
        public bool Permanent;
        public double Time;
        public int Value;
        public string Message;
        public int RobotNumber;

        public override string ToString()
        {
            return Permanent
                       ? String.Format("{0} Points: {1} at time = {2}", Value, Message, (int) Time)
                       : String.Format("{0} Points: {1}", Value, Message);
        }
    }

    public class ScoreCollection
    {
        public ScoreCollection(int robotCount)
        {
            TempSum = new int[robotCount];
            ResetAll();
        }

        public int[] TempSum;
        public List<Score> Penalties = new List<Score>();
        public void ResetAll()
        {
            ResetTemp();   
            Penalties.RemoveAll(x=>true);
        }
        public void ResetTemp()
        {
            for (var i = 0; i < TempSum.Length; i++)
            {
                TempSum[i] = 0;
            } 
        }
        public int GetSum(int robotNumber)
        {
            var pensum = Penalties.Where(x => x.RobotNumber == robotNumber).Sum(x => x.Value);
            return pensum + TempSum[robotNumber];
        }
    }
}


using System;
using System.Collections.Generic;

namespace Eurosim.Core.Replay
{
	[Serializable]
	public class SerializationRoot
	{
		public double DT;
		public List<LoggingObject> Objects;
		public List<ScoreAtTime> Scores;
		public List<Score> Penalties;
	}
}
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using AIRLab.Mathematics;
using System.IO;
using Ionic.Zlib;

namespace Eurosim.Core.Replay
{
	public static class ReplayLogger
	{
		private static readonly Dictionary<int, LoggingObject> _logged = 
            new Dictionary<int, LoggingObject>();

		private static readonly Dictionary<PrimitiveBody, int> _existence = 
            new Dictionary<PrimitiveBody, int>();

		private static double _totalTime;

		private static int _updateCount;

		private static readonly SerializationRoot _serializationRoot = new SerializationRoot();

		private static readonly ScoreSaver _scoreSaver = new ScoreSaver();

        public static void SetDT(double dt)
        {
            _serializationRoot.DT = dt;
        }

		public static void LogScore(double dt, int[] scores)
		{
			_scoreSaver.SaveScores(dt, scores);
		}

		/// <summary>
		/// Запомнит все штрафы. 
		/// </summary>		
		public static void LogPenalties(List<Score> penalties)
		{
			_serializationRoot.Penalties = penalties;
		}

		public static void LogObjects(double dt, Body root)
		{
			LogObjects(dt, root, new Frame3D());
		}

		private static void LogObjects(double dt, Body root, Frame3D offset)
		{
			_updateCount++;
			_totalTime += dt;
			LogObjectsRecursively(root, offset, dt);

			var temp = new List<PrimitiveBody>(_existence.Count / 10);

			foreach(var pb in _existence.Select(pair => pair.Key).Where(pb => _existence[pb] < _updateCount))
			{
				pb.World = null;
				_logged[pb.Id].SaveBody(pb, offset, _totalTime); //выставим там false в visibility
				temp.Add(pb);
			}

			foreach (var t in temp)
			{
				_existence.Remove(t);
				//_logged.Remove(t.Id);
			}
		}

		private static void LogObjectsRecursively(Body root, Frame3D offset, double dt)
		{
			if (root is PrimitiveBody)
			{
				var pb = root as PrimitiveBody;
				LoggingObject logged;
				if (!_logged.TryGetValue(pb.Id,out logged))
				{
					_logged.Add(pb.Id, new LoggingObject(pb));
				}
				else
				{
					logged.SaveBody(pb, offset, _totalTime);
				}
				_existence[pb] = _updateCount;
			}
			else
			{
				foreach (var b in root.Nested)
					LogObjectsRecursively(b, offset.Apply(root.Location), dt);
			}
		}

        public static void SerializeToFile(string fileName)
        {
            using (var fs = new StreamWriter(fileName))
            {
                fs.Write(SerializeToString());
            }
        }

        public static string SerializeToString()
        {
            _serializationRoot.Objects = _logged.Values.ToList();
            _serializationRoot.Scores = _scoreSaver.ScoresAtTime;

            var s = new MemoryStream();
            using (var zipstream = new GZipStream(s, CompressionMode.Compress))
            {
                var bf = new BinaryFormatter();
                bf.Serialize(zipstream, _serializationRoot);
            }
            return System.Convert.ToBase64String(s.ToArray());
        }
	}
}

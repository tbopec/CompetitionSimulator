using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Ionic.Zlib;

namespace Eurosim.Core.Replay
{
	public class ReplayPlayer
	{		
		private readonly List<ObjectLoader> _loadingObjects = new List<ObjectLoader>();
        private SerializationRoot _serializationRoot;
	    private ScoreLoader _scoreLoader;

	    public double DT { get; private set; }

		/// <summary>
		/// Наполнит мир примитивными телами, информацию о них возьмёт из переданного файла
		/// </summary>	
        public void Construct(string fileName, ref BodyCollection<Body> world)
		{
		    using (var fs = new StreamReader(fileName, Encoding.UTF8))
		    {
		        var str = fs.ReadToEnd();
		    	var headerLen = "<Feedback>".Length;
		    	str = str.Substring(headerLen, str.Length - headerLen * 2 - 1);
				using (Stream memoryStream = new MemoryStream(System.Convert.FromBase64String(str)),
					zipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
					{
						var bf = new BinaryFormatter();
						_serializationRoot = (SerializationRoot)bf.Deserialize(zipStream);
					}
		    }
		    DT = _serializationRoot.DT;
		    foreach (var l in _serializationRoot.Objects)
		    {
		        var ol = new ObjectLoader(l, world);
		        _loadingObjects.Add(ol);
		        //ol.SetWorld(world);
		    }
		}
        public void ConstructScores(ScoreCollection scores)
        {
            _scoreLoader = new ScoreLoader(_serializationRoot.Scores, _serializationRoot.Penalties, scores);
        }

	    /// <summary>
		/// Обновит мир
		/// </summary>
		public void UpdateBodies(double dt)
		{
	        foreach (var lo in _loadingObjects)
	        {
	            lo.Update(dt);
	        }
		}

        public void UpdateScore(double dt)
        {
            //if (_scoreLoader!=null) 
                _scoreLoader.UpdateScores(dt);
        }

	    
	}
}

using System.Xml;
using System;
using Eurosim.Core.Replay;
using System.IO;
using Eurosim.Core;
using System.Collections.Generic;
using System.Drawing;
using System.Web.Script.Serialization;


namespace ReplayDemo
{
	internal class ReplayDemoEntry
	{
        private static Dictionary<int, Dictionary<string, object>> existing;
        private static List<Dictionary<string, object>> iter;
        private static List<object> result;


		public static int Main(string[] args)
		{
			foreach(String fileName in args)
            {
                Console.WriteLine(fileName);
                var start = DateTime.Now;

                try
                {
                    var input = File.ReadAllText(fileName);
                    var output = new StreamWriter(fileName + ".json");
                    Console.WriteLine("Converting...");
                    
                    output.Write( Convert(input) );

                    output.Close();
                    Console.WriteLine("End {0}", DateTime.Now - start);
                }
                catch (FileNotFoundException Ex)
                {
                    Console.WriteLine("Not found");
                }
			}
            return 0;
        }

        private static string Convert(String input)
        {
			var replayPlayer = new ReplayPlayer(input);
			Body rootBody = replayPlayer.RootBody;
            existing = new Dictionary<int, Dictionary<string, object>>();
            result = new List<object>();
            result.Add(replayPlayer.DT);

			rootBody.ChildAdded += BodyAdded;
			
            while(!replayPlayer.IsAtEnd)
			{
                iter = new List<Dictionary<string, object>>();
                replayPlayer.Update();
                result.Add(iter);
			}

            return (new JavaScriptSerializer()).Serialize(result);
		}
        
		private static void BodyAdded(Body body)
		{
            iter.Add(body.getCreateJSON());
            existing.Add(body.Id, body.getEditJSON());

			body.PropertyChanged += BodyLocationChanged;
		}

        static void BodyLocationChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(Body.LocationPropertyName))
            {
                var body = (sender as Body);
                var pos = body.getEditJSON();
                var old = existing[body.Id];
                foreach (var elem in old)
                    if (elem.Value.Equals(pos[elem.Key]))
                        pos.Remove(elem.Key);
                iter.Add(pos);
            }
        }
	}
}
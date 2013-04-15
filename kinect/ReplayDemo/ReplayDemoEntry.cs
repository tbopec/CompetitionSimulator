using System;
using System.IO;
using System.Threading;
using Eurosim.Core;
using Eurosim.Core.Replay;
using Eurosim.Graphics.DirectX;

namespace ReplayDemo
{
	internal class ReplayDemoEntry
	{
		private static void Main(string[] args)
		{
			if(args.Length < 1)
			{
				Console.WriteLine("No replay file name provided!");
				return;
			}
			string fileName = args[0];
			var replayPlayer = new ReplayPlayer(File.ReadAllText(fileName));
			Body rootBody = replayPlayer.RootBody;
			rootBody.ChildAdded += BodyAdded;
			//CreateGraphics(rootBody);
			while(!replayPlayer.IsAtEnd)
			{
				replayPlayer.Update();
				Thread.Sleep(TimeSpan.FromSeconds(replayPlayer.DT));
			}
			Console.WriteLine("Finished!");
		}

		private static void CreateGraphics(Body rootBody)
		{
			var d = new DirectXScene(rootBody);
			new DirectXFormDrawer(d).Run();
		}

		private static void BodyAdded(Body body)
		{
			Console.WriteLine("Body {0} was added as a child to {1}", body, body.Parent);
			body.PropertyChanged += BodyLocationChanged;
		}

		static void BodyLocationChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName.Equals(Body.LocationPropertyName))
			{
				var eventSourceBody = (sender as Body);
				Console.WriteLine("Body {0} changed its location to {1}", eventSourceBody, eventSourceBody.Location);
			}
		}
	}
}
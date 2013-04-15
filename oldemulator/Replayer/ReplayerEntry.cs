using System;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using AIRLab.Thornado;
using Eurosim.Core;
using Eurosim.Core.Replay;
using Eurosim.Graphics;

namespace EurosimReplayer
{
	public class ReplayerEntry
	{
		static ReplayerEntry()
		{
			GraphicsAssemblyLoader.Register();
		}

		private ReplayerEntry(ReplayerSettings sets)
		{
			_playerthread = new Thread(() =>
				{
					while(true)
					{
						_replayPlayer.UpdateBodies(_replayPlayer.DT);
						_replayPlayer.UpdateScore(_replayPlayer.DT);
						Thread.Sleep(TimeSpan.FromSeconds(_replayPlayer.DT));
					}
				});
			_playerthread.IsBackground = true;
			if(!String.IsNullOrEmpty(_filename))
				Start();
			FormDrawer drawer = new DrawerFactory().CreateOne(sets.VideoMode, _rootBody,
			                                                  new DrawerSettings(), x => new ReplayerForm(x, _scores));
			_form = (ReplayerForm)drawer.Form;
			_form.OpenFileDialog.FileOk += Reset;
			drawer.Run();
		}

		public static void Main(string[] args)
		{
			try
			{
				string settingsFile = args.Length > 1 ? args[0] : "replayer.cfg";
				var settings = new ReplayerSettings();
				try
				{
					settings = IO.INI.ParseFile<ReplayerSettings>(settingsFile, "EurosimReplayer");
				}
				catch
				{
				}
				_app=new ReplayerEntry(settings);
			}
			catch(Exception e)
			{
				MessageBox.Show(e.Message + e.StackTrace);
			}
		}

		private void Start()
		{
			_replayPlayer.Construct(_filename, ref _rootBody);
			_replayPlayer.ConstructScores(_scores);
			//ReplayPlayer._scoreLoader._result = _scores;
			if((_playerthread.ThreadState & ThreadState.Unstarted) == ThreadState.Unstarted)
				_playerthread.Start();
		}

		private void Reset(object sender, CancelEventArgs e)
		{
			_filename = _form.OpenFileDialog.FileName;
			_rootBody.Clear();
			try
			{
				Start();
			}
			catch(Exception ex)
			{
				MessageBox.Show("Invalid replay file\r\n" + ex.Message);
			}
		}

		private BodyCollection<Body> _rootBody = new BodyCollection<Body>();
		private readonly ReplayPlayer _replayPlayer = new ReplayPlayer();

		private string _filename;
		private readonly ReplayerForm _form;
		private readonly Thread _playerthread;
		private readonly ScoreCollection _scores=new ScoreCollection(2);
// ReSharper disable NotAccessedField.Local
		private static ReplayerEntry _app;
// ReSharper restore NotAccessedField.Local
	}
}
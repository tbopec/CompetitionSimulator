using System;
using System.ComponentModel;
using System.IO;
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
			var drawerSettings = new DrawerSettings();
			var drawerControl = new DrawerFactory(_rootBody).
				CreateAndRunDrawerInStandaloneForm(sets.VideoMode,
				drawerSettings, ()=>new ReplayerForm(drawerSettings));
			_form = drawerControl.TopLevelForm as ReplayerForm;
			_form.OpenFileDialog.FileOk += OnFileSelected;
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
				catch (FileNotFoundException) { }
				_app = new ReplayerEntry(settings);
			}
			catch (Exception e)
			{
				MessageBox.Show(e.Message + e.StackTrace);
			}
		}

		private void ReadReplayFile(string filename)
		{
			_replayPlayer=ReplayPlayer.FromFile(filename);
			_rootBody.Add(_replayPlayer.RootBody);
			_form.ScoreDisplayControl.Scores = _replayPlayer.Scores;
			StartThreadIfRequired();
		}

		private void StartThreadIfRequired()
		{
			if (_playerthread == null)
			{
				_playerthread = new Thread(() =>
					{
						while(true)
						{
							_replayPlayer.UpdateBodies();
							_replayPlayer.UpdateScores();
							Thread.Sleep(TimeSpan.FromSeconds(_replayPlayer.DT));
						}
					});
				_playerthread.IsBackground = true;
			}
			if((_playerthread.ThreadState & ThreadState.Unstarted) == ThreadState.Unstarted)
				_playerthread.Start();
		}

		private void OnFileSelected(object sender, CancelEventArgs e)
		{
			var filename = _form.OpenFileDialog.FileName;
			_rootBody.Clear();
			try
			{
				ReadReplayFile(filename);
			}
			catch(Exception ex)
			{
				MessageBox.Show(string.Format("Invalid replay file\r\n {0}", ex.Message));
			}
		}

		private readonly Body _rootBody = new Body();
		private ReplayPlayer _replayPlayer;

	
		private readonly ReplayerForm _form;
		private Thread _playerthread;
// ReSharper disable NotAccessedField.Local
		private static ReplayerEntry _app;
// ReSharper restore NotAccessedField.Local
	}
}
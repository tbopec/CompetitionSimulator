using System.Threading;
using System.Windows.Forms;
using AIRLab.Thornado;
using Eurosim.Core;
using Eurosim.Graphics;
using RoboCoP.Plus;

namespace EurosimStandalone
{
	internal class StandaloneEntry
	{
		public StandaloneEntry(string[] args)
		{
			var commonEnv = new ServiceAppEnvironment("Eurosim", args);
			var emulatorSettings = IO.INI.ParseString<EmulatorSettings>(commonEnv.CfgFileEntry, commonEnv.ServiceName);
			var standaloneSettings = IO.INI.ParseString<EurosimStandaloneSettings>(commonEnv.CfgFileEntry, "EurosimStandalone");
			var emulator = new Emulator(emulatorSettings)
			               	{
			               		App = commonEnv
			               	};
			var emulatorThread = new Thread(() => { while(true) emulator.MakeCycle(true); })
			                     	{
			                     		Name = "emulator",
			                     		IsBackground = true
			                     	};
			emulatorThread.Start();
			if(emulatorSettings.VideoMode == VideoModes.No)
			{
				var thread = new Thread(() =>
					{
						var f = new Form {Text = "Don't worry. Eurosim is working"};
						Application.Run(f);
					});
				thread.SetApartmentState(ApartmentState.STA);
				thread.Start();
			}
			emulator.CreateGraphics();
			emulator.Drawers.ForEach(x => x.Form.EasyInvoke(
				c => c.Controls.Add(new ScoreDisplayControl(emulator.Scores))));
			if(standaloneSettings.UseKeyboard &&
			   emulatorSettings.Robots.Count > 0 &&
			   emulatorSettings.Robots[0] != null &&
			   string.IsNullOrEmpty(emulatorSettings.Robots[0].AI)
			   && emulatorSettings.VideoMode != VideoModes.No)
				_keyboardController = new KeyboardController(emulator, emulator.Drawers[0].Form);
		}

		public static void Main(string[] args)
		{
			_app = new StandaloneEntry(args);
		}

		// ReSharper disable NotAccessedField.Local
		private static StandaloneEntry _app;
		private KeyboardController _keyboardController;
		// ReSharper restore NotAccessedField.Local
	}
}
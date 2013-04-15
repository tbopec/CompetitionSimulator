using System;
using System.Drawing;
using System.Windows.Forms;
using Eurosim.Graphics;

namespace EurosimReplayer
{
	public sealed partial class ReplayerForm
	{
		public ReplayerForm(DrawerSettings settings)
		{
			BackColor = Color.White;
			ClientSize = new Size(SceneConfig.VideoWidth, SceneConfig.VideoHeight);
			TopLevel = true;
			FormBorderStyle = FormBorderStyle.Fixed3D;
			Text = "EurosimReplayer";
			InitializeComponent();
			Height += MainMenuStrip.Height;
			if(settings.ShowControls)
			{
				ScoreDisplayControl = new ScoreDisplayControl();
				Controls.Add(ScoreDisplayControl);
			}
		}

		public ScoreDisplayControl ScoreDisplayControl { get; private set; }

		private void OpenFileClick(object sender, EventArgs e)
		{
			OpenFileDialog.ShowDialog();
		}

		private void ExitClick(object sender, EventArgs e)
		{
			Dispose();
		}
	}
}
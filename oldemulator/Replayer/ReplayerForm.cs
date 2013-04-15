using System;
using System.Drawing;
using System.Windows.Forms;
using Eurosim.Core;
using Eurosim.Graphics;

namespace EurosimReplayer
{
	public partial class ReplayerForm
	{
		public ReplayerForm(DrawerSettings settings, ScoreCollection scores)
		{
			BackColor = Color.White;
			ClientSize = new Size(SceneConfig.VideoWidth, SceneConfig.VideoHeight);
			TopLevel = true;
			FormBorderStyle = FormBorderStyle.Fixed3D;
			Text = "EurosimReplayer";
			InitializeComponent();
			Height += MainMenuStrip.Height;
			if (settings.ShowControls)
				Controls.Add(new ScoreDisplayControl(scores));
		}

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
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Eurosim.Graphics;
using Eurosim.Graphics.DirectX;

namespace GemsHunt.Tutorial
{
	public partial class TutorialForm<TVideoData> : Form
	{
		private const AnchorStyles AnchorAll = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

		public TutorialForm()
		{
			InitializeComponent();
			ClientSize=new Size(800,600);
		}

		protected override void OnLoad(System.EventArgs e)
		{
			base.OnLoad(e);
			for(int i = 0; i < DrawerControls.Count; i++)
				tableLayoutPanel1.Controls.Add(DrawerControls[i], 0, i);
			if(BitmapDisplayer != null)
				tableLayoutPanel1.Controls.Add(BitmapDisplayer, 1, 0);

			if(ScoreDisplayControl != null)
				tableLayoutPanel1.Controls.Add(ScoreDisplayControl, 1,1);


			foreach(Control control in tableLayoutPanel1.Controls)
				control.Anchor = AnchorAll;

		}

		public List<DrawerControl> DrawerControls=new List<DrawerControl>(); 
		public ScoreDisplayControl ScoreDisplayControl=new ScoreDisplayControl();
		public BitmapDisplayer<TVideoData> BitmapDisplayer;

	}
}

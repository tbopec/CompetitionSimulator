using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Eurosim.Core;

namespace Eurosim.Graphics
{
	public sealed partial class ScoreDisplayControl : UserControl
	{
		public ScoreDisplayControl(ScoreCollection scores)
			: this()
		{
			_scores = scores;
			_scoreBoxes.Add(ScoreLabel0);
			_scoreBoxes.Add(ScoreLabel1);
			BackColor = Color.Transparent;
			Anchor = (AnchorStyles.Top | AnchorStyles.Right);
			BringToFront();
			Invalidate(true);
			//TODO. Рак. Очки обновлять по event-у.
			var timer = new Timer{Interval = 50};
			timer.Tick += (o, e) => UpdateDisplayedScores();
		}

		private ScoreDisplayControl()
		{
		
			InitializeComponent();
		}

		public void UpdateDisplayedScores()
		{
			try
			{
				if(InvokeRequired)
					Invoke(new Action(InternalUpdate));
				else
					InternalUpdate();
			}
			catch(ObjectDisposedException)
			{
			}
			catch(InvalidOperationException)
			{
			}
		}

		//TODO. Очки нужно обновлять не каждый кадр, а по event-у!
		private void InternalUpdate()
		{
			for(int i = 0; i < 2; i++)
			{
				int penSum = _scores.Penalties.Where(s => s.RobotNumber == i).Sum(s => s.Value);
				_scoreBoxes[i].Text = (_scores.TempSum[i] + penSum).ToString();
			}
		}

		/// <summary>
		/// Показывает окно с подробным перечислением очков
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void DetailsLinkLabelClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			var form = new Form
			           	{
			           		Size = new Size(400, 200),
			           		AutoSize = false,
			           		MinimizeBox = false,
			           		MaximizeBox = false,
			           		FormBorderStyle = FormBorderStyle.FixedDialog
			           	};
			var outerPanel = new TableLayoutPanel {AutoSize = true, ColumnCount = 2, RowCount = 2};
			form.Controls.Add(outerPanel);
			for(int rNum = 0; rNum < 2; rNum++)
			{
				var label = new Label
				            	{
				            		AutoSize = true,
				            		TabIndex = rNum,
				            		Anchor = (rNum == 0 ? AnchorStyles.Left : AnchorStyles.Right) | AnchorStyles.Top,
				            		Text = "Robot " + rNum + "\r\n" + "Figures: " + _scores.TempSum[rNum] + "\r\n" +
				            		       string.Concat(_scores.Penalties.Where(s => s.RobotNumber == rNum).Select(s => s.ToString() + "\r\n"))
				            	};
				outerPanel.Controls.Add(label, rNum, 0);
				form.AutoScrollMinSize = label.Size;
			}
			form.ShowDialog();
		}

		private readonly ScoreCollection _scores;
		private readonly List<Label> _scoreBoxes = new List<Label>();
	}
}
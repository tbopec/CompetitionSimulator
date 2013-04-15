using System.Drawing;
using System.Windows.Forms;
using Eurosim.Core;

namespace Eurosim.Graphics
{
	public abstract class FormDrawer
	{
		public abstract void Run();

		public float AspectRatio { get { return Form.ClientSize.Width / (float)Form.ClientSize.Height; } }
		public Form Form { get; protected set; }
		public DrawerSettings Settings { get;  set; }

		/// <summary>
		/// Создает окно, которое может отображать очки
		/// </summary>
		/// <param name="settings">Настройки</param>
		/// <param name="scores">Коллекция очкоы</param>
		/// <returns></returns>
		public static Form CreateForm(DrawerSettings settings, ScoreCollection scores)
		{
			var f = CreateDefaultEmptyForm(settings);
			if (settings.ShowControls)
				f.Controls.Add(new ScoreDisplayControl(scores));
			return f;
		}

		public static Form CreateDefaultEmptyForm(DrawerSettings settings)
		{
			var f = new Form
			        	{
			        		BackColor = Color.White,
			        		ClientSize = new Size(SceneConfig.VideoWidth, SceneConfig.VideoHeight),
			        		TopLevel = true,
			        		FormBorderStyle = FormBorderStyle.Fixed3D,
			        		Text = "Eurosim" + settings.RobotNumber,
			        	};
			return f;
		}
	}
}
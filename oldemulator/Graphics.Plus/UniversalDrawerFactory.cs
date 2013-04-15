using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Eurosim.Core;
using Eurosim.Graphics.DirectX;
using Eurosim.Graphics.WPF;
using Eurosim.Graphics.Winforms;

namespace Eurosim.Graphics
{
	public class DrawerFactory : IDrawerFactory
	{
		public FormDrawer CreateOne(VideoModes videoMode, Body root,
		                            DrawerSettings settings, Func<DrawerSettings, Form> formFactory)
		{
			switch(videoMode)
			{
				case VideoModes.DirectX:
					return new DirectXFormDrawer(GetDirectXScene(root), settings, formFactory);
				case VideoModes.WPF:
					return new WPFDrawer(settings, root, formFactory);
				case VideoModes.Winforms:
					return new WinformsDrawer(settings, root, formFactory);
				case VideoModes.No:
					return null;
				default:throw new Exception("not supported vide mode");
			}
		}

		public List<FormDrawer> CreateForSettingsList(VideoModes videoMode, List<DrawerSettings> settings,
		                                              Body root, Func<DrawerSettings, Form> formFactory)
		{
			return new List<FormDrawer>(
				settings.Select(x => CreateOne(videoMode, root, x, formFactory)));
		}

		public DirectXScene GetDirectXScene(Body root)
		{
			return _scene ?? (_scene = new DirectXScene(root));
		}

		private DirectXScene _scene;
	}
}
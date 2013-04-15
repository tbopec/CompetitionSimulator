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
		public DrawerFactory(Body root)
		{
			_root = root;
		}

		public FormDrawer CreateOne(VideoModes videoMode, Body root,
									DrawerSettings settings)
		{
			return CreateOne(videoMode, root, settings, FormDrawer.CreateDefaultEmptyForm);
		}

		public FormDrawer CreateOne(VideoModes videoMode, Body root,
		                            DrawerSettings settings, Func<DrawerSettings, Form> formFactory)
		{
			switch(videoMode)
			{
				case VideoModes.DirectX:
					return new DirectXFormDrawer(GetDirectXScene(), settings, formFactory);
				case VideoModes.WPF:
					return new WPFDrawer(settings, root, formFactory);
				case VideoModes.Winforms:
					return new WinformsDrawer(settings, root, formFactory);
				case VideoModes.No:
					return null;
				default:throw new Exception("Video mode not supported");
			}
		}

		public List<FormDrawer> CreateForSettingsList(VideoModes videoMode, List<DrawerSettings> settings,
		                                              Body root, Func<DrawerSettings, Form> formFactory)
		{
			return new List<FormDrawer>(
				settings.Select(x => CreateOne(videoMode, root, x, formFactory)));
		}

		public DirectXScene GetDirectXScene()
		{
			return _scene ?? (_scene = new DirectXScene(_root));
		}

		private Body _root;
		private DirectXScene _scene;
	}
}
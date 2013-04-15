using System;
using System.Reflection;
using System.Windows.Forms;
using Eurosim.Core;
using Eurosim.Graphics.Winforms;

namespace Eurosim.Graphics
{
	public class DrawerFactory:IDrawerFactory
	{
		public  FormDrawer CreateOne(VideoModes videoMode, 
			Body root, DrawerSettings settings, Func<DrawerSettings,Form> formFactory)
		{
			if (videoMode==VideoModes.Winforms)
				return new WinformsDrawer(settings, root, formFactory);
			throw new Exception(string.Format("VideoMode {0} not supported " +
				"by {1}", videoMode, Assembly.GetExecutingAssembly().FullName));
		}

	}
}

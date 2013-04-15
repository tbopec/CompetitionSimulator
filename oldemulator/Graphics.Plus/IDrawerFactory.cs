using System;
using System.Windows.Forms;
using Eurosim.Core;

namespace Eurosim.Graphics
{
	public interface IDrawerFactory
	{
		FormDrawer CreateOne(VideoModes videoMode, Body root, 
			DrawerSettings drawerSettings, Func<DrawerSettings,Form> formFactory);
	}
}
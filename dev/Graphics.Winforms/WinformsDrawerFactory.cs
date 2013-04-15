using System;
using System.Reflection;
using Eurosim.Core;
using Eurosim.Graphics.Winforms;

namespace Eurosim.Graphics
{
	public class DrawerFactory : DrawerFactoryBase
	{
		public DrawerFactory(Body root)
		{
			_root = root;
		}

		public override FormDrawer CreateDrawer(VideoModes videoModes, DrawerSettings drawerSettings)
		{
			if(videoModes == VideoModes.Winforms)
				return new WinformsDrawer(_root, drawerSettings);
			throw new Exception(string.Format("VideoMode {0} not supported " +
											"by {1}", videoModes, Assembly.GetExecutingAssembly().FullName));
		}

		private readonly Body _root;
	}
}
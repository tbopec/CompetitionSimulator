using System;
using System.IO;
using System.Reflection;

namespace Eurosim.Core
{
	public static class GraphicsAssemblyLoader
	{
		public static void Register()
		{
			AppDomain.CurrentDomain.AssemblyResolve += HandleAssemblyResolve;
		}

		private static Assembly HandleAssemblyResolve(object sender, ResolveEventArgs args)
		{
			if(args.Name.Contains(BaseAssemblyName))
				return Assembly.LoadFile(Path.Combine(
					Environment.CurrentDirectory, WinformsAssemblyName));
			return null;
		}

		private const string WinformsAssemblyName = "Eurosim.Graphics.Winforms.dll";
		private const string BaseAssemblyName = "Eurosim.Graphics";
	}
}
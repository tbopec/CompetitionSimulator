﻿using System;
using System.Windows.Forms;

namespace Eurosim.Graphics
{
	public abstract class FormDrawer : IDisposable
	{
		public abstract void Dispose();
		public abstract void ResizeToFit();
		public abstract void StartDrawing(Control control);
		public abstract void WaitForInitialization();
	}
}
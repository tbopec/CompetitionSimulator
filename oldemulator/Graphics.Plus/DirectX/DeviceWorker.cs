using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using SlimDX;
using SlimDX.Direct3D9;

namespace Eurosim.Graphics.DirectX
{
	internal partial class DeviceWorker
	{
		public DeviceWorker()
		{
			lock (DeviceLock)
				_thread.Enqueue(Initialize);
		}

		public event Action BeforeReset;
		public event Action AfterReset;
		public event Action Disposing;

		public bool ResetIfRequired(IDirectXDrawer drawer, Size updatingSize)
		{
			lock (DeviceLock)
			{
				_requestedSizes[drawer] = updatingSize;
				int maxWidth = _requestedSizes.Max(x => x.Value.Width);
				int maxHeight = _requestedSizes.Max(x => x.Value.Height);
				if (maxWidth != _deviceparams.BackBufferWidth ||
				   maxHeight != _deviceparams.BackBufferHeight)
				{
					ResetInternal(maxWidth, maxHeight);
					return true;
				}
				return false;
			}
		}

		public void ClearDevice()
		{
			Device.Clear(_deviceparams.EnableAutoDepthStencil ? ClearFlags.All : ClearFlags.Target, Color.White, 1.0f, 0);
		}

		public SwapChain GetSwapChainUnsafe(IDirectXDrawer drawer, Form form)
		{
			lock (DeviceLock)
			{
				_requestedSizes[drawer] = form.ClientSize;
				int width = form.ClientSize.Width;
				int height = form.ClientSize.Height;
				PresentParameters paramsCopy = _deviceparams.Clone();
				paramsCopy.DeviceWindowHandle = form.Handle;
				paramsCopy.BackBufferWidth = width;
				paramsCopy.BackBufferHeight = height;
				return new SwapChain(Device, paramsCopy);
			}
		}

		public Surface GetRenderSurfaceUnsafe(IDirectXDrawer drawer, Size size)
		{
			lock (DeviceLock)
			{
				_requestedSizes[drawer] = size;
				Format format = _deviceparams.BackBufferFormat;
				MultisampleType multisampleType = _deviceparams.Multisample;
				int multisampleQuality = _deviceparams.MultisampleQuality;
				return Surface.CreateRenderTarget(Device, size.Width, size.Height,
				                                  format, multisampleType, multisampleQuality, false);
			}
		}

		public void ReleaseDrawer(IDirectXDrawer drawer)
		{
			lock(DeviceLock)
				_requestedSizes.Remove(drawer);
		}

		public bool HandleIfDeviceLost()
		{
			Result result = Device.TestCooperativeLevel();
			if (result == ResultCode.DeviceLost)
			{
				Thread.Sleep(50);
				return true;
			}
			if (result == ResultCode.DeviceNotReset)
			{
				ResetInternal(_deviceparams.BackBufferWidth, _deviceparams.BackBufferHeight);
				return true;
			}
			return false;
		}

		public void TryDispose()
		{
			Console.WriteLine("Drawers left: {0}", _requestedSizes.Count);
			if (_requestedSizes.Count == 0)
				DisposeInternal();
		}

		public Device Device { get; private set; }
		public readonly object DeviceLock = new object();

		internal Size DeviceSize
		{
			get { return new Size(_deviceparams.BackBufferWidth, _deviceparams.BackBufferHeight); }
		}

		private void DisposeInternal()
		{
			if (Disposing != null)
				Disposing();
			lock (DeviceLock)
				_thread.Enqueue(() => Device.Dispose());
			LogInfo("Device worker disposed");
		}

		private void ResetInternal(int width, int height)
		{
			LogInfo("Beginning device reset");
			if (BeforeReset != null)
				BeforeReset();
			_thread.Enqueue(() =>
			                {
			                	_deviceparams.BackBufferHeight = height;
			                	_deviceparams.BackBufferWidth = width;
			                	Device.Reset(_deviceparams);
			                });
			if (AfterReset != null)
				AfterReset();
			LogInfo("Resized device to {0} x {1}", width, height);
		}

		private static void LogInfo(string message, params object[] args)
		{
			Console.WriteLine(message, args);
		}

		private void Initialize()
		{
			_stopwatch.Start();
			var form = new Form();
			_deviceparams = new PresentParameters
			                {
			                	BackBufferHeight = 10,
			                	BackBufferWidth = 10,
			                	DeviceWindowHandle = form.Handle,
			                	EnableAutoDepthStencil = false
			                };
			using (var d3D = new Direct3D())
				CreateDevice(d3D, _deviceparams);
			SwapChain swapChain = Device.GetSwapChain(0);
			Surface buffer = swapChain.GetBackBuffer(0);
			Device.GetRenderTarget(0).Dispose();
			buffer.Dispose();
			swapChain.Dispose();
			_stopwatch.Stop();
			LogInfo("Device initialized in {0} ms", _stopwatch.ElapsedMilliseconds);
		}

		private PresentParameters _deviceparams;

		private readonly Stopwatch _stopwatch = new Stopwatch();
		private readonly ThreadActionQueue _thread = new ThreadActionQueue();
		private readonly Dictionary<IDirectXDrawer, Size> _requestedSizes = new Dictionary<IDirectXDrawer, Size>();

		#region Initializing a software renderer

		private static bool TryRegisterSoftwareRenderer(Direct3D d3D)
		{
			var library = new IntPtr();
			foreach (string suffix in new[] {"", "_1", "_2"})
			{
				string lpFileName = Path.Combine(Environment.SystemDirectory, "rgb9rast" + suffix + ".dll");
				library = LoadLibrary(lpFileName);
				if (library.ToInt32() != 0)
					break;
			}
			if (library.ToInt32() == 0)
				return false;
			var procedure = unchecked((IntPtr) (long) (ulong) GetProcAddress(library, "D3D9GetSWInfo"));
			d3D.RegisterSoftwareDevice(procedure);
			return true;
		}

		[DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
		private static extern IntPtr LoadLibrary(string lpFileName);

		[DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
		private static extern UIntPtr GetProcAddress(IntPtr hModule, string procName);

		#endregion

		#region Creating a device

		private void CreateDevice(Direct3D d3D, PresentParameters deviceparams)
		{
			Device device;
			if (!TryCreateHardwareDevice(d3D, deviceparams, out device) &&
			    !TryCreateSoftwareDevice(d3D, deviceparams, out device))
				CreateReferenceDevice(d3D, deviceparams, out device);
			Device = device;
		}

		private static bool TryCreateHardwareDevice(Direct3D d3D, PresentParameters deviceparams, out Device device)
		{
			return TryCreateDevice(d3D, deviceparams, DeviceType.Hardware, CreateFlags.HardwareVertexProcessing, out device);
		}

		private static bool TryCreateSoftwareDevice(Direct3D d3D, PresentParameters deviceparams, out Device device)
		{
			device = null;
			return TryRegisterSoftwareRenderer(d3D) &&
			       TryCreateDevice(d3D, deviceparams, DeviceType.Software, CreateFlags.SoftwareVertexProcessing, out device);
		}

		private static void CreateReferenceDevice(Direct3D d3D, PresentParameters deviceparams, out Device device)
		{
			if (!TryCreateDevice(d3D, deviceparams, DeviceType.Reference, CreateFlags.SoftwareVertexProcessing, out device))
				throw new Exception("Unable to create any direct3d device");
		}

		private static bool TryCreateDevice(Direct3D d3D, PresentParameters deviceparams, DeviceType deviceType,
		                                    CreateFlags vertexProcessingFlag, out Device device)
		{
			Format adapterformat = d3D.Adapters[0].CurrentDisplayMode.Format;
			//для теней обязательно нужен stencil buffer. Ищем формат
			foreach (Format stencilformat in new[] {Format.D24S8, Format.D24SingleS8, Format.D24X4S4, Format.D15S1})
			{
				if (d3D.CheckDeviceFormat(0, deviceType, adapterformat,
				                          Usage.DepthStencil, ResourceType.Surface, stencilformat) &&
				    d3D.CheckDepthStencilMatch(0, deviceType,
				                               adapterformat, adapterformat, stencilformat))
				{
					deviceparams.EnableAutoDepthStencil = true;
					deviceparams.AutoDepthStencilFormat = stencilformat;
					break;
				}
			}
			//пытаемся создать девайс с Multisampling.
			foreach (var i in new Dictionary<MultisampleType, int>
			                  {
			                  	{MultisampleType.FourSamples, 4},
			                  	{MultisampleType.TwoSamples, 2},
			                  	{MultisampleType.None, 0}
			                  })
			{
				deviceparams.Multisample = i.Key;
				deviceparams.MultisampleQuality = i.Value;
				try
				{
					device = new Device(d3D, 0, deviceType, deviceparams.DeviceWindowHandle,
					                    CreateFlags.Multithreaded | vertexProcessingFlag, deviceparams);
					return true;
				}
				catch (Direct3D9Exception)
				{
				}
			}
			device = null;
			return false;
		}

		#endregion
	}
}
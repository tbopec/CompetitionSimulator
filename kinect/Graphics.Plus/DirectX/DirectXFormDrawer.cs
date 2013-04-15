using System;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using AIRLab.Mathematics;
using Eurosim.Core;
using SlimDX.Direct3D9;

namespace Eurosim.Graphics.DirectX
{
	/// <summary>
	/// Drawer в DirectX - штука, рисующая сцену с определенного ракурса,
	/// с некоторыми настройками.
	/// </summary>
	public partial class DirectXFormDrawer : FormDrawer, IDirectXDrawer
	{
		/// <summary>
		/// Создает новый Drawer c использованием переданных сцены, настроек
		/// и функции-инициализатора для окна, в которое следует отрисовывать
		/// </summary>
		/// <param name="scene">Сцена, которую следует отрисовывать</param>
		/// <param name="settings">Настройки</param>
		/// <param name="formFactory">Функция, возвращаюшая окно (форму) </param>
		public DirectXFormDrawer(DirectXScene scene, DrawerSettings settings,
		                         Func<DrawerSettings, Form> formFactory)
		{
			Settings = settings;
			_scene = scene;
			_effect = scene.Effect;
			_deviceWorker = scene.DeviceWorker;
			_framerate = GetFramerate(scene.DeviceWorker.Device.CreationParameters.DeviceType);
			var t = new Thread(() => SetUpAndWaitForRun(formFactory));
			t.SetApartmentState(ApartmentState.STA);
			t.Start();
			_formInitialized.Wait();
		}

		/// <summary>
		/// Создает  Drawer c использованием данной сцены, дефолтных настроек,
		/// отрисовывающий в дефолтное пустое окно
		/// </summary>
		/// <param name="scene">Сцена, которую следует отрисовывать</param>
		public DirectXFormDrawer(DirectXScene scene)
			:this(scene, new DrawerSettings(),CreateDefaultEmptyForm)
		{
		}

		public override void Run()
		{
			_starter.Set();
		}

		private void SetUpAndWaitForRun(Func<DrawerSettings, Form> formFactory)
		{
			try
			{
				Form = formFactory(Settings);
				Form.Resize += (o, e) =>
					{
						LogInfo("Form resized to {0} x {1}", Form.ClientSize.Width, Form.ClientSize.Height);
						Reset();
						_camera.AspectRatio = AspectRatio;
					};
				_deviceWorker.ResetIfRequired(this, Form.ClientSize);
				AcquireSurfacesUnsafe();
				_deviceWorker.BeforeReset += DisposeSurfaces;
				_deviceWorker.AfterReset += AcquireSurfacesUnsafe;
				
				_camera = new SwitchableCamera(Settings.Robot, new Frame3D(0, 0, 20, Angle.FromGrad(-35), Angle.Zero, Angle.Zero),
				                               Form, Settings.CameraLocation)
				          	{
				          		AspectRatio = AspectRatio,
				          		Mode = ViewModes.Trackball
				          	};
				if(Settings.ShowControls)
					Form.Controls.Add(new CameraSwitchControl(_camera));
				_formInitialized.Set();
				_starter.Wait();
				ShowManyFrames();
				Application.Run(Form);
			}
			finally
			{
				_deviceWorker.BeforeReset -= DisposeSurfaces;
				_deviceWorker.AfterReset -= AcquireSurfacesUnsafe;
				Dispose();
			}
		}

		private static int GetFramerate(DeviceType deviceType)
		{
			return deviceType == DeviceType.Hardware ? SceneConfig.Framerate : 15;
		}

		private void Dispose()
		{
			lock(_deviceWorker.DeviceLock)
			{
				LogInfo("Disposing form drawer");
				DisposeSurfaces();
				_deviceWorker.ReleaseDrawer(this);
				_deviceWorker.TryDispose();
			}
		}

		private void ShowManyFrames()
		{
			var timr = new System.Windows.Forms.Timer
			           	{
			           		Interval = 1000 / _framerate
			           	};
			timr.Tick += (o, args) => ShowOneFrame();
			timr.Start();
		}

		private void ShowOneFrame()
		{
			lock(_deviceWorker.DeviceLock)
			{
				RenderScene(_camera);
				if(!_deviceWorker.HandleIfDeviceLost())
					_swapChain.Present(Present.None);
			}
		}

		private void RenderScene(Camera camera)
		{
			_deviceWorker.Device.SetRenderTarget(0, _buffer);
			_deviceWorker.ClearDevice();
			_effect.ProjectionTransform = camera.ProjectionTransform;
			_effect.ViewTransform = camera.ViewTransform;
			_effect.WorldTransform = camera.WorldTransform;
			_effect.DrawScene(_scene);
		}

		private static void LogInfo(string message, params object[] args)
		{
			Console.WriteLine(message, args);
		}

		private void Reset()
		{
			if(Form.WindowState == FormWindowState.Minimized)
				return;
			if(_deviceWorker.ResetIfRequired(this, Form.ClientSize))
				return;
			LogInfo("Performing local reset");
			DisposeSurfaces();
			AcquireSurfacesUnsafe();
		}

		private void AcquireSurfacesUnsafe()
		{
			LogInfo("Acquiring surfaces");
			_swapChain = _deviceWorker.GetSwapChainUnsafe(this, Form);
			_buffer = _swapChain.GetBackBuffer(0);
		}

		private void DisposeSurfaces()
		{
			LogInfo("Disposing surfaces");
			if(_buffer != null && !_buffer.Disposed)
				_buffer.Dispose();
			if(_swapChain != null && !_swapChain.Disposed)
				_swapChain.Dispose();
		}

		private SwitchableCamera _camera;

		private readonly DeviceWorker _deviceWorker;
		private readonly Effect _effect;
		private readonly DirectXScene _scene;
		private SwapChain _swapChain;
		private Surface _buffer;
		private readonly ManualResetEventSlim _starter = new ManualResetEventSlim();
		private readonly ManualResetEventSlim _formInitialized = new ManualResetEventSlim();
		private readonly int _framerate;
	}
}
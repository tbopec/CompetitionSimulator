using System;
using System.Drawing;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Windows.Media.Media3D;
using System.Windows.Threading;
using Eurosim.Core;

namespace Eurosim.Graphics.WPF
{
	public class WPFDrawer : FormDrawer
	{
		/// <summary>
		/// Создает Drawer с использованием заданных настроек, корневого тела и
		/// фцнкции-инициализатора для формы
		/// </summary>
		/// <param name="settings">Настройки</param>
		/// <param name="root">Корень дерева тел</param>
		/// <param name="formFactory">Функция, инициализируущая форму,
		///  в которую будет отрисовываться изображение </param>
		public WPFDrawer(DrawerSettings settings, Body root, 
			Func<DrawerSettings, Form> formFactory)
		{
			Settings = settings;
			var thread = new Thread(() =>
			{
				Form = formFactory(Settings);
				Initialize();
				InitializeModels(root);
				_initialized.Set();
				_starter.Wait();
				ShowManyFrames();
				System.Windows.Forms.Application.Run(Form);
				Dispose();
			});
			thread.SetApartmentState(ApartmentState.STA);
			thread.Start();
			_initialized.Wait();
		}


		/// <summary>
		/// Создает Drawer c использованием переданного корневого тела,
		/// дефолтными настройками, отрисовывающий в пустую форму
		/// </summary>
		/// <param name="root">Корень дерева тел</param>
		public WPFDrawer(Body root)
			: this(new DrawerSettings(), root,CreateDefaultEmptyForm)
		{
		}

		public override void Run()
		{
			_starter.Set();
		}

		private void Dispose()
		{
			_mainViewport.Children.Clear();
			_bodyWorker.DisposeModels();
		}

		private void InitializeModels(Body root)
		{
			_rootBody = root;
			_mainModelGroup = new Model3DGroup();
			_bodyWorker = new WPFBodyWorker(_mainModelGroup);
			_mainViewport.Children.Add(new ModelVisual3D {Content = _mainModelGroup});
		}

		private void ShowManyFrames()
		{
			var dpTimer = new DispatcherTimer(
				TimeSpan.FromMilliseconds(1000 / (double)_framerate),
				DispatcherPriority.Normal,
				(o, args) => ShowOneFrame(),
				Dispatcher.CurrentDispatcher
				);
			dpTimer.Start();
		}

		private void ShowOneFrame()
		{
			_bodyWorker.UpdateModels(_rootBody);
		}

		private void Initialize()
		{
			_framerate = (SceneConfig.Framerate > 20) ? 20 : SceneConfig.Framerate;
			var mainGrid = new Grid();
			_mainViewport = new Viewport3D();
			mainGrid.Children.Add(_mainViewport);
			Vector3D camDirection = -Settings.CameraLocation.ToWPFVector();
			_mainViewport.Camera = new PerspectiveCamera(Settings.CameraLocation.ToWPFPoint(),
			                                             camDirection,
			                                             new Vector3D(0, 0, 1),
			                                             SceneConfig.ThirdPersonViewAngle.Grad);
			_mainViewport.ClipToBounds = false;
			_mainViewport.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
			_mainViewport.VerticalAlignment = VerticalAlignment.Stretch;
			var light = new Model3DGroup();
			foreach(LightSettings lightSettingse in SceneConfig.Lights)
				light.Children.Add(lightSettingse.ToWPFLight());
			var lightmodel = new ModelVisual3D {Content = light};
			_mainViewport.Children.Add(lightmodel);
			//заклинание. добавляем Viewport в форму.
			var wpfControlHost = new ElementHost
			                     {
			                     	BackColor = Color.White,
			                     	Width = SceneConfig.VideoWidth,
			                     	Height = SceneConfig.VideoHeight,
			                     	Child = mainGrid
			                     };
			Form.Controls.Add(wpfControlHost);
		}

		private Body _rootBody;

		private Viewport3D _mainViewport;
		private WPFBodyWorker _bodyWorker;
		private Model3DGroup _mainModelGroup;
		private int _framerate;
		private readonly ManualResetEventSlim _initialized = new ManualResetEventSlim();
		private readonly ManualResetEventSlim _starter = new ManualResetEventSlim();
	}
}
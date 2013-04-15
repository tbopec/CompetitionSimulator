using System;
using System.Threading;
using System.Windows.Forms;
using AIRLab.Mathematics;
using Eurosim.Core;
using Timer = System.Threading.Timer;

namespace Eurosim.Graphics.Winforms
{
	public class WinformsDrawer:FormDrawer
	{
		/// <summary>
		/// Создает Drawer с использованием заданных настроек, корневого тела и
		/// фцнкции-инициализатора для формы
		/// </summary>
		/// <param name="settings">Настройки</param>
		/// <param name="root">Корень дерева тел</param>
		/// <param name="formFactory">Функция, инициализируущая форму,
		///  в которую будет отрисовываться изображение </param>
		public WinformsDrawer(DrawerSettings settings, Body root, Func<DrawerSettings, Form> formFactory)
		{
			Settings = settings;
			_root = root;
			var thread = new Thread(() => Initialize(formFactory));
			thread.SetApartmentState(ApartmentState.STA);
			thread.Start();
			_initialized.Wait();	
		}

		/// <summary>
		/// Создает Drawer c использованием переданного корневого тела,
		/// дефолтными настройками, отрисовывающий в пустую форму
		/// </summary>
		/// <param name="root">Корень дерева тел</param>
		public WinformsDrawer(Body root)
			:this(new DrawerSettings(), root,
			CreateDefaultEmptyForm)
		{	
		}

		private void Initialize(Func<DrawerSettings, Form> formFactory)
		{
			Form=formFactory(Settings);
			_pictureBox = new PictureBox
			         {
						 Width = Form.ClientSize.Width,
						 Height = Form.ClientSize.Height
			         };
			_pictureBox.Paint += FormPaint;
			Form.Controls.Add(_pictureBox);
			_pictureBox.SendToBack();
			_scene = new WinformsScene(_root);
			Form.Resize += (sender, e) => Resize();
			_initialized.Set();
				
			_starter.Wait();
			Application.Run(Form);
			Dispose();
		}

		private void Resize()
		{
			_pictureBox.Size = Form.ClientSize;
		}

		private float GetScalingFactor()
		{
			var worldVisible =(float)( Settings.CameraLocation.Z*
			                    Angem.Tg(SceneConfig.ThirdPersonViewAngle/2)*2);
			return Math.Min(Form.ClientSize.Width/worldVisible,
				Form.ClientSize.Height/worldVisible);
		}

		private void FormPaint(object sender, PaintEventArgs e)
		{
			_graphics = e.Graphics;
			_scene.Graphics = _graphics;
			_graphics.ResetTransform();
			_graphics.TranslateTransform((float) Form.ClientSize.Width/2,
			                             (float) Form.ClientSize.Height/2);
			var scalingFactor = GetScalingFactor();
			_graphics.ScaleTransform(scalingFactor, scalingFactor);
			_scene.UpdateModels(_root);
		}

		public override void Run()
		{
			_starter.Set();
			_timer = new Timer(Tick, null, 0, 50);
		}

		private void Tick(object state)
		{
			_pictureBox.Invalidate();
		}

		private void Dispose()
		{
			_timer.Dispose();
			_graphics.Dispose();
			_pictureBox.Dispose();
			Form.Dispose();
		}

		private WinformsScene _scene;
		private readonly ManualResetEventSlim _starter=new ManualResetEventSlim();
		private readonly ManualResetEventSlim _initialized=new ManualResetEventSlim();
		private System.Drawing.Graphics _graphics;
		private readonly Body _root;
		private Timer _timer;
		private PictureBox _pictureBox;
	}
}

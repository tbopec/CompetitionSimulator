using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using GemsHunt.Library.Sensors;

namespace GemsHunt.Tutorial
{
	public partial class BitmapDisplayer<TSensorData>:UserControl
	{
		private BitmapDisplayer()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Создает объект BitmapDisplayer для отображения данных с сенсора.
		/// </summary>
		/// <param name="sensor">Сенсор</param>
		/// <param name="updateIntervalMilliseconds">Интервал снятия данных с сенсора, в миллисекундах</param>
		/// <param name="converterFunction">Функция, которая принимает TSensorData и возвращает на основе него Image</param>
		public BitmapDisplayer(ISensor<TSensorData> sensor, int updateIntervalMilliseconds,
								Func<TSensorData, Image> converterFunction)
			: this()
		{
			_sensor = sensor;
			_updateIntervalMilliseconds = updateIntervalMilliseconds;
			_converterFunction = converterFunction;

			_pictureBox = new PictureBox
				{
					ClientSize = this.ClientSize
				};
			_pictureBox.Paint += FormPaint;
			Controls.Add(_pictureBox);
			_timer = new System.Threading.Timer(OnTimerTick, null, 0, _updateIntervalMilliseconds);
		}

		protected override void OnParentChanged(EventArgs e)
		{
			ResizeThisToParent(this,e);
			base.OnParentChanged(e);
			if(Parent != null)
				Parent.Resize += ResizeThisToParent;
		}

		private void ResizeThisToParent(object sender, EventArgs e)
		{
			ClientSize = Parent.ClientSize;
			_pictureBox.ClientSize = ClientSize;
		}

		private void FormPaint(object sender, PaintEventArgs e)
		{
			Graphics graphics = e.Graphics;
			graphics.Clear(Color.White);
			if (_image != null)
				graphics.DrawImage(_image, 0, 0, Width, Height);
		}

		private void OnTimerTick(object sender)
		{
			try
			{
				var result = _sensor.Measure();
				_image = _converterFunction(result);
				_pictureBox.Invalidate();
			}
			catch (Exception e)
			{
				LogError("Sensor.Measure threw an exception", e);
			}

		}
		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
			_pictureBox.Dispose();
			_timer.Dispose();
		}


		private static void LogError(string format, Exception exception)
		{
			Console.WriteLine(format, exception.Message);
		}

		private readonly ISensor<TSensorData> _sensor;
		private readonly int _updateIntervalMilliseconds;
		private readonly Func<TSensorData, Image> _converterFunction;
		private readonly PictureBox _pictureBox;
		private Image _image;

		// ReSharper disable NotAccessedField.Local
		private readonly System.Threading.Timer _timer;
		// ReSharper restore NotAccessedField.Local
	}
}
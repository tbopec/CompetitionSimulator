using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using GemsHunt.Library.Sensors;

namespace GemsHunt.Tutorial
{
	/// <summary>
	/// Класс для отображения данных с сенсора (напр., камеры или Kinect)
	/// </summary>
	/// <typeparam name="TSensorData"></typeparam>
	public class BitmapDisplayer<TSensorData>
	{
		/// <summary>
		/// Создает объект BitmapDisplayer для отображения данных с сенсора.
		/// </summary>
		/// <param name="sensor">Сенсор</param>
		/// <param name="updateIntervalMilliseconds">Интервал снятия данных с сенсора, в миллисекундах</param>
		/// <param name="converterFunction">Функция, которая принимает TSensorData и возвращает на основе него Image</param>
		public BitmapDisplayer(ISensor<TSensorData> sensor, int updateIntervalMilliseconds,
								Func<TSensorData, Image> converterFunction)
		{
			_sensor = sensor;
			_updateIntervalMilliseconds = updateIntervalMilliseconds;
			_converterFunction = converterFunction;
			Form = new Form();
			new Thread(Run)
				{
					IsBackground = true,
				}.Start();
		}

		public Form Form { get; private set; }

		private void Run()
		{
			_pictureBox = new PictureBox {Width = 800, Height = 600};
			_pictureBox.Resize += PictureBoxResize;
			_pictureBox.Paint += FormPaint;
			Form.Controls.Add(_pictureBox);
			_timer=new System.Threading.Timer(OnTimerTick, null, 0, _updateIntervalMilliseconds);
			Application.Run(Form);
		}

		private void PictureBoxResize(object sender, EventArgs eventArgs)
		{
			Form.Size = _pictureBox.Size;
		}

		private void FormPaint(object sender, PaintEventArgs e)
		{
			Graphics graphics = e.Graphics;
			graphics.Clear(Color.White);
			if(_image != null)
				graphics.DrawImage(_image, 0, 0);
		}

		private Image GetImage(ISensor<TSensorData> sensor)
		{
			return _converterFunction(sensor.Measure());
		}

		private void OnTimerTick(object sender)
		{
			_image = GetImage(_sensor);
			_pictureBox.Invalidate();
		}

		private readonly ISensor<TSensorData> _sensor;
		private readonly int _updateIntervalMilliseconds;
		private readonly Func<TSensorData, Image> _converterFunction;
		private PictureBox _pictureBox;
		private Image _image;
		private System.Threading.Timer _timer;
	}
}
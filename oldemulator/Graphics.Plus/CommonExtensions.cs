using System;
using System.Drawing;
using System.Windows.Forms;

namespace Eurosim.Graphics
{
	public static class CommonExtensions
	{
		/// <summary>
		/// Правильно работающий Equals для Color.
		/// Возвращает true если равны значения цветов для каждого из каналов
		/// </summary>
		/// <param name="c1"></param>
		/// <param name="c2"></param>
		/// <returns></returns>
		public static bool RgbEquals(this Color c1, Color c2)
		{
			return c1.A == c2.A && c2.B == c1.B && c2.R == c1.R && c1.G == c2.G;
		}

		/// <summary>
		/// Вызывает Invoke для Control-a только если это необходимо
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="control"></param>
		/// <param name="action"></param>
		public static void EasyInvoke<T>(this T control, Action<T> action)
			where T : Control
		{
			if (control.InvokeRequired)
				control.Invoke(new Action(() => action(control)));
			else
				action(control);
		}
	}
}

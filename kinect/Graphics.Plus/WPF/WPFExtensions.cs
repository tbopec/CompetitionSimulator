using System;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Threading;
using AIRLab.Mathematics;

namespace Eurosim.Graphics.WPF
{
	public static class WPFExtensions
	{
		/// <summary>
		/// Вызывает action с использванием Dispatcher(если необходим)
		/// или без него
		/// </summary>
		/// <param name="element"></param>
		/// <param name="action"></param>
		public static void EasyInvoke(this DispatcherObject element, Action action)
		{
			if(element.Dispatcher.CheckAccess())
				action();
			else
				element.Dispatcher.Invoke(DispatcherPriority.Render, action);
		}

		public static System.Windows.Media.Media3D.Point3D ToWPFPoint(this AIRLab.Mathematics.Point3D point)
		{
			return new System.Windows.Media.Media3D.Point3D(point.X, point.Y, point.Z);
		}

		public static Vector3D ToWPFVector(this AIRLab.Mathematics.Point3D point)
		{
			return new Vector3D(point.X, point.Y, point.Z);
		}

		public static Vector3D ToWPFVector(this Vector vec)
		{
			return new Vector3D(vec[0], vec[1], vec[2]);
		}

		public static System.Windows.Media.Media3D.Point3D ToWPFPoint(this Frame3D frame)
		{
			return new System.Windows.Media.Media3D.Point3D(frame.X, frame.Y, frame.Z);
		}

		public static Vector3D ToWPFVector(this Frame3D frame)
		{
			return new Vector3D(frame.X, frame.Y, frame.Z);
		}

		public static Color ToWPFColor(this System.Drawing.Color color)
		{
			return Color.FromArgb(color.A, color.R, color.G, color.B);
		}

		/// <summary>
		/// Создаем из данных настроек объект Light для WPF
		/// </summary>
		/// <param name="sets">настройки</param>
		/// <returns></returns>
		public static Light ToWPFLight(this LightSettings sets)
		{
			if(sets.Type == LightSettings.MyLightType.Directional)
				return new DirectionalLight(ColorFromString(sets.ColorString), sets.Direction.ToWPFVector());
			if(sets.Type == LightSettings.MyLightType.Spot)
			{
				return new SpotLight(ColorFromString(sets.ColorString), sets.Position.ToWPFPoint(),
				                     sets.Direction.ToWPFVector(), sets.OuterAngle.Grad, sets.InnerAngle.Grad);
			}
			if(sets.Type == LightSettings.MyLightType.Point)
				return new PointLight(ColorFromString(sets.ColorString), sets.Position.ToWPFPoint());
			return new AmbientLight(ColorFromString(sets.ColorString));
		}

		private static Color ColorFromString(string x)
		{
			object convertedColor = ColorConverter.ConvertFromString(x);
			if(convertedColor != null)
				return (Color)convertedColor;
			return Colors.White;
		}
	}
}
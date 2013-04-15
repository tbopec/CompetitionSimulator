﻿using System;
using System.Collections.Generic;
using AIRLab.Mathematics;

namespace Eurosim.Graphics
{
	/// <summary>
	/// Класс содержит редко меняющиеся глобальные настройки сцены
	/// </summary>
	public static class SceneConfig
	{
		public static readonly Angle FirstPersonViewAngle = Angle.HalfPi;
		public static readonly Angle ThirdPersonViewAngle = Angle.FromGrad(45);

		public static readonly List<LightSettings> Lights = new List<LightSettings>
		{
			new LightSettings
			{
               ColorString = "White",
               Direction = new Point3D(-1, 2, -3),
               Type = LightSettings.MyLightType.Directional
			}
		};

		public const int VideoHeight = 600;
		public const int VideoWidth = 800;
		public const int Framerate = 50;
		public const bool EnableShadows = true;
		public const bool EnableSpecularHighlights = true;

		public const float NearClipDistance = 1;
		public const float FarClipDistance = 800;
	}

	/// <summary>
	/// Настройки для источника света. Нужны тк Light в WPF и Light в DirectX-разные объекты
	/// </summary>
	public class LightSettings
	{
		public MyLightType Type;
		public Point3D Position;
		public Point3D Direction;
		public String ColorString;
		/// <summary>
		/// Внутренний угол для Type=Spot
		/// </summary>
		public Angle InnerAngle;
		/// <summary>
		/// Внешний угол для Type=Spot
		/// </summary>
		public Angle OuterAngle;

		public enum MyLightType
		{
			Directional,
			Ambient,
			Point,
			Spot
		}
	}
}
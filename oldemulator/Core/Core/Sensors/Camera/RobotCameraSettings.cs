using System;
using AIRLab.Mathematics;
using AIRLab.Thornado;
using RoboCoP.Common;

namespace Eurosim.Core
{
	[Serializable]
	public class RobotCameraSettings : VideoProviderSettings
	{
		/// <summary>
		/// Угол зрения
		/// </summary>
		[Thornado]
		public Angle ViewAngle;

		/// <summary>
		/// точка крепления камеры
		/// </summary>
		[Thornado]
		public Frame3D Location;

		/// <summary>
		/// Писать в файл? для дебага
		/// </summary>
		[Thornado]
		public bool WriteToFile;

		/// <summary>
		/// Формат изображения (jpg/png итд). Альтернатива VideoProviderSettings.Compress
		/// </summary>
		[Thornado]
		public string Format;
	}
}
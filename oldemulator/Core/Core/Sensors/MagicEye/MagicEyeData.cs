using System;
using System.Collections.Generic;
using AIRLab.Mathematics;
using AIRLab.Thornado;

namespace Eurosim.Core
{
	[Serializable]
	public class MagicEyeObject
	{
		public override string ToString()
		{
			return string.Format("Location: {0}, Name: {1}", Location, Name);
		}

		[Thornado]
		public Frame2D Location;

		[Thornado]
		public string Name;
	}

	public class MagicEyeData
	{
		public MagicEyeData()
		{
			Objects = new List<MagicEyeObject>();
		}

		[Thornado]
		public List<MagicEyeObject> Objects { get; set; }
	}
}
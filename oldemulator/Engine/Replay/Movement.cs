using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AIRLab.Mathematics;

namespace Eurosim.Core.Replay
{
	[Serializable]
	public class Movement : BodyDescription
	{		
		private List<Frame3D> _newLocations;
		private int _counter;

		public Movement(double startTime)
		{
			StartTime = startTime;
			_newLocations = new List<Frame3D>(32);
			_counter = 0;
		}

		public void SaveBody(Frame3D newLocation)
		{
			_newLocations.Add(newLocation);
		}

		public string ToSingleString()
		{
			StringBuilder res = new StringBuilder("Start: " + StartTime + "; ", 512);
			foreach (var l in _newLocations)
				res.Append(l + "; ");
			//else return "Never moved";
			return res.ToString();
		}

		//public void ResetCounter()
		//{
		//    _counter = 0;
		//}

		/// <summary>
		/// Есть ли изменение локации
		/// </summary>
		/// <returns></returns>
		public bool HasNextChange()
		{
			return _counter < _newLocations.Count;
		}

		public void UpdateBody(PrimitiveBody pb, double dt)
		{
			pb.Location = NextLocation();
		}

		public Frame3D NextLocation()
		{
			var res = _newLocations[_counter].NewZ(_newLocations[_counter].Z + 30);
			_counter++;
			return res;
		}
	}
}

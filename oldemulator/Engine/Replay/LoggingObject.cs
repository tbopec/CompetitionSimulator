using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;
using AIRLab.Mathematics;
using System.Drawing;

namespace Eurosim.Core.Replay
{
	[Serializable]
	public class LoggingObject
	{
		/// <summary>
		/// Последнее положение объекта.
		/// </summary>
		private Frame3D _lastLocation;

		/// <summary>
		/// Моменты, в которые движения начинаются
		/// </summary>
		public List<Movement> Movements;
		/// <summary>
		/// Моменты, в которые меняются видимость тела
		/// </summary>
		public List<Visibility> VisibilityStates;
		public Shape Shape { get; private set; }
		public string ModelName { get; private set; }
		public Color Color { get; private set; }
		public Frame3D InitialLocation { get; private set; }

		private bool _isCurrentlyMoving;
		private bool _isCurrentlyVisible;
		private int _id;

		public LoggingObject()
		{
		}

		public LoggingObject(PrimitiveBody pb)
		{
			Movements = new List<Movement>(32);
			VisibilityStates = new List<Visibility>(2);
			Shape = pb.Shape;
			ModelName = pb.ModelFileName;
			_lastLocation = pb.Location;
			InitialLocation = pb.Location;
			_isCurrentlyVisible = false;
			Color = pb.Color;
			_id = pb.Id;
		}

		#region Saving

		private void SaveLocation(Frame3D newLocation, double totalTime)
		{
			if (newLocation.Equals(_lastLocation)) // Положение не изменилось
			{
				_isCurrentlyMoving = false;
				return;
			}

			_lastLocation = newLocation;

			if (_isCurrentlyMoving)
			{
				Movements.Last().SaveBody(newLocation);
			}
			else
			{
				Movements.Add(new Movement(totalTime));
				Movements.Last().SaveBody(newLocation);
				_isCurrentlyMoving = true;
			}
		}

		private void SaveState(PrimitiveBody pb, double totalTime)
		{
			bool isVisible = pb.World != null;
			
			if (isVisible == _isCurrentlyVisible) 
			{				
				return;
			}

			_isCurrentlyVisible = isVisible;

			if (isVisible && VisibilityStates.Count == 0)
				VisibilityStates.Add(new Visibility(false, 0));

			VisibilityStates.Add(new Visibility(isVisible, totalTime));
		}

		public void SaveBody(PrimitiveBody pb, Frame3D offset, double totalTime)
		{
			SaveLocation(offset.Apply(pb.Location), totalTime);
			SaveState(pb, totalTime);
		}

		public string AllMovementsToString()
		{
			StringBuilder res = new StringBuilder(_id.ToString(), 5000);
			foreach (var m in Movements)
				res.Append("\n\t" + m.ToSingleString());

			return res.ToString();
		}

		public string AllVisibilityStatesToString()
		{
			StringBuilder res = new StringBuilder(_id.ToString(), 5000);
			foreach (var v in VisibilityStates)
				res.Append("\n\t" + v.StartTime + "; " + v.IsVisible);

			return res.ToString();
		}

		#endregion
		
	}
}

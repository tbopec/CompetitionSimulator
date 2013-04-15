using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eurosim.Core.Replay
{
	public class ObjectLoader
	{

		#region Constructors

		public ObjectLoader(LoggingObject lo, BodyCollection<Body> world)
		{
		    _world = world;
			_loadedBody = new PrimitiveBody(lo.Shape, lo.Color, lo.ModelName);
			_loadedBody.Location = lo.InitialLocation.NewZ(lo.InitialLocation.Z + 30);
			//_loadedBody.Wo
			Movements = lo.Movements;
			VisibilityStates = lo.VisibilityStates;
		}

		#endregion

		#region Loading

		public List<Movement> Movements;
		public List<Visibility> VisibilityStates;

		private PrimitiveBody _loadedBody;
		private double _totalTime;
		private bool _isMoving = false;
		private int _currentMove = 0;
		private int _currentVisibilityState = 0;
		private BodyCollection<Body> _world;

/*		public void SetWorld(BodyCollection<Body> world)
		{
			_world = world;
			//world.Add(_loadedBody);
		}*/

		public void Update(double dt)
		{
			_totalTime += dt;
			LoadLocation();
			LoadVisibility();			
		}

		public void LoadVisibility()
		{
			if (_currentVisibilityState < VisibilityStates.Count && VisibilityStates[_currentVisibilityState].StartTime < _totalTime)
			{
				if (VisibilityStates[_currentVisibilityState].IsVisible)
					_world.Add(_loadedBody);
				else
					_world.Remove(_loadedBody);

				_currentVisibilityState++;
			}	
			
			//else //если у нас есть движение, чьё время ещё не пришло, но там нет перемещений
			//    if (_currentMove < Movements.Count && !Movements[_currentMove].HasNextChange())
			//        _currentMove++;
		}

		public void LoadLocation()
		{
			if (_isMoving)
			{
				if (Movements[_currentMove].HasNextChange())
				{
					_loadedBody.Location = Movements[_currentMove].NextLocation();
					return;
				}
			    _isMoving = false;
			    _currentMove++;
			}

			if (_currentMove < Movements.Count && Movements[_currentMove].StartTime < _totalTime
				&& Movements[_currentMove].HasNextChange())
			{
				_isMoving = true;
				_loadedBody.Location = Movements[_currentMove].NextLocation();
			}
			else //если у нас есть движение, чьё время ещё не пришло, но там нет перемещений
				if (_currentMove < Movements.Count && !Movements[_currentMove].HasNextChange())
					_currentMove++;
		}

		#endregion
	}
}

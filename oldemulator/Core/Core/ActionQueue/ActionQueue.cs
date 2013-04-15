using System;
using System.Collections.Generic;

namespace Eurosim.Core
{
	public class ActionQueueSelection<T>
	{
		public double DTime { get { return EndTime - StartTime; } }
		public T Action;
		public double StartTime;
		public double EndTime;
	}

	//(MK).Класс нужно использовать под внешним локом. 
	//Подумать, как можно внести лок внутрь?
	public class ActionQueue<T>
		where T : IRobotAction
	{
		public void Enqueue(T action, Action callBack)
		{
			_queue.Enqueue(new ActionQueueElement<T> {Action = action, CallBack = callBack});
		}

		public void Enqueue(T action)
		{
			Enqueue(action,null);
		}

		public IEnumerable<ActionQueueSelection<T>> Dequeue(double dtime)
		{
			while(true)
			{
				if(dtime == 0) yield break;
				if(_queue.Count == 0) yield break;
				ActionQueueElement<T> top = _queue.Peek();
				if(_currentElementElapsed + dtime < top.Action.TotalTime)
				{
					_currentElementElapsed += dtime;
					yield return new ActionQueueSelection<T>
					             	{
					             		Action = top.Action, 
										StartTime = _currentElementElapsed - dtime, 
										EndTime = _currentElementElapsed
					             	};
					yield break;
				}
				yield return new ActionQueueSelection<T>
				             	{
				             		Action = top.Action, 
									StartTime = _currentElementElapsed, 
									EndTime = top.Action.TotalTime
				             	};
				dtime -= top.Action.TotalTime - _currentElementElapsed;
				_currentElementElapsed = 0;
				if (top.CallBack!=null)
					top.CallBack();
				_queue.Dequeue();
			}
		}

		private readonly Queue<ActionQueueElement<T>> _queue = new Queue<ActionQueueElement<T>>();
		private double _currentElementElapsed;
	}
}
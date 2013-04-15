using System;

namespace Eurosim.Core
{
	public class ActionQueueElement<T>
		where T : IRobotAction
	{
		public T Action;
		public Action CallBack;
	}
}
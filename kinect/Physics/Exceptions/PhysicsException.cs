using System;

namespace Eurosim.Physics.Exceptions
{
	public class PhysicsException : Exception
	{
		public PhysicsException()
		{
		}

		public PhysicsException(string message) : base(message)
		{			
		}
	}
}

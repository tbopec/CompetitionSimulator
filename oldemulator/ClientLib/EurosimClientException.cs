using System;
using System.Runtime.Serialization;

namespace Eurosim.ClientLib
{
	[Serializable]
	public class EurosimClientException : Exception
	{
		public EurosimClientException()
		{
		}

		public EurosimClientException(string message)
			: base(message)
		{
		}

		public EurosimClientException(string message, Exception inner)
			: base(message, inner)
		{
		}

		protected EurosimClientException(
			SerializationInfo info,
			StreamingContext context)
			: base(info, context)
		{
		}
	}
}
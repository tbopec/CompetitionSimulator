using System.Drawing;
using Eurosim.Core;
using NUnit.Framework;

namespace Eurosim.Graphics.DirectX
{
	public sealed partial class DirectXScene
	{
		[TestFixture]
		public class BodyComparerTests
		{
			[Test]
			public void Equal()
			{
				var x1 = new PrimitiveBody(new BallShape(3), Color.Yellow, "queen");
				var x2 = new PrimitiveBody(new BallShape(3), Color.Yellow, "queen",
				                           "somethingElse");
				var x3 = new PrimitiveBody(new BallShape(3),
				                           Color.FromArgb(255, 255, 255, 0), "queen");
				Assert.That(_comparer.Equals(x1,x2));
				Assert.That(_comparer.Equals(x2,x3));
			}
			[Test]
			public void NotEqual()
			{
				var x1 = new PrimitiveBody(new BallShape(3), Color.Yellow, "queen");
				var x2 = new PrimitiveBody(new BallShape(4), Color.Green, "queen");
				var x3= new PrimitiveBody(new BallShape(3), Color.Yellow, "king");
				Assert.That(!_comparer.Equals(x1,x2));
				Assert.That(!_comparer.Equals(x2,x3));
				Assert.That(!_comparer.Equals(x1,x3));
				
			}

			private readonly BodyComparer _comparer = new BodyComparer();
		}
	}
}

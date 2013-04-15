using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Eurosim.Core
{
	class BasicBodyTests
	{
		[Test]
		public void Subtree()
		{
			var pb1 = new PrimitiveBody("xxx");
			var pb2 = new PrimitiveBody("yyy");
			var pb3 = new PrimitiveBody("zzz");
			var collection2 = new BodyCollection<PrimitiveBody> {pb2, pb3};
			var root = new BodyCollection<Body> {pb1, collection2};
			var actual = root.GetSubtreeChildrenFirst().ToList();
			Assert.AreEqual(5,actual.Count);
			CheckResults(actual);
			Assert.That(new List<Body>{pb1}, 
				Is.EquivalentTo(pb1.GetSubtreeChildrenFirst().ToList()));

			actual = collection2.GetSubtreeChildrenFirst().ToList();
			Assert.AreEqual(3,actual.Count);
			CheckResults(actual);

			var c1 = new BodyCollection<BodyCollection<BodyCollection<Body>>>();
			var c2 = new BodyCollection<BodyCollection<Body>>();
			var c3 = new BodyCollection<Body>();
			c1.Add(c2);
			c2.Add(c3);
			actual = c1.GetSubtreeChildrenFirst().ToList();
			Assert.AreEqual(3,actual.Count);
			CheckResults(actual);
		}

		private static void CheckResults(IList<Body> actual)
		{
			foreach (var body in actual.Where(x=>x.Parent!=null))
			{
				var index = actual.IndexOf(body);
				var parentIndex = actual.IndexOf(body.Parent);
				if (parentIndex>0)
					Assert.Less(index,parentIndex);
			}
		}
	}
}

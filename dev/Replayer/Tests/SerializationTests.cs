using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using AIRLab.Mathematics;
using Eurosim.Graphics.DirectX;
using Eurosim.Graphics.Properties;
using NUnit.Framework;

namespace Eurosim.Core
{
	public class SerializationTests
	{
		[Test, TestCaseSource("BodySerializationTests")]
		public void SimpleBodyWithModel(Body body)
		{
			TestSerializationAndCheck(body);
		}

		public IEnumerable<TestCaseData> BodySerializationTests
		{
			get
			{
				return new[]
				       	{
				       		new Body
				       			{
				       				Model = Model.FromResource(() => Resources.queenModel),
				       				DefaultColor = Color.Yellow,
				       				Location = new Frame3D(1, 2, 3)
				       			},
				       		new Body(),
				       		new Box
				       			{
				       				XSize = 10,
				       				YSize = 20,
				       				ZSize = 30,
				       				DefaultColor = Color.Green,
				       				Back = new PlaneImageBrush {FilePath = "testfile"}
				       			},
				       		new Ball
				       			{
				       				Radius = 5,
				       				DefaultColor = Color.Red
				       			},
				       		new Cylinder
				       			{
				       				RTop = 10,
				       				RBottom = 20,
				       				Height = 3,
				       				Top = new PlaneImageBrush {FilePath = "fakefile"}
				       			}
				       	}.Select(x => new TestCaseData(x));
			}
		}

		private static void TestSerializationAndCheck(Body expected)
		{
			var bf = new BinaryFormatter();
			Stream ms = new MemoryStream();
			bf.Serialize(ms, expected);
			ms.Seek(0, SeekOrigin.Begin);
			var res = (Body)bf.Deserialize(ms);
			Assert.That(GraphicsComparer.Equals(expected, res));
			Assert.AreEqual(expected.Name, res.Name);
			Assert.AreEqual(res.Id, expected.Id);
			Assert.Null(res.Parent);
			Assert.IsEmpty(res.Nested);
		}

		private static readonly BodyComparer GraphicsComparer = new BodyComparer();
	}
}
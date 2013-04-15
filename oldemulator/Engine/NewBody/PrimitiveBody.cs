using System.Collections.Generic;
using System.Drawing;

namespace Eurosim.Core
{
	public class PrimitiveBody : BodyCollection<Body>
	{
		protected PrimitiveBody()
		{
			IsCollection = true;
			IsPrimaryBody = false;
		}
		//TODO(tothink).Имеют ли право на существование тела без шейпа?
		public PrimitiveBody(string modelName) : this()
		{
			ModelFileName = modelName;
		}

		public PrimitiveBody(Shape shape, string modelName)
			: this()
		{
			Shape = shape;
			ModelFileName = modelName;
		}

		public PrimitiveBody(Shape shape, Color color) : this()
		{
			Color = color;
			Shape = shape;
		}

		public PrimitiveBody(Shape shape, Color color, string modelname) :
			this(shape, color)
		{
			ModelFileName = modelname;
		}

		public PrimitiveBody(Shape shape, Color color,
		                     string modelname, string topviewFileName)
			: this(shape, color, modelname)
		{
			TopViewFileName = topviewFileName;
		}

//		public override IEnumerable<Body> Nested
//		{
//			get { yield break; }
//		}

		public Shape Shape { get; protected set; }
		public Color Color { get; protected set; }
		
		//TODO.Это имена ресурсов, не файлов. переименовать или сделать настояшую загрузку из фвйлов в рантайме
		public string ModelFileName { get; protected set; }
		public string TopViewFileName { get; protected set; }
	}
}
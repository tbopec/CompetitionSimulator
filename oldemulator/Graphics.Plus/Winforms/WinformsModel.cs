using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using Eurosim.Core;

namespace Eurosim.Graphics.Winforms
{
	public class WinformsModel : IDisposable
	{
		private WinformsModel()
		{
		}

		public static WinformsModel FromShape(PrimitiveBody body)
		{
			return new WinformsModel
			       	{
			       		Brush = new SolidBrush(body.Color),
			       		Body = body
			       	};
		}

		public static WinformsModel FromImage(PrimitiveBody body)
		{
			string path = body.TopViewFileName;
			using(Stream stream = GetResourceStream(path))
			{
				var m = new WinformsModel
				        	{
				        		Body = body,
				        		HasImage = true,
				        		Image = Image.FromStream(stream)
				        	};
				var newSizeBitmap = new Bitmap(m.Image,
				                               m.Body.Shape.GetBoundingRect());
				m.Image.Dispose();
				m.Image = newSizeBitmap;
				return m;
			}
		}

		public void Draw(System.Drawing.Graphics graphics)
		{
			if(HasImage)
				DrawUsingImage(graphics);
			else
				DrawUsingShape(graphics);
		}

		public void Dispose()
		{
			if(Image != null)
				Image.Dispose();
			if(Brush != null)
				Brush.Dispose();
		}

		//TODO. ѕодумать, как обращатьс€ с пут€ми до ресурсов.
		protected static Stream GetResourceStream(string filePath)
		{
			Assembly asm = Assembly.GetExecutingAssembly();
			Stream stream =
				asm.GetManifestResourceStream("Eurosim.Graphics"+".Models."+filePath);
			return stream;
		}

		private void DrawUsingImage(System.Drawing.Graphics graphics)
		{
			Size size = Body.Shape.GetBoundingRect();
			graphics.DrawImage(Image, -size.Width / 2, -size.Height / 2);
		}

		private void DrawUsingShape(System.Drawing.Graphics graphics)
		{
			if(Body.Shape == null)
				return;
			Size size = Body.Shape.GetBoundingRect();
			if(Body.Shape is BoxShape)
			{
				graphics.FillRectangle(Brush,
				                       - (float)size.Width / 2,
				                       - (float)size.Height / 2,
				                       size.Width, size.Height);
			}
			else if(Body.Shape is BallShape || Body.Shape is CyllinderShape)
			{
				float rad = size.Width;
				graphics.FillEllipse(Brush, - rad, - rad, rad * 2, rad * 2);
			}
		}

		private bool HasImage { get; set; }
		private Brush Brush { get; set; }
		private Image Image { get; set; }
		private PrimitiveBody Body { get; set; }
	}
}
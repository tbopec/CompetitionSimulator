using System.Drawing;
using AIRLab.Drawing;
using System;
using AIRLab.Mathematics;

namespace RoboCoP.Plus.Common {
    public class VideoDataDrawer {

        static StringFormat format = new StringFormat();

        static VideoDataDrawer() {
            format.Alignment = StringAlignment.Center;
            format.LineAlignment = StringAlignment.Center;
        }


        static FastBitmap original;
        static ClassifiedBitmap classified;
        static VectorVideoData vvd;
        static VideoDataDrawerSettings settings;
        static Font Font = new Font("Arial", 8);

        static Color GetClassColor(int c) {
            if(settings.ClassColors != null && settings.ClassColors.Length > c)
                return settings.ClassColors[c];
            return Color.White;
        }

        static FastBitmap MakeMask(FastBitmap bitmap, Func<Color, Color> mask) {
            var res = new FastBitmap(bitmap.Width, bitmap.Height);
            for(int x = 0; x < bitmap.Width; x++)
                for(int y = 0; y < bitmap.Height; y++)
                    res[x, y] = mask(bitmap[x, y]);
            return res;
        }

        static FastBitmap MakeRecognitionMask(FastBitmap original, ClassifiedBitmap classified, double percent) {
            FastBitmap result = null;

            if(classified == null)
                return original;

            if(original != null)
                result = new FastBitmap(original.Width, original.Height);
            else
                result = new FastBitmap(classified.OriginalWidth, classified.OriginalHeight);

            for(int x = 0; x < classified.OriginalWidth; x += classified.CellSize)
                for(int y = 0; y < classified.OriginalHeight; y += classified.CellSize) {
                    try {

                        var cl = classified.GetByOriginal(x, y);
                        Color col = GetClassColor(cl);

                        for(int xx = 0; xx < classified.CellSize; ++xx)
                            for(int yy = 0; yy < classified.CellSize; ++yy)
                                if(xx + x < classified.OriginalWidth && yy + y < classified.OriginalHeight)

                                    if(original != null)
                                        result.SetPixel(x + xx, y + yy, ImageTransform.ConvexCombination(col, original[x, y], percent));
                                    else
                                        result.SetPixel(x + xx, y + yy, col);

                    } catch {
                    }
                }
            return result;
        }


        static Bitmap MakeGround() {
            FastBitmap result = null;

            if(settings.Raster.MaskType == MaskType.RecognizedImage)
                return MakeRecognitionMask(original, classified, settings.Raster.RecognizedMaskPercent).ToBitmap();

            if(original != null) {
                switch(settings.Raster.MaskType) {
                    case MaskType.No:
                    result = original;
                    break;
                    case MaskType.Hue:
                    result = MakeMask(original, c => HSBColor.Create(c.GetHue(), 0.5, 0.5));
                    break;
                    case MaskType.Lightness:
                    result = MakeMask(original, delegate(Color c) {
                        var m = (int)(c.GetBrightness() * 255);
                        return Color.FromArgb(m, m, m);
                    });
                    break;
                    case MaskType.Saturation:
                    result = MakeMask(original, delegate(Color c) {
                        var m = (int)(c.GetSaturation() * 255);
                        return Color.FromArgb(m, m, m);
                    });
                    break;
                    case MaskType.R:
                    result = MakeMask(original, c => Color.FromArgb(c.R, c.R, c.R));
                    break;
                    case MaskType.G:
                    result = MakeMask(original, c => Color.FromArgb(c.G, c.G, c.G));
                    break;
                    case MaskType.B:
                    result = MakeMask(original, c => Color.FromArgb(c.B, c.B, c.B));
                    break;
                    case MaskType.RecognizedImage:

                    break;
                }
                return result.ToBitmap();
            }

            return new Bitmap(640, 480);
        }


        static void DrawCenteredString(Graphics g, string str, Font font, Brush brush, int x, int y) {
            try {
                g.DrawString(str, font, brush, new Rectangle(x - 500, y - 500, 1000, 1000), format);
            } catch {
            }
        }

        static void DrawGraph(Graphics g) {
            if(vvd == null) return;
            var brush = new SolidBrush(settings.Graph.VertexColor);
            var d = 2 * settings.Graph.VertexSize;
            foreach(var p in vvd.Vertices) {
                g.FillEllipse(brush,
                    p.Picture.ToPointF().X - settings.Graph.VertexSize,
                    p.Picture.ToPointF().Y - settings.Graph.VertexSize,
                    d, d);

                string str = "";
                if(settings.Graph.DrawVertexPicture)
                    str += MathIO.Point2D.ExtendedFormat.WithPrecision(settings.Precision).Write(p.Picture) + "\n";
                if(settings.Graph.DrawVertexAllPicture)
                    str += MathIO.Point2D.ExtendedFormat.InArray.Nullable.WithPrecision(settings.Precision).Write(p.Pictures) + "\n";
                if(settings.Graph.DrawVertexReal)
                    str += MathIO.Point3D.ExtendedFormat.WithPrecision(settings.Precision).Write(p.Real) + "\n";
                DrawCenteredString(g, str, Font, Brushes.Black, (int)p.Picture.X, (int)p.Picture.Y);
            }


            var pen = new Pen(settings.Graph.EdgeColor, (float)settings.Graph.EdgeWidth);
            foreach(var e in vvd.Edges)
                if(e.Point1 != null && e.Point2 != null)
                    g.DrawLine(pen,
                        e.Point1.Picture.ToPointF(),
                        e.Point2.Picture.ToPointF());
        }

        static void DrawBodies(Graphics g) {
            if(vvd == null) return;




            foreach(var b in vvd.Bodies) {
                if(settings.Bodies.DrawBoundingBox)
                    g.DrawRectangle(
                        new Pen(GetClassColor(b.Class)),
                        b.PictureBoundingBox.X,
                        b.PictureBoundingBox.Y,
                        b.PictureBoundingBox.Width,
                        b.PictureBoundingBox.Height);


                if(settings.Bodies.DrawYawArrow)
                    g.DrawLine(Pens.Black,
                        (float)b.Picture.X,
                        (float)b.Picture.Y,
                        (float)b.Picture.X + (float)(20 * Math.Cos(b.Real.Yaw.Radian)),
                        (float)b.Picture.Y - (float)(20 * Math.Sin(b.Real.Yaw.Radian)));
                string s = "";
                if(settings.Bodies.DrawPictureLocation)
                    s += MathIO.Frame2D.ExtendedFormat.WithPrecision(3).Write(b.Picture) + "\n";
                if(settings.Bodies.DrawRealLocation)
                    s += MathIO.Frame3D.ExtendedFormat.WithPrecision(3).Write(b.Real) + "\n";


                DrawCenteredString(g, s, Font, Brushes.Black, (int)b.Picture.X, (int)b.Picture.Y);
            }
        }

        public static Bitmap Draw(FastBitmap _original, ClassifiedBitmap _classified, VectorVideoData _vvd, VideoDataDrawerSettings _settings) {
            original = _original;
            classified = _classified;
            vvd = _vvd;
            settings = _settings;
            var Base = MakeGround();
            Graphics g = Graphics.FromImage(Base);
            DrawGraph(g);
            DrawBodies(g);
            if(vvd != null && settings.DrawAdditionalData)
                vvd.Addition2D.Draw(g);
            g.Dispose();
            return Base;
        }

    }
}
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace AIRLab.Drawing
{
    /// <summary>
    /// Class for processing FastBitmap
    /// </summary>
    public class ImageTransform
    {
        /// <summary>
        /// Rotates FastBitmap 90 degree clockwise
        /// </summary>
        public static FastBitmap Rotate90(FastBitmap bmp)
        {
            FastBitmap bmp1 = new FastBitmap(bmp.Height, bmp.Width);
            for (int x = 0; x < bmp.Width; x++)
                for (int y = 0; y < bmp.Height; y++)
                    bmp1.SetPixel(y, x, bmp.GetPixel(x, y));
            return bmp1;
        }

        /// <summary>
        /// Mirrors FastBitmap over X axis
        /// </summary>
        public static void MirrorX(FastBitmap bmp)
        {
            for (int x = bmp.Width / 2; x >= 0; x--)
            {
                int x1 = bmp.Width - x-1;
                for (int y = 0; y < bmp.Height; y++)
                {
                    Color c = bmp.GetPixel(x, y);
                    bmp.SetPixel(x, y, bmp.GetPixel(x1, y));
                    bmp.SetPixel(x1, y, c);
                }
            }
        }

        /// <summary>
        /// Mirrors FastBitmap over Y axis
        /// </summary>
        public static void MirrorY(FastBitmap bmp)
        {
            for (int y = bmp.Height / 2; y >= 0; y--)
            {
                int y1 = bmp.Height - y-1;
                for (int x = 0; x < bmp.Width; x++)
                {
                    Color c = bmp.GetPixel(x, y);
                    bmp.SetPixel(x, y, bmp.GetPixel(x, y1));
                    bmp.SetPixel(x, y1, c);
                }
            }
        }

        /// <summary>
        /// Crops rectangular subimage from image
        /// </summary>
        public static FastBitmap Crop(FastBitmap bmp, Rectangle rect)
        {
            FastBitmap bmp1 = new FastBitmap(rect.Width, rect.Height);
            
            for (int x = 0; x < rect.Width; x++)
                for (int y = 0; y < rect.Height; y++)
                    bmp1.SetPixel(x,y,bmp.GetPixel(x+rect.Left,y+rect.Top));

            return bmp1;
        }

        public static Color ConvexCombination(Color c1, Color c2, double k)
        {
            double l = 1 - k;
            return Color.FromArgb(
                (int)(c1.R * k + c2.R * l),
                (int)(c1.G * k + c2.G * l),
                (int)(c1.B * k + c2.B * l));
        }

        public static FastBitmap ConvexCombination(FastBitmap original, FastBitmap mask, double k)
        {
            FastBitmap n=new FastBitmap(original.Width,original.Height);
            for (int x = 0; x < original.Width; x++)
                for (int y = 0; y < original.Height; y++)
                    n[x, y] = ConvexCombination(original[x, y], mask[x, y], k);
            return n;
        }

        #region Converting HSB to Color


        static double InRange(double c)
        {
            while (c < 0) c += 1;
            while (c > 1) c -= 1;
            return c;
        }

        static double SX = 1.0 / 6.0;

        static void Convert(ref double c, double p, double q)
        {

            if (c < SX) c = p + ((q - p) * 6 * c);
            else if (SX <= c && c < 3 * SX) c = q;
            else if (3 * SX <= c && c < 4 * SX) c = p + ((q - p) * 6 * (4 * SX - c));
            else c = p;
        }


        /// <summary>
        /// Converts HSB to Color
        /// </summary>
        /// <param name="h">Hue from 0 to 360</param>
        /// <param name="s">Saturation from 0 tî 1</param>
        /// <param name="l">Brightness from 0 tî 1</param>
        /// <returns></returns>
        public static Color ColorFromHSB(double h, double s, double l)
        {
            double q = 0;
            if (l < 0.5)
                q = l * (l + s);
            else
                q = l + s - (l * s);
            double p = 2 * l - q;
            double hk = h / 360;
            double tr = InRange(hk + 2 * SX);
            double tg = InRange(hk);
            double tb = InRange(hk - 2 * SX);

            Convert(ref tr, p, q);
            Convert(ref tg, p, q);
            Convert(ref tb, p, q);

            return Color.FromArgb
                (
                (int)(tr * 255),
                (int)(tg * 255),
                (int)(tb * 255)
                );
        }

        /// <summary>
        /// Gets some count of colors from color ring (with given saturation and brightness)
        /// </summary>
        /// <param name="count"></param>
        /// <param name="saturation"></param>
        /// <param name="brightness"></param>
        /// <returns></returns>
        public static Color[] CreateColors(int count, double saturation, double brightness)
        {
            Color[] cols = new Color[count];
            double delta = 360.0 / count;
            double start = 0;
            for (int i = 0; i < count; i++)
            {
                cols[i] = ColorFromHSB(start, saturation, brightness);
                start += delta;
            }
            return cols;


        }




        #endregion
    }
}
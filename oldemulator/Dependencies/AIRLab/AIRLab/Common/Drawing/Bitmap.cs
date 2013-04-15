using System.Drawing;
namespace AIRLab.Drawing
{
    public static class BitmapExtensions
    {
        public static Bitmap Resize(this Image original, int newWidth, int newHeight)
        {
            var bmp = new Bitmap(newWidth, newHeight);
            using (var g = Graphics.FromImage(bmp))
            {
                g.DrawImage(original, 0, 0, newWidth, newHeight);
            }
            return bmp;
        }

        public static Bitmap Resize(this Image original, float scale)
        {
            return Resize(original, (int)(scale * original.Width), (int)(scale * original.Height));
        }
        public static Bitmap Resize(this Image original, bool byWidth, int newSize)
        {
            if (byWidth) return Resize(original, ((float)newSize) / original.Width);
            else return Resize(original, ((float)newSize) / original.Height);
        }
    }
}
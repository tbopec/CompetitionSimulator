//Authors: Y. Okulovsky
//Small changes: D. Kononchuk

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Collections.Generic;

namespace AIRLab.Drawing
{
    


    /// <summary>
    /// This class is a simple representation of bitmap with a byte array of colors. Working with this class is faster, than with Bitmap
    /// </summary>
    [Serializable]
    public class FastBitmap : IEnumerable<Color>
    {
        #region Data

        /// <summary>
        /// Byte array for colors in bitmap
        /// </summary>
        private readonly byte[] data;

        /// <summary>
        /// height of bitmap
        /// </summary>
        private readonly int height;

        /// <summary>
        /// A size of one bitmap's row. It is the less number, which dividable to 4, and greater or equal width*3
        /// </summary>
        private readonly int rowSize;

        /// <summary>
        /// Width of bitmap
        /// </summary>
        private readonly int width;

        /// <summary>
        /// Builds new empty <see cref="FastBitmap"/> which size is <paramref name="width"/> x <paramref name="height"/>
        /// </summary>
        public FastBitmap(int width, int height)
        {
            this.width = width;
            this.height = height;
            rowSize = width * 3;
            if(rowSize % 4 != 0)
                rowSize += 4 - rowSize % 4;
            data = new byte[this.height * rowSize];
        }

        /// <summary>
        /// Height of bitmap
        /// </summary>
        public int Height
        {
            get { return height; }
        }

        /// <summary>
        /// Width of bitmap
        /// </summary>
        public int Width
        {
            get { return width; }
        }

        /// <summary>
        /// A size of one bitmap's row. It is the less number, which dividable to 4, and greater or equal width*3
        /// </summary>
        public int RowSize
        {
            get { return rowSize; }
        }

        /// <summary>
        /// Colors in bitmap.
        /// The actual count of bytes for one row is equal to <see cref="RowSize"/>
        /// The order of rows in this array is REVERSED, i.e. first row is encoded last, as in BMP-file.
        /// </summary>
        public byte[] Data
        {
            get { return data; }
        }

        #endregion

        #region GetPixel / Set pixel

        /// <summary>
        /// Gets or sets specified pixel in bitmap
        /// </summary>
        public Color this[int x, int y]
        {
            get
            {
                return GetPixel(x, y);
            }
            set
            {
                SetPixel(x, y, value);
            }
        }


        private void Check(int x, int y)
        {
            if(x < 0 || x >= Width || y < 0 || y >= Height)
                throw new IndexOutOfRangeException("Wrong (x,y) coordinates when work with FastBitmap class");
        }

        /// <summary>
        /// Gets an offset if byte array, from which pixel (x,y) is encoded.
        /// </summary>
        public int GetOffset(int x, int y)
        {
            Check(x, y);
            return (Height - y - 1) * RowSize + x * 3;
        }

        /// <summary>
        /// Simmilar to <see cref="Bitmap.GetPixel"/>.
        /// </summary>
        public Color GetPixel(int x, int y)
        {
            int o = GetOffset(x, y);
            return Color.FromArgb(data[o + 2], data[o + 1], data[o]);
        }

        /// <summary>
        /// Simmilar to <see cref="Bitmap.SetPixel"/>.
        /// </summary>
        public void SetPixel(int x, int y, Color c)
        {
            int o = GetOffset(x, y);
            data[o + 2] = c.R;
            data[o + 1] = c.G;
            data[o] = c.B;
        }


        /// <summary>
        /// Clones bitmap to specified bitmap
        /// </summary>
        /// <param name="where"></param>
        public void CloneBitmap(FastBitmap where)
        {
            Data.CopyTo(where.Data, 0);
        }
      
        /// <summary>
        /// Clones bitmap
        /// </summary>
        /// <returns></returns>
        public FastBitmap CloneBitmap()
        {
            FastBitmap bmp = new FastBitmap(Width, Height);
            CloneBitmap(bmp);
            return bmp;
        }


        #endregion

        #region Conversion to Bitmap

        /// <summary>
        /// Converts this <see cref="FastBitmap"/> to <see cref="Bitmap"/>
        /// </summary>
        public Bitmap ToBitmap()
        {
            MemoryStream stream = new MemoryStream();
            ToBMPStream(stream);
            stream.Position = 0;
            Bitmap bmp = (Bitmap) Image.FromStream(stream);
            return bmp;
        }

        /// <summary>
        /// Converts <paramref name="bmp"/> <see cref="Bitmap"/> to <see cref="FastBitmap"/>
        /// </summary>
        public static FastBitmap FromBitmap(Bitmap bmp)
        {
            Bitmap bm1 = null;
            lock (bmp)
            {
                bm1 = new Bitmap(bmp.Width, bmp.Height, PixelFormat.Format24bppRgb);
                Graphics g = Graphics.FromImage(bm1);
                g.DrawImage(bmp, 0, 0);
                g.Dispose();
            }
            MemoryStream str = new MemoryStream();
            bm1.Save(str, ImageFormat.Bmp);
            str.Flush();
            str.Position = 0;
            FastBitmap bm = FromBMPStream(str);
            str.Close();
            return bm;
        }

        #endregion

        #region Saving and loading

        
        /// <summary>
        /// Saves this <see cref="FastBitmap"/> to file <paramref name="filename"/>.
        /// In BMP 24 b/pixel format.
        /// </summary>
        public void ToBMPFile(string filename)
        {
            Stream s = File.Open(filename, FileMode.OpenOrCreate, FileAccess.Write);
            ToBMPStream(s);
            s.Close();
        }

        /// <summary>
        /// Loads <see cref="FastBitmap"/> from <see cref="Stream"/> <paramref name="str"/> with image in 24b/pixel BMP format.
        /// </summary>
        public static FastBitmap FromBMPStream(Stream str)
        {
            byte[] loadBuffer = new byte[54];
            int c = str.Read(loadBuffer, 0, 54);
            if(c != 54)
                throw new FormatException("File is not BMP file");
            if(loadBuffer[0] != (byte) 'B' || loadBuffer[1] != (byte) 'M')
                throw new FormatException("File is not BMP file");
            int w = Read(loadBuffer, 18, 4);
            int h = Read(loadBuffer, 22, 4);
            FastBitmap bm = new FastBitmap(w, h);
            c = str.Read(bm.data, 0, bm.Data.Length);
            if(c != bm.Data.Length)
                throw new FormatException("File is not 24b/pixel BMP file");
            return bm;
        }

        /// <summary>
        /// Loads <see cref="FastBitmap"/> from file <paramref name="filename"/> with image in 24b/pixel BMP format.
        /// </summary>
        public static FastBitmap FromBMPFile(string filename)
        {
            Stream str = File.Open(filename, FileMode.Open, FileAccess.Read);
            FastBitmap bm = FromBMPStream(str);
            str.Close();
            return bm;
        }

        /// <summary>
        /// Loads <see cref="FastBitmap"/> from file <paramref name="filename"/>.
        /// For BMP files in 24b/pixel mode it is better to use <see cref="FromBMPFile"/>.
        /// </summary>
        public static FastBitmap FromFile(string filename)
        {
            Bitmap bmp = new Bitmap(filename);
            return FromBitmap(bmp);
        }

        #endregion

        #region Auxiliary methods for IO in file

        private static int Read(byte[] loadBuffer, int offset, int size)
        {
            int result = 0;
            for(int i = offset + size - 1; i >= offset; i--)
                result = result * 256 + loadBuffer[i];
            return result;
        }

        private static void Write(Stream str, int number)
        {
            Write(str, number, 4);
        }

        private static void Write(Stream str, int number, int size)
        {
            for(int i = 0; i < size; i++)
            {
                str.WriteByte((byte) (number % 256));
                number /= 256;
            }
        }

        /// <summary>
        /// Saves this <see cref="FastBitmap"/> to <see cref="Stream"/> <paramref name="str"/> in 24b/pixel BMP format.
        /// </summary>
        public void ToBMPStream(Stream str)
        {
            int sz = Width * Height * 3;
            str.WriteByte((byte) 'B');
            str.WriteByte((byte) 'M');
            Write(str, sz + 54); //2
            Write(str, 0); //6
            Write(str, 54); //10
            Write(str, 40); //14
            Write(str, Width); //18
            Write(str, Height); //22
            Write(str, 1, 2); //26
            Write(str, 24, 2); //28
            Write(str, 0); //30
            Write(str, sz); //34
            Write(str, 0); //38
            Write(str, 0); //42
            Write(str, 0); //46
            Write(str, 0); //50
            str.Write(data, 0, data.Length);
        }

        #endregion


        public IEnumerator<Color> Part(int x, int y, int width, int height)
        {
            var x1 = x + width;
            var y1 = y + height;
            for (var xx = x; xx < x1; xx++)
                for (var yy = y; yy < y1; yy++)
                    yield return this[xx, yy];
        }

        public IEnumerator<Color> GetEnumerator()
        {
            return Part(0, 0, Width, Height);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
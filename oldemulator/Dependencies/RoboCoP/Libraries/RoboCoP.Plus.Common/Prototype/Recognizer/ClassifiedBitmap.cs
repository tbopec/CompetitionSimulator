using System.IO;
using System.Text;
using System.Drawing;
using System;
namespace RoboCoP.Plus.Common
{
    /// <summary>
    /// Class for work with classified image
    /// </summary>
    public class ClassifiedBitmap
    {
        /// <summary>
        /// Width of classified
        /// </summary>
        public int Width { get; private set; }
        /// <summary>
        /// Height of classified
        /// </summary>
        public int Height { get; private set; }
        /// <summary>
        /// Width of original image
        /// </summary>
        public int OriginalWidth { get; private set; }
        /// <summary>
        /// Height of original image
        /// </summary>
        public int OriginalHeight { get; private set; }
        /// <summary>
        /// Coeficient of same between classified and original images
        /// </summary>
        public int CellSize { get; private set; }
        /// <summary>
        /// Array, where stored data about classified image
        /// </summary>
        int[,] map;

        /// <summary>
        /// Get class of x,y point on classified image
        /// </summary>
        /// <returns>Class of x,y point</returns>
        public int this[int x, int y]
        {
            set
            {
                if (Width > x && Height > y)
                    map[x, y] = value;
                else {
                    x = x;
                }
            }
            get { return map[x, y]; }
        }
        /// <summary>
        /// Get class of point (x,y) on classified image in original coordinats
        /// </summary>
        public int GetByOriginal(int x, int y)
        {
            if (x < OriginalWidth && y < OriginalHeight)
            {
                Console.WriteLine(x +" "+ y);
                return this[x / CellSize, y / CellSize];
            }
            else { return 0; }
        }
        /// <summary>
        /// Set class of point (x,y) on classified image in original coordinates
        /// </summary>
        public void SetByOriginal(int x, int y, int clas)
        {
            this[x / CellSize, y / CellSize] = clas;
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="originalWidth">Width of original image</param>
        /// <param name="originalHeight">Height of original image</param>
        /// <param name="cellSize">Coeficient of discretion</param>
        public ClassifiedBitmap(int originalWidth, int originalHeight, int cellSize)
        {
            OriginalWidth = originalWidth;
            OriginalHeight = originalHeight;
            CellSize = cellSize;
            SetMapSize();
            map = new int[Width, Height];
        }
        /// <summary>
        /// Rearrange width and height
        /// </summary>
        void SetMapSize()
        {
            Width = OriginalWidth / CellSize + (OriginalWidth % CellSize != 0 ? 1 : 0);
            Height = OriginalHeight / CellSize + (OriginalHeight % CellSize != 0 ? 1 : 0);
        }

        public ClassifiedBitmap() { }

        public int GetClassCount(Rectangle? rect = null)
        {
            var rectangle = rect ?? new Rectangle(0, 0, Width, Height);
            int max = 0;
            for (int x = rectangle.X; x < rectangle.Width; x++)
                for (int y = rectangle.Y; y < rectangle.Height; y++)
                    if (map[x, y] >= max) max = map[x, y] + 1;
            return max + 1;
        }

        public int GetClass(Rectangle? rect = null, double[] amp = null, int defaultClass = 0)
        {
            var rectangle = rect == null ? new Rectangle(0, 0, Width, Height): new Rectangle(rect.Value.X/CellSize, rect.Value.Y/CellSize, rect.Value.Width/CellSize, rect.Value.Height/CellSize);
            var amplification = amp ?? new double[GetClassCount(rect)];
            var clss = new double[amplification.Length];
            for (int x = rectangle.X; x < Math.Min(rectangle.Width+rectangle.X, Width); x++)
                for (int y = rectangle.Y; y < Math.Min(rectangle.Height+rectangle.Y, Height); y++)
                    if (clss.Length > this[x, y])
                        clss[this[x, y]]++;
            var indexOfMax = defaultClass;
            var max = 0.0;
            for (int i = 0; i < clss.Length; ++i)
            {
                clss[i] *= amplification[i];
                if (max < clss[i])
                {
                    max = clss[i];
                    indexOfMax = i;
                }
            }
            return indexOfMax;
        }

        /// <summary>
        /// Write to byte[]. Format: [cellsize][width of original image (in count of 256-block)][else width][height of original image (in count of 256-block)][else height][map by line]
        /// </summary>
        public byte[] Write()
        {
            var msg = new byte[1 + 2 + 2 + Width * Height];
            msg[0] = (byte)CellSize;
            msg[1] = (byte)(OriginalWidth / 256);
            msg[2] = (byte)(OriginalWidth% 256);
            msg[3] = (byte)(OriginalHeight/ 256);
            msg[4] = (byte)(OriginalHeight % 256);
            int ptr = 5;
            for (int y = 0; y < Height; y++)
                for (int x = 0; x < Width; x++)
                {
                    msg[ptr] = (byte)map[x, y];
                    ptr++;
                }
            return msg;
        }
        /// <summary>
        /// Read from byte[]
        /// </summary>
        /// <param name="msg"></param>
        public void Read(byte[] msg)
        {
            CellSize = msg[0];
            OriginalWidth = msg[1] * 256 + msg[2];
            OriginalHeight = msg[3] * 256 + msg[4];
            SetMapSize();
            int ptr = 5;
            map = new int[Width, Height];
            for (int y = 0; y < Height; y++)
                for (int x = 0; x < Width; x++)
                {
                    map[x, y] = msg[ptr];
                    ptr++;
                }
        }
    }
}

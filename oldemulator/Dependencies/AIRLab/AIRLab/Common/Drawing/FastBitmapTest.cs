/*using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace DCIMAP.GANS.Basic
{
    class FastBitmapTest
    {


        public static void Main()
        {

            FastBitmap bm = new FastBitmap(101, 101);

            for (int x=0;x<bm.Width;x++)
                for (int y = 0; y < bm.Height; y++)
                {
                    bm.SetPixel(x, y, Color.Red);
                }
            Bitmap bb = bm.ToBitmap();
            PictureBox box = new PictureBox();
            box.Image = bb;
            box.Dock = DockStyle.Fill;
            Form f = new Form();
            f.Controls.Add(box);
            Application.Run(f);
        
        }
    }
}
*/
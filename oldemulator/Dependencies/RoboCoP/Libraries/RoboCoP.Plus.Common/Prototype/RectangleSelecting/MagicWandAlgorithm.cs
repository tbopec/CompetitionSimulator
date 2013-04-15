using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Collections;
using AIRLab.Drawing;

namespace RoboCoP.Plus.Common {

    class Algorithm {

        static FastBitmap bmp;
        static Color center;

        static double Dev(Color c1, Color c2) {
            return (double)(Math.Abs(c1.R - c2.R) + Math.Abs(c1.G - c2.G) + Math.Abs(c1.B - c2.B)) / 765.0;
        }


        static double GetAverage(int x, int y, int dx, int dy, int count) {
            double S = 0;
            int cnt = 0;
            for (int i = 0; i < count; i++) {
                if (x < 0 || x >= bmp.Width) continue;
                if (y < 0 || y >= bmp.Height) continue;
                cnt++;
                S += Dev(bmp.GetPixel(x, y), center);
                x += dx;
                y += dy;
            }
            if (cnt != count) return 2;
            return S /= (double)count;
        }

        static double GetExpand(Rectangle rect, int dir) {
            switch (dir) {
                case 0: return GetAverage(rect.Left - 1, rect.Top, 0, 1, rect.Height);
                case 1: return GetAverage(rect.Left, rect.Top - 1, 1, 0, rect.Width);
                case 2: return GetAverage(rect.Right + 1, rect.Top, 0, 1, rect.Height);
                case 3: return GetAverage(rect.Left, rect.Bottom + 1, 1, 0, rect.Width);
            }
            throw new Exception();
        }


        class Pair {
            public int direction;
            public double deviace;
        }

        class PairComparer : IComparer {
            public int Compare(object x, object y) {
                return ((Pair)x).deviace.CompareTo(((Pair)y).deviace);
            }
        }


        public static Rectangle Make(FastBitmap bmp, Point p, double sens, int minSize, int margin) {
            Algorithm.bmp = bmp;
            Rectangle selection = new Rectangle(p, new Size(1, 1));
            center = bmp.GetPixel(p.X, p.Y);
            ArrayList temp = new ArrayList();


            while (true) {
                temp.Clear();
                for (int i = 0; i < 4; i++) {
                    Pair a = new Pair();
                    a.direction = i;
                    a.deviace = GetExpand(selection, i);
                    temp.Add(a);
                }
                temp.Sort(new PairComparer());

                Pair best = (Pair)temp[0];

                if (best.deviace > 1) break;
                if (best.deviace > sens) break;


                if (best.direction == 0 || best.direction == 2) {
                    selection.Width++;
                    if (best.direction == 0)
                        selection.X--;
                }
                if (best.direction == 1 || best.direction == 3) {
                    selection.Height++;
                    if (best.direction == 1)
                        selection.Y--;
                }

            }

            if (selection.Width < minSize) {
                selection.Width = minSize;
                selection.X -= minSize / 2;
            }

            if (selection.Height < minSize) {
                selection.Height = minSize;
                selection.Y -= minSize / 2;
            }

            selection.X -= margin;
            selection.Width += 2 * margin;
            selection.Y -= margin;
            selection.Height += 2 * margin;

            if (selection.X < 0) {
                selection.Width += selection.X;
                selection.X = 0;
            }
            if (selection.Y < 0) {
                selection.Height += selection.Y;
                selection.Y = 0;
            }
            if (selection.Right >= bmp.Width)
                selection.Width -= (selection.Right - bmp.Width + 1);
            if (selection.Bottom >= bmp.Height)
                selection.Height -= (selection.Bottom - bmp.Height + 1);
            return selection;
        }
    }

}

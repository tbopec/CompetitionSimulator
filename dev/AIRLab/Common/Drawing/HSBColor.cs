using System.Drawing;
//using System.Windows.Forms;
using System;

namespace AIRLab.Drawing
{
    /// <summary>
    /// Позволяет получать списки цветов из HSB-круга
    /// </summary>
    public class HSBColor //: Form
    {

    

        public static Color Create(double H, double S, double L)
        {
            H /= 360;
            double r = 0, g = 0, b = 0;

            double temp1, temp2;



            if (L == 0)
            {

                r = g = b = 0;

            }

            else
            {

                if (S == 0)
                {

                    r = g = b = L;

                }

                else
                {

                    temp2 = ((L <= 0.5) ? L * (1.0 + S) : L + S - (L * S));

                    temp1 = 2.0 * L - temp2;



                    double[] t3 = new double[] { H + 1.0 / 3.0, H, H - 1.0 / 3.0 };

                    double[] clr = new double[] { 0, 0, 0 };

                    for (int i = 0; i < 3; i++)
                    {

                        if (t3[i] < 0)

                            t3[i] += 1.0;

                        if (t3[i] > 1)

                            t3[i] -= 1.0;



                        if (6.0 * t3[i] < 1.0)

                            clr[i] = temp1 + (temp2 - temp1) * t3[i] * 6.0;

                        else if (2.0 * t3[i] < 1.0)

                            clr[i] = temp2;

                        else if (3.0 * t3[i] < 2.0)

                            clr[i] = (temp1 + (temp2 - temp1) * ((2.0 / 3.0) - t3[i]) * 6.0);

                        else

                            clr[i] = temp1;

                    }

                    r = clr[0];

                    g = clr[1];

                    b = clr[2];

                }

            }



            return Color.FromArgb((int)(255 * r), (int)(255 * g), (int)(255 * b));
        }

        /// <summary>
        /// Возвращает заданное количество цветов из цветового круга
        /// </summary>
        /// <param name="count"></param>
        /// <param name="saturation"></param>
        /// <param name="brightness"></param>
        /// <returns></returns>
        public static object[] CreateColors(int count, double saturation, double brightness)
        {
            object[] cols = new object[count];
            double delta = 360.0 / count;
            double start = 0;
            for (int i = 0; i < count; i++)
            {
                cols[i] = Create(start, saturation, brightness);
                start += delta;
            }
            return cols;


        }



        /*
        protected override void OnPaint(PaintEventArgs e)
        {
            int rad = 100;
            Pen p=new Pen(Color.White,1);
			
			
            for (double a=0;a<360;a+=0.25 )
            {
        double
                al = Math.PI*a/180;
                int x = (int) (rad*(1 + Math.Cos(al)));
                int y=(int) (rad*(1 + Math.Sin(al)));
                p.Color = ColorFromHSB(a,0.6, 0.6);
                e.Graphics.DrawLine(p, rad, rad, x, y);
            }
        }

        public static void Main()
        {
            Application.Run(new Test());
        }
        */
    }
}
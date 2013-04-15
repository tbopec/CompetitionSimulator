using System;

namespace BackEnd
{
    public class FunctionRegression
    {
        public static double Tanh(double x)
        {
            double k = (Math.Exp(2 * x) - 1) / (Math.Exp(2 * x) + 1);
            return k;
        }


        public static double Arctanh(double x)
        {
            return Math.Log((1 + x) / (1 - x)) / 2;
        }
    }
}

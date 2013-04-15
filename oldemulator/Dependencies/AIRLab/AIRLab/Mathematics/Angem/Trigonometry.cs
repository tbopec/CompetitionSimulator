using System;
using M = System.Math;

namespace AIRLab.Mathematics {
    public static partial class Angem {
        public static double Sin(Angle angle) {
            return M.Sin(angle.Radian);
        }

        public static double Cos(Angle angle) {
            return M.Cos(angle.Radian);
        }
        public static double Tg(Angle angle) {
            return M.Tan(angle.Radian);
        }

        public static Angle Asin(double value) {
            return Angle.FromRad(M.Asin(value));
        }

        public static Angle Acos(double value) {
            return Angle.FromRad(M.Acos(value));
        }

        public static Angle Atan(double value) {
            return Angle.FromRad(M.Atan(value));
        }

        public static Angle Atan2(double y, double x) {
            return Angle.FromRad(M.Atan2(y, x));
        }

        public static Angle Acos(double farSide, double firstSide, double secondSide) {
            double cos = farSide * farSide - firstSide * firstSide - secondSide * secondSide;
            cos /= (-2 * secondSide * firstSide);
            if(System.Math.Abs(cos) > 1) cos = System.Math.Sign(cos);
            return Angle.FromRad(M.Acos(cos));
        }

        public static Angle OrientedAngleDif(Angle first, Angle second)
        {
            first = first.Simplify360();
            second = second.Simplify360();
            if (second > first) return second - first;
            return second + Angle.Pi * 2 - first;

        }

        public static Angle Abs(Angle angle)
        {
            return Angle.FromGrad(Math.Abs(angle.Grad));
        }
    }
}
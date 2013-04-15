using System.Drawing;
using System;

namespace AIRLab.Mathematics {
    [Serializable]
    public struct Point2D {
        public readonly double X;
        public readonly double Y;
        public readonly bool IsEmpty;
        #region Creation
        public Point2D(double x, double y) {
            X = x;
            Y = y;
            IsEmpty = X == 0 && Y == 0;
        }
        public Point2D(Point p) : this(p.X, p.Y) { }
        public Point2D(PointF p) : this(p.X, p.Y) { }
        #endregion

        #region Conversion
        public Point ToPoint() { return new Point((int)X, (int)Y); }
        public PointF ToPointF() { return new PointF((float)X, (float)Y); }
        public Point3D ToPoint3D() { return new Point3D(X, Y, 0); }
        public Frame2D ToFrame2D() { return new Frame2D(X, Y, new Angle()); }
        #endregion

        #region Arithmetical operations
        public static Point2D operator +(Point2D p1, Point2D p2) {
            return new Point2D(p1.X + p2.X, p1.Y + p2.Y);
        }

        public static Point2D operator -(Point2D p1, Point2D p2) {
            return new Point2D(p1.X - p2.X, p1.Y - p2.Y);
        }

        public static Point2D operator -(Point2D p) {
            return new Point2D(-p.X, -p.Y);
        }

        public static Point2D operator *(Point2D p, double l) {
            return new Point2D(p.X * l, p.Y * l);
        }

        public static Point2D operator *(double l, Point2D p) {
            return p * l;
        }

        public static Point2D operator /(Point2D p, double l) {
            return p * (1 / l);
        }
        public static bool operator ==(Point2D a,Point2D b)
        {
            const double epsilon = 0.000001;
            return GetDistance(a, b) < epsilon;
        }

        public static bool operator !=(Point2D a, Point2D b)
        {
            return !(a == b);
        }

        public double MultiplyScalar(Point2D p) {
            return X * p.X + Y * p.Y;
        }

        public Point3D MultiplyVector(Point2D p) {
            return new Point3D(0, 0, X * p.Y - Y * p.X);
        }
        #endregion

        public double Norm() {
            return Angem.Hypot(X, Y);
        }

        public Point2D Normalize() {
            return this / Norm();
        }
        public static double GetDistance(Point2D a, Point2D b)
        {
            return Math.Sqrt((a.X - b.X)*(a.X - b.X) + (a.Y - b.Y)*(a.Y - b.Y));
        }

        public override string ToString() {
            return String.Format("(X={0};Y{1})",this.X,this.Y);
            //return MathIO.Point2D.ExtendedFormat.Write(this);
        }
    }
}

using System.Drawing;

namespace DCIMAP.Mathematics {
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

        public override string ToString() {
            return "";
            //return MathIO.Point2D.ExtendedFormat.Write(this);
        }
    }
}

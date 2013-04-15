﻿using System.Collections.ObjectModel;
using System;

namespace AIRLab.Mathematics {
    [Serializable]
    public struct Frame2D {
        public readonly double X;
        public readonly double Y;
        public readonly Angle Angle;
        public readonly Point2D Center;

        public Frame2D(double x, double y, Angle angle) {
            X = x;
            Y = y;
            Angle = angle;
            Center = new Point2D(x, y);
        }

        public Frame3D ToFrame3D() { return new Frame3D(X, Y, 0, new Angle(), Angle, new Angle()); }
        public Point2D ToPoint2D() { return new Point2D(X, Y); }
        public Frame2D Revert() {
            return ToFrame3D().Revert().ToFrame2D();
        }

        public Frame2D Invert()
        {
            return new Frame2D(
                -X * Angem.Cos(Angle) - Y * Angem.Sin(Angle), 
                X * Angem.Sin(Angle) - Y * Angem.Cos(Angle), 
                -Angle);
            //return ToFrame3D().Invert().ToFrame2D();
        }

        public Frame2D Apply(Frame2D arg) {
            //todo: это пипец по производительности, при случае переделать на что-то более разумное
            var res = ToFrame3D().Apply(arg.ToFrame3D());
            return new Frame2D(res.X, res.Y, res.Yaw);
        }

        public Point2D Apply(Point2D arg) {
            var res = Apply(arg.ToFrame2D());
            return new Point2D(res.X, res.Y);
        }

        public Line2D Apply(Line2D arg) {
            return new Line2D(Apply(arg.Begin), Apply(arg.End));
        }
        public override string ToString()
        {
            return String.Format("X = {0}, Y = {1}, Angle = {2}", X, Y, Angle);
        }

        #region Arithmetical operations

        public static Frame2D operator +(Frame2D a, Frame2D b) {
            return new Frame2D(a.X + b.X, a.Y + b.Y, a.Angle + b.Angle);
        }

        public static Frame2D operator -(Frame2D a, Frame2D b) {
            return new Frame2D(a.X - b.X, a.Y - b.Y, a.Angle - b.Angle);
        }

        public static Frame2D operator *(Frame2D a, double l) {
            return new Frame2D(a.X * l, a.Y * l, a.Angle * l);
        }

        public static Frame2D operator *(double l, Frame2D a) {
            return a * l;
        }

        public static Frame2D operator /(Frame2D a, double l) {
            return a * (1 / l);
        }
        public static bool operator ==(Frame2D a,Frame2D b)
        {
            const double epsilon = 0.00001;
            //Если по модулю Frame2D находятся в окрестности epsilon то они равны
            return Math.Abs(a.X - b.X) < epsilon && Math.Abs(a.Y - b.Y) < epsilon && Math.Abs(a.Angle.Radian - b.Angle.Radian) < epsilon;
        }
        public static bool operator !=(Frame2D a, Frame2D b)
        {
            return !(a==b);
        }

        #endregion
        #region Change operations

        public Frame2D NewX(double newX) { return new Frame2D(newX, Y, Angle); }
        public Frame2D NewY(double newY) { return new Frame2D(X, newY, Angle); }
        public Frame2D NewA(Angle newA) { return new Frame2D(X, Y, newA); }
        public Frame2D NewPoint(Point2D newPoint) { return new Frame2D(newPoint.X, newPoint.Y, Angle); }
        public Frame2D NewPoint(double newX, double newY) { return new Frame2D(newX, newY, Angle); }

        #endregion
    }
}
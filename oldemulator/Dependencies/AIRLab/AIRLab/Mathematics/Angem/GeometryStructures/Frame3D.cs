﻿using System;
using System.Linq;

namespace AIRLab.Mathematics {
    [Serializable]
    public struct Frame3D {
        public readonly double X;
        public readonly double Y;
        public readonly double Z;
        public readonly Angle Pitch;
        public readonly Angle Yaw;
        public readonly Angle Roll;
        private Matrix _uniMatrix;
        private Matrix UniMatrix { get { if (_uniMatrix == null) _uniMatrix = GenerateMatrix(); return _uniMatrix; } }

        private readonly bool _notIdentity;
        public bool IsIdentity { get { return !_notIdentity; } }
        public static readonly Frame3D Identity = new Frame3D(0, 0, 0);
        
        public override string ToString()
        {
			return String.Format("X = {0}, Y = {1}, Z = {2}, Pitch = {3}, Yaw = {4}, Roll = {5}", X, Y, Z, Pitch, Yaw, Roll);
			//return "REPLACE Frame3D.ToString Method when the time is right";// return MathIO.Frame3D.ExtendedFormat.Write(this);
        }
		
        public Frame3D(double x, double y, double z)
            : this(x, y, z, new Angle(), new Angle(), new Angle()) { }

        public Frame2D ToFrame2D() {
            return new Frame2D(X, Y, Yaw);
        }

        public Point3D ToPoint3D() {
            return new Point3D(X, Y, Z);
        }

        public Frame3D(double x, double y, double z, Angle pitch, Angle yaw, Angle roll)
        {
            X = x;
            Y = y;
            Z = z;
            Pitch = pitch;
            Yaw = yaw;
            Roll = roll;
            _uniMatrix = null;
            if (x == 0 && y == 0 && z == 0 && Pitch.Equals(Angle.Zero) &&
                Yaw.Equals(Angle.Zero) && Roll.Equals(Angle.Zero))
                _notIdentity = false;
            else _notIdentity = true;
            Orientation = null;
        }

        #region Работа с матрицами

        private Matrix GenerateMatrix()
        {
            var res = new Matrix(4, 4);
            var cosYaw = Angem.Cos(Yaw);
            var cosPitch = Angem.Cos(Pitch);
            var cosRoll = Angem.Cos(Roll);
            var sinYaw = Angem.Sin(Yaw);
            var sinPitch = Angem.Sin(Pitch);
            var sinRoll = Angem.Sin(Roll);
            res[0, 0] = cosYaw*cosPitch;
            res[0, 1] = cosYaw*sinPitch*sinRoll - sinYaw*cosRoll;
            res[0, 2] = cosYaw*sinPitch*cosRoll + sinYaw*sinRoll;
            res[1, 0] = sinYaw*cosPitch;
            res[1, 1] = sinYaw*sinPitch*sinRoll + cosYaw*cosRoll;
            res[1, 2] = sinYaw*sinPitch*cosRoll - cosYaw*sinRoll;
            res[2, 0] = -sinPitch;
            res[2, 1] = cosPitch*sinRoll;
            res[2, 2] = cosPitch*cosRoll;
            res[0, 3] = X;
            res[1, 3] = Y;
            res[2, 3] = Z;
            res[3, 3] = 1;
            return res;
        }

        private static Matrix FastMatrixMultiply(Matrix m0, Matrix m1)
        {
            if (m1.RowCount != m1.ColumnCount || m1.RowCount != m1.ColumnCount || m1.RowCount != 4 ||
                m1.ColumnCount != 4)
                throw new Exception("Use regular multiplication!");
            var res = new Matrix(4, 4);
            res[0, 0] = (m0[0, 0]*m1[0, 0]) + (m0[0, 1]*m1[1, 0]) + (m0[0, 2]*m1[2, 0]) + (m0[0, 3]*m1[3, 0]);
            res[0, 1] = (m0[0, 0]*m1[0, 1]) + (m0[0, 1]*m1[1, 1]) + (m0[0, 2]*m1[2, 1]) + (m0[0, 3]*m1[3, 1]);
            res[0, 2] = (m0[0, 0]*m1[0, 2]) + (m0[0, 1]*m1[1, 2]) + (m0[0, 2]*m1[2, 2]) + (m0[0, 3]*m1[3, 2]);
            res[0, 3] = (m0[0, 0]*m1[0, 3]) + (m0[0, 1]*m1[1, 3]) + (m0[0, 2]*m1[2, 3]) + (m0[0, 3]*m1[3, 3]);
            res[1, 0] = (m0[1, 0]*m1[0, 0]) + (m0[1, 1]*m1[1, 0]) + (m0[1, 2]*m1[2, 0]) + (m0[1, 3]*m1[3, 0]);
            res[1, 1] = (m0[1, 0]*m1[0, 1]) + (m0[1, 1]*m1[1, 1]) + (m0[1, 2]*m1[2, 1]) + (m0[1, 3]*m1[3, 1]);
            res[1, 2] = (m0[1, 0]*m1[0, 2]) + (m0[1, 1]*m1[1, 2]) + (m0[1, 2]*m1[2, 2]) + (m0[1, 3]*m1[3, 2]);
            res[1, 3] = (m0[1, 0]*m1[0, 3]) + (m0[1, 1]*m1[1, 3]) + (m0[1, 2]*m1[2, 3]) + (m0[1, 3]*m1[3, 3]);
            res[2, 0] = (m0[2, 0]*m1[0, 0]) + (m0[2, 1]*m1[1, 0]) + (m0[2, 2]*m1[2, 0]) + (m0[2, 3]*m1[3, 0]);
            res[2, 1] = (m0[2, 0]*m1[0, 1]) + (m0[2, 1]*m1[1, 1]) + (m0[2, 2]*m1[2, 1]) + (m0[2, 3]*m1[3, 1]);
            res[2, 2] = (m0[2, 0]*m1[0, 2]) + (m0[2, 1]*m1[1, 2]) + (m0[2, 2]*m1[2, 2]) + (m0[2, 3]*m1[3, 2]);
            res[2, 3] = (m0[2, 0]*m1[0, 3]) + (m0[2, 1]*m1[1, 3]) + (m0[2, 2]*m1[2, 3]) + (m0[2, 3]*m1[3, 3]);
            res[3, 0] = (m0[3, 0]*m1[0, 0]) + (m0[3, 1]*m1[1, 0]) + (m0[3, 2]*m1[2, 0]) + (m0[3, 3]*m1[3, 0]);
            res[3, 1] = (m0[3, 0]*m1[0, 1]) + (m0[3, 1]*m1[1, 1]) + (m0[3, 2]*m1[2, 1]) + (m0[3, 3]*m1[3, 1]);
            res[3, 2] = (m0[3, 0]*m1[0, 2]) + (m0[3, 1]*m1[1, 2]) + (m0[3, 2]*m1[2, 2]) + (m0[3, 3]*m1[3, 2]);
            res[3, 3] = (m0[3, 0]*m1[0, 3]) + (m0[3, 1]*m1[1, 3]) + (m0[3, 2]*m1[2, 3]) + (m0[3, 3]*m1[3, 3]);
            return res;
        }

        public Matrix GetMatrix() {
            return (Matrix)UniMatrix.Clone();
        }

        public static Frame3D FromMatrix(Matrix m) {
            if(m.ColumnCount != 4 || m.RowCount != 4)
                throw new ArgumentException("Only 4x4 matrix can be converted to a frame");
            double x = m[0, 3];
            double y = m[1, 3];
            double z = m[2, 3];
            //todo: Этот метод не очень хороший и не универсальный, поэтому его надо хорошо протестировать, или полностью заменить
            var yaw = Angem.Atan2(m[1, 0], m[0, 0]);
            var pitch = Angem.Atan2(-m[2, 0], Angem.Hypot(m[2, 1], m[2, 2]));
            var roll = Angem.Atan2(m[2, 1], m[2, 2]);
            return new Frame3D(x, y, z, pitch, yaw, roll);
        }

        double Minor44(Matrix m, int row, int column)
        {
            if (m.RowCount!= 4 || m.ColumnCount!=4) throw new Exception("");
            var rs= ( new[]{0,1,2,3} ).Except(new[]{row}).ToArray();
            var cs= ( new[]{0,1,2,3} ).Except(new[]{column}).ToArray();

            var c=
                + m[rs[0], cs[0]] * (m[rs[1], cs[1]] * m[rs[2], cs[2]] - m[rs[1], cs[2]] * m[rs[2], cs[1]])
                - m[rs[0], cs[1]] * (m[rs[1], cs[0]] * m[rs[2], cs[2]] - m[rs[1], cs[2]] * m[rs[2], cs[0]])
                + m[rs[0], cs[2]] * (m[rs[1], cs[0]] * m[rs[2], cs[1]] - m[rs[1], cs[1]] * m[rs[2], cs[0]]);


            c *= (row + column) % 2 == 0 ? 1 : -1;
            return c;
        }

        #endregion

        public Frame3D Invert()
        {
            var th=GetMatrix();
            var m = new Matrix(4, 4);
            for (int r = 0; r < 4; r++)
                for (int c = 0; c < 4; c++)
                    m[c, r] = Minor44(th, r, c);


            var res = th * m;
            for (int i=0;i<4;i++)
                for (int j = 0; j < 4; j++)
                {
                    var real = i == j ? 1 : 0;
                    if (Math.Abs(res[i, j] - real) > 1e-5)
                        throw new Exception("Someone cant calculate inverse matrix here");
                }

            return FromMatrix(m);
        }

        #region Производные

        public Matrix GetDerivativeOnX()
        {
            Matrix matrix = new Matrix(4, 4);
            matrix[0, 3] = 1;
            return matrix;
        }

        public Matrix GetDerivativeOnY() {
            Matrix matrix = new Matrix(4, 4);
            matrix[1, 3] = 1;
            return matrix;
        }

        public Matrix GetDerivativeOnZ() {
            Matrix matrix = new Matrix(4, 4);
            matrix[2, 3] = 1;
            return matrix;
        }

        public Matrix GetDerivativeOnYaw() {
            Matrix matrix = new Matrix(4, 4);

            double sinYaw = Angem.Sin(Yaw);
            double cosYaw = Angem.Cos(Yaw);
            double sinPitch = Angem.Sin(Pitch);
            double cosPitch = Angem.Cos(Pitch);
            double sinRoll = Angem.Sin(Roll);
            double cosRoll = Angem.Cos(Roll);

            matrix[0, 0] = -sinYaw * cosPitch;
            matrix[1, 0] = cosYaw * cosPitch;

            matrix[0, 1] = -cosYaw * cosRoll - sinYaw * sinPitch * sinRoll;
            matrix[1, 1] = -sinYaw * cosRoll + cosYaw * sinPitch * sinRoll;

            matrix[0, 2] = cosYaw * sinRoll - sinYaw * sinPitch * cosRoll;
            matrix[1, 2] = sinYaw * sinRoll + cosYaw * sinPitch * cosRoll;

            return matrix;
        }

        public Matrix GetDerivativeOnPitch() {
            Matrix matrix = new Matrix(4, 4);

            double sinYaw = Angem.Sin(Yaw);
            double cosYaw = Angem.Cos(Yaw);
            double sinPitch = Angem.Sin(Pitch);
            double cosPitch = Angem.Cos(Pitch);
            double sinRoll = Angem.Sin(Roll);
            double cosRoll = Angem.Cos(Roll);

            matrix[0, 0] = -cosYaw * sinPitch;
            matrix[1, 0] = -sinYaw * sinPitch;
            matrix[2, 0] = -cosPitch;

            matrix[0, 1] = cosYaw * cosPitch * sinRoll;
            matrix[1, 1] = sinYaw * cosPitch * sinRoll;
            matrix[2, 1] = -sinPitch * sinRoll;

            matrix[0, 2] = cosYaw * cosPitch * cosRoll;
            matrix[1, 2] = sinYaw * cosPitch * cosRoll;
            matrix[2, 2] = -sinPitch * cosRoll;

            return matrix;
        }

        public Matrix GetDerivativeOnRoll() {
            Matrix matrix = new Matrix(4, 4);

            double sinYaw = Angem.Sin(Yaw);
            double cosYaw = Angem.Cos(Yaw);
            double sinPitch = Angem.Sin(Pitch);
            double cosPitch = Angem.Cos(Pitch);
            double sinRoll = Angem.Sin(Roll);
            double cosRoll = Angem.Cos(Roll);

            matrix[0, 1] = sinYaw * sinRoll + cosYaw * sinPitch * cosRoll;
            matrix[1, 1] = -cosYaw * sinRoll + sinYaw * sinPitch * cosRoll;
            matrix[2, 1] = cosPitch * cosRoll;

            matrix[0, 2] = sinYaw * cosRoll - cosYaw * sinPitch * sinRoll;
            matrix[1, 2] = -cosYaw * cosRoll - sinYaw * sinPitch * sinRoll;
            matrix[2, 2] = -cosPitch * sinRoll;

            return matrix;
        }

        #endregion

        #region Простейшие фреймы

        public static Frame3D DoTranspose(double X, double Y, double Z) {
            return new Frame3D(X, Y, Z, new Angle(), new Angle(), new Angle());
        }

        public static Frame3D DoRoll(Angle angle) {
            return new Frame3D(0, 0, 0, new Angle(), new Angle(), angle);
        }

        public static Frame3D DoPitch(Angle angle) {
            return new Frame3D(0, 0, 0, angle, new Angle(), new Angle());
        }

        public static Frame3D DoYaw(Angle angle) {
            return new Frame3D(0, 0, 0, new Angle(), angle, new Angle());
        }

        public static Frame3D DoRotate(Point3D axis, Angle rotation) {
            var m = new Matrix(4, 4);
            m[3, 3] = 1;
            axis = axis.Normalize();
            var x = axis.X;
            var y = axis.Y;
            var z = axis.Z;
            var c = Angem.Cos(rotation);
            var s = Angem.Sin(rotation);

            m[0, 0] = x * x + (1 - x * x) * c;
            m[0, 1] = x * y * (1 - c) - z * s;
            m[0, 2] = x * z * (1 - c) + y * s;

            m[1, 0] = x * y * (1 - c) + z * s;
            m[1, 1] = y * y + (1 - y * y) * c;
            m[1, 2] = y * z * (1 - c) - x * s;

            m[2, 0] = x * z * (1 - c) - y * s;
            m[2, 1] = y * z * (1 - c) + x * s;
            m[2, 2] = z * z + (1 - z * z) * c;

            //m.Print();

            return FromMatrix(m);
        }

        #endregion

        #region Применение фреймов

        public Frame3D Apply(Frame3D arg) {
            if (IsIdentity)
                return arg;
            if (arg.IsIdentity)
                return this;
            return FromMatrix(FastMatrixMultiply(UniMatrix, arg.UniMatrix));
        }

        public Point3D Apply(Point3D arg) {
            var r = Apply(arg.ToFrame());
            return new Point3D(r.X, r.Y, r.Z);
        }

        public Line3D Apply(Line3D arg) {
            return new Line3D(Apply(arg.Begin), Apply(arg.End));
        }

        public Plane Apply(Plane arg) {
            return new Plane(Apply(arg.V1), Apply(arg.V2), Apply(arg.V3));
        }

        #endregion

        #region Arithmetical operations

        public static Frame3D operator +(Frame3D a, Frame3D b) {
            return new Frame3D(a.X + b.X, a.Y + b.Y, a.Z + b.Z, a.Pitch + b.Pitch, a.Yaw + b.Yaw, a.Roll + b.Roll);
        }

        public static Frame3D operator -(Frame3D a, Frame3D b) {
            return new Frame3D(a.X - b.X, a.Y - b.Y, a.Z - b.Z, a.Pitch - b.Pitch, a.Yaw - b.Yaw, a.Roll - b.Roll);
        }

        public static Frame3D operator *(Frame3D a, double l) {
            return new Frame3D(a.X * l, a.Y * l, a.Z * l, a.Pitch * l, a.Yaw * l, a.Roll * l);
        }

        public static Frame3D operator *(double l, Frame3D a) {
            return a * l;
        }

        public static Frame3D operator /(Frame3D a, double l) {
            return a * (1 / l);
        }


        #endregion

        #region Change operations

        public Frame3D NewX(double newX) { return new Frame3D(newX, Y, Z, Pitch, Yaw, Roll); }
        public Frame3D NewY(double newY) { return new Frame3D(X, newY, Z, Pitch, Yaw, Roll); }
        public Frame3D NewZ(double newZ) { return new Frame3D(X, Y, newZ, Pitch, Yaw, Roll); }
        public Frame3D NewPitch(Angle newPitch) { return new Frame3D(X, Y, Z, newPitch, Yaw, Roll); }
        public Frame3D NewYaw(Angle newYaw) { return new Frame3D(X, Y, Z, Pitch, newYaw, Roll); }
        public Frame3D NewRoll(Angle newRoll) { return new Frame3D(X, Y, Z, Pitch, Yaw, newRoll); }


        public Frame3D NewPoint(double newX, double newY, double newZ) { return new Frame3D(newX, newY, newZ, Pitch, Yaw, Roll); }
        public Frame3D NewPoint(Point3D newPoint) { return new Frame3D(newPoint.X, newPoint.Y, newPoint.Z, Pitch, Yaw, Roll); }

        public Frame3D Revert() {
            Matrix matrix = new Matrix(4, 4);

            matrix[0, 0] = UniMatrix[0, 0];
            matrix[1, 1] = UniMatrix[1, 1];
            matrix[2, 2] = UniMatrix[2, 2];

            matrix[0, 1] = UniMatrix[1, 0];
            matrix[0, 2] = UniMatrix[2, 0];
            matrix[1, 0] = UniMatrix[0, 1];
            matrix[1, 2] = UniMatrix[2, 1];
            matrix[2, 0] = UniMatrix[0, 2];
            matrix[2, 1] = UniMatrix[1, 2];

            return Frame3D.FromMatrix(matrix * Frame3D.DoTranspose(-X, -Y, -Z).GetMatrix());
        }

        #endregion

        #region Equals
        public static bool operator ==(Frame3D one, Frame3D two)
        {
            return one.Equals(two);
        }

        public static bool operator !=(Frame3D one, Frame3D two)
        {
            return !(one == two);
        }

        public override bool Equals(object obj)
        {
            if ((obj is Frame3D))
                return Equals((Frame3D)obj);
            return false;
        }

        public bool Equals(Frame3D other)
        {
            return other.X.Equals(X) && other.Y.Equals(Y) && other.Z.Equals(Z)
                && other.Pitch.Radian.Equals(Pitch.Radian) &&
                other.Yaw.Radian.Equals(Yaw.Radian) && other.Roll.Radian.Equals(Roll.Radian);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = X.GetHashCode();
                result = (result * 397) ^ Y.GetHashCode();
                result = (result * 397) ^ Z.GetHashCode();
                result = (result * 397) ^ Pitch.Radian.GetHashCode();
                result = (result * 397) ^ Yaw.Radian.GetHashCode();
                result = (result * 397) ^ Roll.Radian.GetHashCode();
                return result;
            }
        }
        #endregion

		/// <summary>
		/// Пока неполноценный кватернион
		/// </summary>
		public Quat? Orientation;
    }
}
using System;
using System.Collections.Generic;
using System.Text;
using DCIMAP.Mathematics;

namespace DCIMAP.Mathematics {
    public partial class Angem {
        /*
         * There are some not obvious calculations.
         * With them we can find the closest points of 
         * two straits. Function returns mean point of 
         * them.
         * p11, p21 - some points of straits.
         * p12, p22 - some other points of straits.
         * a1, a2 - vectors of straits.
         */
        public static Point2D getCrossing(Line2D line1, Line2D line2) {
            double a1 = line1.Direction.X;
            double b1 = line1.Direction.Y;
            double a2 = line2.Direction.X;
            double b2 = line2.Direction.Y;
            double c1 = line2.Begin.X - line1.Begin.X;
            double c2 = line2.Begin.Y - line1.Begin.Y;

            double t1 = (c1 * b2 - a2 * c2) * 1.0 / (a1 * b2 - a2 * b1);

            return new Point2D(line1.Begin.X + a1 * t1, line1.Begin.Y + b1 * t1);
        }

        public static Line3D GetPerpendicular(Line3D line1, Line3D line2) {
            Point3D p11 = line1.Begin;
            Point3D p21 = line2.Begin;
            Point3D a1 = line1.Direction.Normalize();
            Point3D a2 = line2.Direction.Normalize();

            if(a1.X == a2.X && a1.Y == a2.Y && a1.Z == a2.Z)
                throw new Exception("straits are parallel");

            Point3D p12 = new Point3D(p11.X + a1.X, p11.Y + a1.Y, p11.Z + a1.Z);
            Point3D p22 = new Point3D(p21.X + a2.X, p21.Y + a2.Y, p21.Z + a2.Z);

            double P1 = (p12.X - p11.X) * (p12.X - p11.X) + (p12.Y - p11.Y) * (p12.Y - p11.Y) + (p12.Z - p11.Z) * (p12.Z - p11.Z);
            double P2 = (p12.X - p11.X) * (p22.X - p21.X) + (p12.Y - p11.Y) * (p22.Y - p21.Y) + (p12.Z - p11.Z) * (p22.Z - p21.Z);
            double Q1 = -((p22.X - p21.X) * (p12.X - p11.X) + (p22.Y - p21.Y) * (p12.Y - p11.Y) + (p22.Z - p21.Z) * (p12.Z - p11.Z));
            double Q2 = -((p22.X - p21.X) * (p22.X - p21.X) + (p22.Y - p21.Y) * (p22.Y - p21.Y) + (p22.Z - p21.Z) * (p22.Z - p21.Z));
            double R1 = (p21.X - p11.X) * (p12.X - p11.X) + (p21.Y - p11.Y) * (p12.Y - p11.Y) + (p21.Z - p11.Z) * (p12.Z - p11.Z);
            double R2 = (p21.X - p11.X) * (p22.X - p21.X) + (p21.Y - p11.Y) * (p22.Y - p21.Y) + (p21.Z - p11.Z) * (p22.Z - p21.Z);

            double m = (Q2 * R1 - Q1 * R2) / (P1 * Q2 - P2 * Q1);
            double n = (P1 * R2 - P2 * R1) / (P1 * Q2 - P2 * Q1);

            double x1 = p11.X + m * (p12.X - p11.X);
            double y1 = p11.Y + m * (p12.Y - p11.Y);
            double z1 = p11.Z + m * (p12.Z - p11.Z);

            double x2 = p21.X + n * (p22.X - p21.X);
            double y2 = p21.Y + n * (p22.Y - p21.Y);
            double z2 = p21.Z + n * (p22.Z - p21.Z);

            return new Line3D(new Point3D(x1, y1, z1), new Point3D(x2, y2, z2));
        }


        /// <summary>
        /// this method translates from Spherical coordinates system to orthonormal.
        /// </summary>
        /// <returns></returns>
        public static Point3D FromSphericToOrthonorm(Angle phi, Angle psi, Double r) {
            double x = Sin(psi) * Cos(phi);
            double y = Sin(psi) * Sin(phi);
            double z = Cos(psi);

            return new Point3D(x, y, z) * r;
        }

        public static Tuple<Angle, Angle, Double> FromOrthonormToSpheric(Point3D point) {
            double r = System.Math.Sqrt(point.X * point.X + point.Y * point.Y + point.Z * point.Z);
            Angle psi = Acos(point.Z / r);
            Angle phi = Atan2(point.Y, point.X);
            return new Tuple<Angle, Angle, double>(phi, psi, r);
        }

        public static bool IsParallel(Line3D line1, Line3D line2) {
            return AreCollinear(line1.Direction, line2.Direction);
        }

        public static bool IsParallel(Line2D line1, Line2D line2) {
            return AreCollinear(line1.Direction, line2.Direction);
        }

        public static double Distance(Point3D point1, Point3D point2) {
            return (point2 - point1).Norm();
        }

        public static double Distance(Point2D point1, Point2D point2) {
            return (point2 - point1).Norm();
        }

        public static double Distance(Point3D point, Line3D line) {
            Point3D vector = point - line.Begin;
            return vector.MultiplyVector(line.Direction).Norm() / line.Direction.Norm();
        }

        public static double Distance(Point2D point, Line2D line) {
            Point2D vector = point - line.Begin;
            return vector.MultiplyVector(line.Direction).Norm() / line.Direction.Norm();
        }

        /// <summary>
        /// Проверка, лежит ли точка внутри выпуклой области, заданной массивом точек
        /// </summary>
        /// <returns></returns>
        public static bool PointIsFromRegion(Point2D thisPoint, Point2D[] regionPoints) {
            /*
            int scalarMul = 0;
            for (int i = 0; i < regionPoints.Length; ++i)
            {
                Point2D firstPoint = regionPoints[i];
                Point2D secondPoint = regionPoints[(i==regionPoints.Length-1)?0:(i+1)];
                Point2D firstVector = new Point2D(secondPoint.X - firstPoint.X, secondPoint.Y - firstPoint.Y);
                Point2D secondVector = new Point2D(thisPoint.X - firstPoint.X, thisPoint.Y - firstPoint.Y);
                int tmpScalarMul = System.Math.Sign(firstVector.X * secondVector.X + firstVector.Y * secondVector.Y);
                if (scalarMul == 0)
                    scalarMul = tmpScalarMul;
                if (scalarMul * tmpScalarMul < 0)
                    return false;
            }
            return true;
            */
            Point2D firstVector = new Point2D(
                thisPoint.X - regionPoints[regionPoints.Length - 1].X,
                thisPoint.Y - regionPoints[regionPoints.Length - 1].Y);
            Point2D secondVector = new Point2D(
                regionPoints[0].X - regionPoints[regionPoints.Length - 1].X,
                regionPoints[0].Y - regionPoints[regionPoints.Length - 1].Y);
            double mul = firstVector.MultiplyScalar(secondVector);
            for(int i = 0; i < regionPoints.Length - 1; ++i) {
                firstVector = new Point2D(
                    thisPoint.X - regionPoints[i].X,
                    thisPoint.Y - regionPoints[i].Y);
                secondVector = new Point2D(
                    regionPoints[i + 1].X - regionPoints[i].X,
                    regionPoints[i + 1].Y - regionPoints[i].Y);
                double currentMul = firstVector.MultiplyScalar(secondVector);
                if(mul * currentMul < 0)
                    return false;
            }
            return true;
        }
    }
}

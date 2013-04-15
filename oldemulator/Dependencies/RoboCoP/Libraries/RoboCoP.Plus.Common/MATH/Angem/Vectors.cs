using System;
using M = System.Math;

namespace DCIMAP.Mathematics {
    partial class Angem {
        public static double Hypot(double x, double y) {
            return M.Sqrt(x * x + y * y);
        }

        public static double Hypot(double x, double y, double z) {
            return M.Sqrt(x * x + y * y + z * z);
        }

        public static double Hypot(Point2D p1, Point2D p2) {
            return Hypot(p1.X - p2.X, p1.Y - p2.Y);
        }

        public static double Hypot(Point3D p1, Point3D p2) {
            return Hypot(p1.X - p2.X, p1.Y - p2.Y, p1.Z - p2.Z);
        }


        public static Point3D Projection(Point3D toProject, Point3D axis) {
            if(axis.IsEmpty) throw new ArgumentException("Axis of projection cannot be null-vector");
            return (toProject.Norm() * Cos(AngleBetweenVectors(toProject, axis))) * axis.Normalize();
        }

        public static Point3D Projection(Point3D toProject, Plane plane) {
            return toProject - Projection(toProject, plane.Normal);
        }

        public static Point2D Projection(Point2D toProject, Point2D axis) {
            if(axis.IsEmpty) throw new ArgumentException("Axis of projection cannot be null-vector");
            return (toProject.Norm() * Cos(AngleBetweenVectors(toProject, axis))) * axis.Normalize();
        }


        public static Point3D Orthonorm(Point3D vector, Point3D axis) {
            return (vector - Projection(vector, axis)).Normalize();
        }
        public static Point2D Orthonorm(Point2D vector, Point2D axis) {
            return (vector - Projection(vector, axis)).Normalize();
        }


        public static Angle AngleBetweenVectors(Point3D firstVector, Point3D secondVector) {
            if(firstVector.IsEmpty || secondVector.IsEmpty)
                throw new ArgumentException("Vectors cannot be null-vector when calculating angle");
            var cos = firstVector.MultiplyScalar(secondVector) / (firstVector.Norm() * secondVector.Norm());
            if(Math.Abs(cos) > 1)
                cos = Math.Sign(cos) * 1;
            return Acos(cos);
        }

        public static Angle AngleBetweenVectors(Point2D firstVector, Point2D secondVector) {
            return AngleBetweenVectors(firstVector.ToPoint3D(), secondVector.ToPoint3D());
        }

        public static bool AreCollinear(Point3D a, Point3D b) {
            return a.MultiplyVector(b).Norm() == 0;
        }

        public static bool AreCollinear(Point2D a, Point2D b) {
            return a.MultiplyVector(b).Norm() == 0;
        }

    }
}

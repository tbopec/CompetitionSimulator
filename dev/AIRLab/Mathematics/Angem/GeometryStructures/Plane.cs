using System;
namespace AIRLab.Mathematics {
    [Serializable]
    public class Plane {
        public readonly Point3D V1;
        public readonly Point3D V2;
        public readonly Point3D V3;

        public readonly Point3D Center;
        public readonly Point3D Basis1;
        public readonly Point3D Basis2;

        public readonly Point3D Normal;

        public Plane(Point3D V1, Point3D V2, Point3D V3) {
            this.V1 = V1;
            this.V2 = V2;
            this.V3 = V3;
            this.Center = V1;
            this.Basis1 = (V2 - V1).Normalize();
            this.Basis2 = Angem.Orthonorm(V3 - V1, Basis1);
            this.Normal = Basis1.MultiplyVector(Basis2).Normalize();
        }

        public static Plane FromDirection(Point3D center, Point3D direction1, Point3D direction2) {
            return new Plane(center, center + direction1, center + direction2);
        }

        public static Plane FromNormalVector(Point3D center, Point3D normalVector) {
            var some = new Point3D(1, 1, 1);
            if(Angem.AreCollinear(some, normalVector))
                some = new Point3D(1, 2, 1);
            var bas1 = Angem.Orthonorm(some, normalVector);
            return new Plane(center, center + bas1, center + bas1.MultiplyVector(normalVector));
        }
    }
}
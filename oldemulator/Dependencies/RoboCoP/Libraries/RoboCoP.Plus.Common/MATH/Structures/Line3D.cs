namespace DCIMAP.Mathematics {
    public struct Line3D {
        public readonly Point3D Begin;
        public readonly Point3D End;
        public readonly Point3D Direction;
        public readonly bool IsEmpty;

        public Line3D(Point3D Begin, Point3D End) {
            this.Begin = Begin;
            this.End = End;
            Direction = End - Begin;
            IsEmpty = Direction.IsEmpty;
        }

        public static Line3D FromDirection(Point3D Begin, Point3D Direction) {
            return new Line3D(Begin, Begin + Direction);
        }

        public Point3D GetCenter() {
            return (Begin + End) / 2;
        }
    }
}
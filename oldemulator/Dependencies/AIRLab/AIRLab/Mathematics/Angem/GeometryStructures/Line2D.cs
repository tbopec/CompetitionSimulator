using System.Drawing;
using System;

namespace AIRLab.Mathematics {
    [Serializable]
    public struct Line2D {
        public readonly Point2D Begin;
        public readonly Point2D End;
        public readonly Point2D Direction;
        public readonly Point2D Normal;
        public readonly double EqA;
        public readonly double EqB;
        public readonly double EqC;
        public readonly bool IsEmpty;

        public Line2D(Point2D Begin, Point2D End) {
            this.Begin = Begin;
            this.End = End;
            Direction = End - Begin;
            Normal = new Point2D(-Direction.Y, Direction.X);
            EqA = Normal.X;
            EqB = Normal.Y;
            EqC = -EqA * Begin.X - EqB * Begin.Y;
            IsEmpty = Direction.IsEmpty;
        }

        public static Line2D FromDirection(Point2D begin, Point2D direction) {
            return new Line2D(begin, begin + direction);
        }

        public double getXbyY(double y) {
            if(Direction.Y == 0)
                throw new Exception("There are too many points or zero on line.");
            return Begin.X + Direction.X * (y - Begin.Y) / Direction.Y;
        }

        public double getYbyX(double x) {
            if(Direction.X == 0)
                throw new Exception("There are too many points or zero on line.");
            return Begin.Y + Direction.Y * (x - Begin.X) / Direction.X;
        }
    }
}

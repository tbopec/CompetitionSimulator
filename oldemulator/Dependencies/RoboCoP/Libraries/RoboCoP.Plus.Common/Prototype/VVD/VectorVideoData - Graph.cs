using AIRLab.Mathematics;

namespace RoboCoP.Plus.Common {
    partial class Vertex {
        ///<summary>
        ///Создает NumberedPoint
        ///</summary>
        public Vertex(int x, int y) {
            this.Picture = new Point2D(x, y); ;
        }
    }

    partial class Edge {
        public bool Incident(Vertex v) {
            return Point1 == v || Point2 == v;
        }

        public Vertex AnotherPoint(Vertex v) {
            if(Point1 == v) return Point2;
            if(Point2 == v) return Point1;
            return null;
        }
    }


}

using AIRLab.Thornado;
using AIRLab.Thornado.IOs;
using System.Drawing;

namespace RoboCoP.Plus.Common {
    public enum Drawing2DItemType {
        Point,
        Line,
        Rectangle,
        String
    }

    public class Drawing2DItem {
        [ThornadoField("", typeof(EnumIO<Drawing2DItemType>))]
        public Drawing2DItemType Type;
        [ThornadoField("", typeof(BoolIO))]
        public bool Filled;
        [ThornadoField("", typeof(PenIO), TypeIOModifier.Nullable)]
        public Pen Pen;
        [ThornadoField("", typeof(BrushIO), TypeIOModifier.Nullable)]
        public Brush Brush;
        [ThornadoField("", typeof(PointIO))]
        public Point Point1;
        [ThornadoField("", typeof(PointIO))]
        public Point Point2;
        [ThornadoField("", typeof(StringIO))]
        public string String = "";

        public static Font Font = new Font("Times New Roman", 10);

        public void Draw(Graphics g) {
            switch(Type) {
                case Drawing2DItemType.Point:
                g.FillEllipse(Brush, Point1.X, Point1.Y, 1, 1);
                break;
                case Drawing2DItemType.Line:
                g.DrawLine(Pen, Point1, Point2);
                break;
                case Drawing2DItemType.String:
                g.DrawString(String, Font, Brush, Point1);
                break;
                case Drawing2DItemType.Rectangle:
                if(!Filled)
                    g.DrawRectangle(Pen, Point1.X, Point1.Y, Point2.X, Point2.Y);
                else
                    g.FillRectangle(Brush, Point1.X, Point1.Y, Point2.X, Point2.Y);
                break;
            }
        }
    }
}

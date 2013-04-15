using System.Collections.Generic;
using System.Drawing;

namespace RoboCoP.Plus.Common {
    public class Drawing2D : List<Drawing2DItem> {
        public void DrawPoint(Color c, Point p) {
            Add(new Drawing2DItem() { Type = Drawing2DItemType.Point, Brush = new SolidBrush(c), Point1 = p });
        }

        public void DrawLine(Color c, Point p1, Point p2) {
            Add(new Drawing2DItem() { Type = Drawing2DItemType.Line, Pen = new Pen(c), Point1 = p1, Point2 = p2 });
        }

        public void DrawRectangle(Color c, Rectangle rect) {
            Add(new Drawing2DItem() { Type = Drawing2DItemType.Rectangle, Pen = new Pen(c), Point1 = rect.Location, Point2 = new Point(rect.Width, rect.Height) });
        }

        public void FillRectangle(Color c, Rectangle rect) {
            Add(new Drawing2DItem() { Type = Drawing2DItemType.Rectangle, Filled = true, Brush = new SolidBrush(c), Point1 = rect.Location, Point2 = new Point(rect.Width, rect.Height) });
        }

        public void DrawString(Color c, string s, Point p1) {
            Add(new Drawing2DItem() { Type = Drawing2DItemType.String, Brush = new SolidBrush(c), Point1 = p1, String = s });
        }

        public void Draw(Graphics g) {
            foreach(var e in this)
                e.Draw(g);
        }

    }
}

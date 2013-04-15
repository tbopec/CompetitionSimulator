using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using AIRLab.Drawing;
using System.Windows.Forms;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using AIRLab.Thornado;

namespace RoboCoP.Plus.Common {

    /// <summary>
    /// Form which allows to choose a classes for subimages, store samples into file etc.
    /// </summary>
    public class RectangleSelecting : Form {

        public Bitmap bitmap;
        public FastBitmap fastBitmap;
        public RecognitionBase recBase;

        public RectangleSelecting() {
            bitmap = new Bitmap(1, 1);
            fastBitmap = FastBitmap.FromBitmap(bitmap);
        }

        #region Receiving and preparing data

        protected override void OnPaint(PaintEventArgs e) {
            base.OnPaint(e);
            e.Graphics.DrawImage(bitmap, 0, 0);
            e.Graphics.DrawRectangle(Pens.Red, selection);

        }

        #endregion

        #region Preparing data
        int selectSize = 10;

        enum Mode { Pick, Paint, Custom }
        Mode mode = Mode.Pick;
        Rectangle selection;
        Point startPaint;

        void MakePickSelection() {
            var Location = PointToClient(Control.MousePosition);
            selection = new Rectangle(Location.X - selectSize, Location.Y - selectSize, 2 * selectSize, 2 * selectSize);
        }

        protected override void OnMouseWheel(MouseEventArgs e) {
            var delta = Math.Sign(e.Delta);
            switch (mode) {
                case Mode.Pick:
                    selectSize += delta;
                    if (selectSize < 1) selectSize = 1;
                    MakePickSelection();
                    Invalidate();
                    break;
                case Mode.Custom:
                    try {
                        selection = new Rectangle(
                            selection.X - delta,
                            selection.Y - delta,
                            selection.Width + 2 * delta,
                            selection.Height + 2 * delta);
                        Invalidate();
                    }
                    catch { }
                    break;
            }
        }


        protected override void OnMouseMove(MouseEventArgs e) {
            switch (mode) {
                case Mode.Pick:
                    MakePickSelection();
                    Invalidate();
                    break;
                case Mode.Paint:
                    selection = new Rectangle(
                        Math.Min(startPaint.X, e.Location.X),
                        Math.Min(startPaint.Y, e.Location.Y),
                        Math.Abs(startPaint.X - e.Location.X),
                        Math.Abs(startPaint.Y - e.Location.Y));
                    Invalidate();
                    break;
            }
        }

        protected override void OnMouseDown(MouseEventArgs e) {
            if (e.Button == MouseButtons.Right) {
                mode = Mode.Pick;
            }
            if (e.Button == MouseButtons.Left) {
                mode = Mode.Paint;
                startPaint = e.Location;
            }
        }

        protected override void OnMouseUp(MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                mode = Mode.Custom;
                var delta = Math.Abs(e.Location.X - startPaint.X) + Math.Abs(e.Location.Y - startPaint.Y);
                if (delta < 3 && e.X < fastBitmap.Width && e.Y < fastBitmap.Height)
                    selection = Algorithm.Make(fastBitmap, e.Location, 0.1, selectSize, 0);
                Invalidate();
            }
        }

        public FastBitmap GetSample() {
            var x0 = selection.Left;
            var y0 = selection.Top;
            var x1 = selection.Right;
            var y1 = selection.Bottom;

            if (x0 == x1 || y0 == y1) return null;
            FastBitmap res = new FastBitmap(x1 - x0 + 1, y1 - y0 + 1);
            for (int x = x0; x <= x1; x++)
                for (int y = y0; y <= y1; y++)
                    res.SetPixel(x - x0, y - y0, fastBitmap.GetPixel(x, y));
            return res;

        }

        public Rectangle GetSelectionRectangle() {
            return selection;
        }

        #endregion
    }

}

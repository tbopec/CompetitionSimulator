using AIRLab.Thornado;
using AIRLab.Thornado.IOs;
using System.Drawing;

namespace RoboCoP.Plus.Common {

    public partial class RectangleRecognitionBase {

        [ThornadoField("File name", typeof(StringIO))]
        public string FileName { get; set; }

        [ThornadoField("Rectangle coordinates", typeof(RectangleIO))]
        public Rectangle Rect { get; set; }

        [ThornadoField("Class", typeof(IntIO))]
        public int Class { get; set; }

        public RectangleRecognitionBase() {
            FileName = "";
        }

    }

}

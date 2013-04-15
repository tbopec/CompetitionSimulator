namespace RoboCoP.Plus.Common {
    using System.Drawing;

    using AIRLab.Thornado;
    using AIRLab.Thornado.IOs;

    public enum MaskType {
        No,
        RecognizedImage,
        Hue,
        Saturation,
        Lightness,
        R,
        G,
        B
    }


    ///<summary>
    ///Сгенерированно системой кодегенерации Thornado
    ///</summary>
    public partial class VideoDataDrawerSettings {

        ///<summary>
        ///Создает VideoDataDrawerSettings
        ///</summary>
        public VideoDataDrawerSettings() {
            ClassColors = new Color[0];
            Precision = 3;
        }

        ///<summary>
        ///Массив цветов, соответствующих классу. Используется для подсветки распознанного изображения, и для рисование BoundingBox
        ///</summary>
        [ThornadoField("Массив цветов, соответствующих классу. Используется для подсветки распознанного изображения, и для рисование BoundingBox", typeof(NamedColorIO), TypeIOModifier.InArray)]
        public Color[] ClassColors { get; set; }

        [ThornadoField("Количество знаков после запятой в отображаемых строках")]
        public int Precision { get; set; }

        ///<summary>
        ///Настройки отрисовки растра - оригинального и распознанного
        ///</summary>
        RasterDrawing __raster = new RasterDrawing();
        ///<summary>
        ///Настройки отрисовки растра - оригинального и распознанного
        ///</summary>
        [ThornadoNode("Настройки отрисовки растра - оригинального и распознанного")]
        public RasterDrawing Raster { get { return __raster; } }
        ///<summary>
        ///Настройки отрисовки тел
        ///</summary>
        BodiesDrawing __bodies = new BodiesDrawing();
        ///<summary>
        ///Настройки отрисовки тел
        ///</summary>
        [ThornadoNode("Настройки отрисовки тел")]
        public BodiesDrawing Bodies { get { return __bodies; } }
        ///<summary>
        ///Настройки отрисовки графа (вершин и ребер)
        ///</summary>
        GraphDrawing __graph = new GraphDrawing();
        ///<summary>
        ///Настройки отрисовки графа (вершин и ребер)
        ///</summary>
        [ThornadoNode("Настройки отрисовки графа (вершин и ребер)")]
        public GraphDrawing Graph { get { return __graph; } }

        [ThornadoField("", typeof(BoolIO))]
        public bool DrawAdditionalData { get; set; }

    }

    ///<summary>
    ///Сгенерированно системой кодегенерации Thornado
    ///</summary>
    public partial class RasterDrawing {

        ///<summary>
        ///Создает RasterDrawing
        ///</summary>
        public RasterDrawing() {
            DrawOriginal = true;
            RecognizedMaskPercent = 0.5;
        }

        ///<summary>
        ///TRUE, если необходимо отрисовывать оригинальное изображение
        ///</summary>
        [ThornadoField("TRUE, если необходимо отрисовывать оригинальное изображение", typeof(BoolIO))]
        public bool DrawOriginal { get; set; }

        [ThornadoField("Type of the mask to be applied to original image", typeof(EnumIO<MaskType>))]
        public MaskType MaskType { get; set; }

        ///<summary>
        ///Сила маски (0 - отсутствие маски, 1 - отсутсвие оригинального изображения)
        ///</summary>
        [ThornadoField("Strength of recognized mask over original image (0 - no mask, 1 - no image)", typeof(DoubleIO))]
        public double RecognizedMaskPercent { get; set; }

    }

}

namespace RoboCoP.Plus.Common {
    using System.Drawing;

    using AIRLab.Thornado;
    using AIRLab.Thornado.IOs;


    ///<summary>
    ///Сгенерированно системой кодегенерации Thornado
    ///</summary>
    public partial class GraphDrawing {

        ///<summary>
        ///Создает GraphDrawing
        ///</summary>
        public GraphDrawing() {
            VertexSize = 3;
            VertexColor = Color.Lime;
            EdgeWidth = 2f;
            EdgeColor = Color.Black;
        }

        ///<summary>
        ///Радиус круга, соответствующей одной вершине
        ///</summary>
        [ThornadoField("Радиус круга, соответствующей одной вершине", typeof(IntIO))]
        public int VertexSize { get; set; }

        ///<summary>
        ///Цвет вершин
        ///</summary>
        [ThornadoField("Цвет вершин", typeof(NamedColorIO))]
        public Color VertexColor { get; set; }

        ///<summary>
        ///Толищна линий-ребер
        ///</summary>
        [ThornadoField("Толищна линий-ребер", typeof(DoubleIO))]
        public double EdgeWidth { get; set; }

        ///<summary>
        ///Цвет линий-ребер
        ///</summary>
        [ThornadoField("Цвет линий-ребер", typeof(NamedColorIO))]
        public Color EdgeColor { get; set; }

        ///<summary>
        ///TRUE, если нужно отобразить координату вершины на изображении
        ///</summary>
        [ThornadoField("TRUE, если нужно отобразить координату вершины на изображении", typeof(BoolIO))]
        public bool DrawVertexPicture { get; set; }

        ///<summary>
        ///TRUE, если нужно отобразить координаты вершины на всех изображениях
        ///</summary>
        [ThornadoField("TRUE, если нужно отобразить координаты вершины на всех изображениях", typeof(BoolIO))]
        public bool DrawVertexAllPicture { get; set; }

        ///<summary>
        ///TRUE, если нужно отобразить реальную координату точки
        ///</summary>
        [ThornadoField("TRUE, если нужно отобразить реальную координату точки", typeof(BoolIO))]
        public bool DrawVertexReal { get; set; }



    }
}

namespace RoboCoP.Plus.Common {

    using AIRLab.Thornado;
    using AIRLab.Thornado.IOs;


    ///<summary>
    ///Сгенерированно системой кодегенерации Thornado
    ///</summary>
    public partial class BodiesDrawing {

        ///<summary>
        ///Создает BodiesDrawing
        ///</summary>
        public BodiesDrawing() {
            DrawBoundingBox = true;
            DrawPictureLocation = false;
            DrawRealLocation = true;
            DrawYawArrow = false;
        }

        ///<summary>
        ///TRUE, если нужно отрисовывать BoundingBox объектов
        ///</summary>
        [ThornadoField("TRUE, если нужно отрисовывать BoundingBox объектов", typeof(BoolIO))]
        public bool DrawBoundingBox { get; set; }

        ///<summary>
        ///TRUE, если нужно отображать координаты объектов на изображении
        ///</summary>
        [ThornadoField("TRUE, если нужно отображать координаты объектов на изображении", typeof(BoolIO))]
        public bool DrawPictureLocation { get; set; }

        ///<summary>
        ///TRUE, если нужно отображать реальные координаты объектов
        ///</summary>
        [ThornadoField("TRUE, если нужно отображать реальные координаты объектов", typeof(BoolIO))]
        public bool DrawRealLocation { get; set; }

        ///<summary>
        ///TRUE, если следует отрисовывать стрелку-вектор направления Yaw
        ///</summary>
        [ThornadoField("TRUE, если следует отрисовывать стрелку-вектор направления Yaw", typeof(BoolIO))]
        public bool DrawYawArrow { get; set; }

    }

}
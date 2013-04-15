namespace RoboCoP.Plus.Common {
    using System.Collections.Generic;
    using AIRLab.Thornado;

    ///<summary>
    ///Сгенерированно системой кодегенерации Thornado
    ///</summary>
    public partial class VectorVideoData : ICheckable {

        ///<summary>
        ///Создает VectorVideoData
        ///</summary>
        public VectorVideoData() { }

        ///<summary>
        ///Список вершин (точек), обнаруженных на изображении
        ///</summary>
        List<Vertex> __vertices = new List<Vertex>();
        ///<summary>
        ///Список вершин (точек), обнаруженных на изображении
        ///</summary>
        [ThornadoList("Список вершин (точек), обнаруженных на изображении")]
        public List<Vertex> Vertices { get { return __vertices; } }
        ///<summary>
        ///Список ребер (линий), обнаруженных на изображении
        ///</summary>
        List<Edge> __edges = new List<Edge>();
        ///<summary>
        ///Список ребер (линий), обнаруженных на изображении
        ///</summary>
        [ThornadoList("Список ребер (линий), обнаруженных на изображении")]
        public List<Edge> Edges { get { return __edges; } }
        ///<summary>
        ///Список объектов, обнаруженных на изображении
        ///</summary>
        List<Body> __bodies = new List<Body>();
        ///<summary>
        ///Список объектов, обнаруженных на изображении
        ///</summary>
        [ThornadoList("Список объектов, обнаруженных на изображении")]
        public List<Body> Bodies { get { return __bodies; } }
        ///<summary>
        ///Список камер
        ///</summary>
        List<Camera> __cameras = new List<Camera>();
        ///<summary>
        ///Список камер
        ///</summary>
        [ThornadoList("Список камер")]
        public List<Camera> Cameras { get { return __cameras; } }

        Drawing2D __addition2D = new Drawing2D();

        [ThornadoList("Additional video information")]
        public Drawing2D Addition2D { get { return __addition2D; } }

    }

}

namespace RoboCoP.Plus.Common {
    using System.Drawing;
    using AIRLab.Thornado;

    using AIRLab.Thornado.IOs;
    using AIRLab.Mathematics;


    ///<summary>
    ///Сгенерированно системой кодегенерации Thornado
    ///</summary>
    public partial class Vertex {
        ///<summary>
        ///Расположение точки на растре
        ///</summary>
        [ThornadoField("Расположение точки на растре", typeof(Point2DIO))]
        public Point2D Picture { get; set; }

        ///<summary>
        ///Расположения точек на разных растрах
        ///</summary>
        [ThornadoField("Расположения точек на разных растрах", typeof(Point2DIO), TypeIOModifier.InNullableArray)]
        public Point2D[] Pictures { get; set; }

        ///<summary>
        ///Идентификатор точки
        ///</summary>
        [ThornadoField("Идентификатор точки", typeof(IntIO))]
        public int TrackID { get; set; }

        ///<summary>
        ///Реальное расположение точки в пространстве
        ///</summary>
        [ThornadoField("Реальное расположение точки в пространстве", typeof(Point3DIO))]
        public Point3D Real { get; set; }

        ///<summary>
        ///Цвет точки
        ///</summary>
        [ThornadoField("цвет точки", typeof(ColorIO))]
        public Color Color { get; set; }

        public Vertex() {
            Color = Color.White;
        }
    }

}

namespace RoboCoP.Plus.Common {
    using AIRLab.Thornado;

    using AIRLab.Thornado.IOs;


    ///<summary>
    ///Сгенерированно системой кодегенерации Thornado
    ///</summary>
    public partial class Edge {


        ///<summary>
        ///
        ///</summary>
        [ThornadoField("", typeof(IntIO))]
        internal int Point1Index;

        ///<summary>
        ///
        ///</summary>
        [ThornadoField("", typeof(IntIO))]
        internal int Point2Index;

        ///<summary>
        ///Первая точка, входящая в ребро. Должна быть либо null, либо объектом из коллекции Vertices
        ///</summary>
        public Vertex Point1 { get; set; }

        ///<summary>
        ///вторая точка, входящая в ребро. Должна быть либо null, либо объектом из коллекции Vertices
        ///</summary>
        public Vertex Point2 { get; set; }


        ///<summary>
        ///Индекс ребра в списке
        ///</summary>
        [ThornadoField("Индекс ребра в списке", typeof(IntIO))]
        public int Idx { get; set; }

    }

}

namespace RoboCoP.Plus.Common {
    using System.Collections.Generic;
    using System.Drawing;
    using AIRLab.Thornado;

    using AIRLab.Thornado.IOs;
    using AIRLab.Mathematics;


    ///<summary>
    ///Сгенерированно системой кодегенерации Thornado
    ///</summary>
    public partial class Body {

        ///<summary>
        ///Создает Body
        ///</summary>
        public Body() { }

        ///<summary>
        ///
        ///</summary>
        [ThornadoField("", typeof(IntIO), TypeIOModifier.InNullableArray)]
        public int[] PointsIndices { get; set; }

        ///<summary>
        ///
        ///</summary>
        [ThornadoField("", typeof(IntIO), TypeIOModifier.InNullableArray)]
        public int[] EdgesIndices { get; set; }

        ///<summary>
        ///
        ///</summary>
        List<Vertex> __vertices = new List<Vertex>();
        ///<summary>
        ///
        ///</summary>
        public List<Vertex> Vertices { get { return __vertices; } }
        ///<summary>
        ///
        ///</summary>
        List<Edge> __edges = new List<Edge>();
        ///<summary>
        ///
        ///</summary>
        public List<Edge> Edges { get { return __edges; } }
        ///<summary>
        ///
        ///</summary>
        [ThornadoField("", typeof(Frame3DIO))]
        public Frame3D Real { get; set; }

        ///<summary>
        ///
        ///</summary>
        [ThornadoField("", typeof(Frame2DIO))]
        public Frame2D Picture { get; set; }

        ///<summary>
        ///
        ///</summary>
        [ThornadoField("", typeof(RectangleFIO))]
        public RectangleF PictureBoundingBox { get; set; }

        ///<summary>
        ///
        ///</summary>
        [ThornadoField("", typeof(IntIO))]
        public int Class { get; set; }

    }

}

namespace RoboCoP.Plus.Common {
    using AIRLab.Thornado;
    using AIRLab.Thornado.IOs;
    using AIRLab.Mathematics;



    ///<summary>
    ///Сгенерированно системой кодегенерации Thornado
    ///</summary>
    public partial class Camera {

        ///<summary>
        ///Создает Camera
        ///</summary>
        public Camera() { }


        ///<summary>
        ///
        ///</summary>
        [ThornadoField("", typeof(Frame3DIO))]
        public Frame3D Real { get; set; }
    }

}

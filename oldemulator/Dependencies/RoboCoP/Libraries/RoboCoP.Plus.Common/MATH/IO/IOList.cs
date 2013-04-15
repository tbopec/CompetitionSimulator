using DCIMAP.Thornado.IOs;

namespace DCIMAP.Mathematics {
    public class MathIO {
        ///<summary>Предопределенный экземпляр класса GradusIO</summary>
        public static readonly GradusIO Gradus = new GradusIO();
        ///<summary>Предопределенный экземпляр класса RadianIO</summary>
        public static readonly RadianIO Radian = new RadianIO();
        ///<summary>Предопределенный экземпляр класса Point2DIO</summary>
        public static readonly Point2DIO Point2D = new Point2DIO();
        ///<summary>Предопределенный экземпляр класса Point3DIO</summary>
        public static readonly Point3DIO Point3D = new Point3DIO();
        ///<summary>Предопределенный экземпляр класса Line2DIO</summary>
        public static readonly Line2DIO Line2D = new Line2DIO();
        ///<summary>Предопределенный экземпляр класса Line3DIO</summary>
        public static readonly Line3DIO Line3D = new Line3DIO();
        ///<summary>Предопределенный экземпляр класса Frame2DIO</summary>
        public static readonly Frame2DIO Frame2D = new Frame2DIO();
        ///<summary>Предопределенный экземпляр класса Frame3DIO</summary>
        public static readonly Frame3DIO Frame3D = new Frame3DIO();

    }
}

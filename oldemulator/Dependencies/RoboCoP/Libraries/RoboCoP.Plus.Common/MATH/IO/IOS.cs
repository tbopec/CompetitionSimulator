namespace DCIMAP.Thornado.IOs {
    using DCIMAP.Mathematics;
    using DCIMAP.Thornado;

    ///<summary>
    ///Обеспечивает вывод типа `Двумерная точка` в формате `X;Y`
    ///</summary>
    public partial class Point2DIO : BasicTypeIO<Point2D> {
        ///<inheritdoc/>
        override public string Description { get { return "Двумерная точка. Формат: X;Y"; } }
        TypeIO[] ios = new TypeIO[] { TypeIO.Double, TypeIO.Double };
        string[] names = new string[] { "X", "Y" };
        ///<inheritdoc/>
        override protected Point2D InternalParse(string str) {
            try {
                var parts = str.Split(';');
                return
    new Point2D(TypeIO.Double.Parse(parts[0]), TypeIO.Double.Parse(parts[1]))
    ;
            } catch {
                var parts = ParseFull(str, names, ios);
                return
    new Point2D(TypeIO.Double.Parse(parts[0]), TypeIO.Double.Parse(parts[1]))
    ;
            }
        }
        ///<inheritdoc/>
        override protected string InternalWrite(Point2D obj) {
            var arr = new object[] { obj.X, obj.Y };
            return WriteShort(arr, ios);
        }
        ///<inheritdoc/>
        override protected Point2D InternalDefault {
            get {
                return new Point2D();
            }
        }
        public readonly Point2DExtendedIO ExtendedFormat = new Point2DExtendedIO();



    }

    ///<summary>
    ///Обеспечивает вывод типа `Двумерная точка` в формате `X=<X> Y=<Y>`
    ///</summary>
    public partial class Point2DExtendedIO : BasicTypeIO<Point2D> {
        ///<inheritdoc/>
        override public string Description { get { return "Двумерная точка. Формат: X=<X> Y=<Y>"; } }
        TypeIO[] ios = new TypeIO[] { TypeIO.Double, TypeIO.Double };
        string[] names = new string[] { "X", "Y" };
        ///<inheritdoc/>
        override protected Point2D InternalParse(string str) {
            try {
                var parts = ParseFull(str, names, ios);
                return
    new Point2D(TypeIO.Double.Parse(parts[0]), TypeIO.Double.Parse(parts[1]))
    ;
            } catch {
                var parts = str.Split(';');
                return
    new Point2D(TypeIO.Double.Parse(parts[0]), TypeIO.Double.Parse(parts[1]))
    ;
            }
        }
        ///<inheritdoc/>
        override protected string InternalWrite(Point2D obj) {
            var arr = new object[] { obj.X, obj.Y };
            return WriteFull(arr, ios, names);
        }
        ///<inheritdoc/>
        override protected Point2D InternalDefault {
            get {
                return new Point2D();
            }
        }
    }
}

namespace DCIMAP.Thornado.IOs {
    using DCIMAP.Mathematics;
    using DCIMAP.Thornado;

    ///<summary>
    ///Обеспечивает вывод типа `Трехмерная точка` в формате `X;Y;Z`
    ///</summary>
    public partial class Point3DIO : BasicTypeIO<Point3D> {
        ///<inheritdoc/>
        override public string Description { get { return "Трехмерная точка. Формат: X;Y;Z"; } }
        TypeIO[] ios = new TypeIO[] { TypeIO.Double, TypeIO.Double, TypeIO.Double };
        string[] names = new string[] { "X", "Y", "Z" };
        ///<inheritdoc/>
        override protected Point3D InternalParse(string str) {
            try {
                var parts = str.Split(';');
                return
    new Point3D(TypeIO.Double.Parse(parts[0]), TypeIO.Double.Parse(parts[1]), TypeIO.Double.Parse(parts[2]))
    ;
            } catch {
                var parts = ParseFull(str, names, ios);
                return
    new Point3D(TypeIO.Double.Parse(parts[0]), TypeIO.Double.Parse(parts[1]), TypeIO.Double.Parse(parts[2]))
    ;
            }
        }
        ///<inheritdoc/>
        override protected string InternalWrite(Point3D obj) {
            var arr = new object[] { obj.X, obj.Y, obj.Z };
            return WriteShort(arr, ios);
        }
        ///<inheritdoc/>
        override protected Point3D InternalDefault {
            get {
                return new Point3D();
            }
        }
        public readonly Point3DExtendedIO ExtendedFormat = new Point3DExtendedIO();



    }

    ///<summary>
    ///Обеспечивает вывод типа `Трехмерная точка` в формате `X=<X> Y=<Y> Z=<Z>`
    ///</summary>
    public partial class Point3DExtendedIO : BasicTypeIO<Point3D> {
        ///<inheritdoc/>
        override public string Description { get { return "Трехмерная точка. Формат: X=<X> Y=<Y> Z=<Z>"; } }
        TypeIO[] ios = new TypeIO[] { TypeIO.Double, TypeIO.Double, TypeIO.Double };
        string[] names = new string[] { "X", "Y", "Z" };
        ///<inheritdoc/>
        override protected Point3D InternalParse(string str) {
            try {
                var parts = ParseFull(str, names, ios);
                return
    new Point3D(TypeIO.Double.Parse(parts[0]), TypeIO.Double.Parse(parts[1]), TypeIO.Double.Parse(parts[2]))
    ;
            } catch {
                var parts = str.Split(';');
                return
    new Point3D(TypeIO.Double.Parse(parts[0]), TypeIO.Double.Parse(parts[1]), TypeIO.Double.Parse(parts[2]))
    ;
            }
        }
        ///<inheritdoc/>
        override protected string InternalWrite(Point3D obj) {
            var arr = new object[] { obj.X, obj.Y, obj.Z };
            return WriteFull(arr, ios, names);
        }
        ///<inheritdoc/>
        override protected Point3D InternalDefault {
            get {
                return new Point3D();
            }
        }
    }
}

namespace DCIMAP.Thornado.IOs {
    using DCIMAP.Mathematics;
    using DCIMAP.Thornado;

    ///<summary>
    ///Обеспечивает вывод типа `Двумерная прямая/отрезок` в формате `Begin.X;Begin.Y;End.X;End.Y`
    ///</summary>
    public partial class Line2DIO : BasicTypeIO<Line2D> {
        ///<inheritdoc/>
        override public string Description { get { return "Двумерная прямая/отрезок. Формат: Begin.X;Begin.Y;End.X;End.Y"; } }
        TypeIO[] ios = new TypeIO[] { TypeIO.Double, TypeIO.Double, TypeIO.Double, TypeIO.Double };
        string[] names = new string[] { "X1", "Y1", "X2", "Y2" };
        ///<inheritdoc/>
        override protected Line2D InternalParse(string str) {
            try {
                var parts = str.Split(';');
                return
    new Line2D(new Point2D(TypeIO.Double.Parse(parts[0]), TypeIO.Double.Parse(parts[1])), new Point2D(TypeIO.Double.Parse(parts[2]), TypeIO.Double.Parse(parts[3])))
    ;
            } catch {
                var parts = ParseFull(str, names, ios);
                return
    new Line2D(new Point2D(TypeIO.Double.Parse(parts[0]), TypeIO.Double.Parse(parts[1])), new Point2D(TypeIO.Double.Parse(parts[2]), TypeIO.Double.Parse(parts[3])))
    ;
            }
        }
        ///<inheritdoc/>
        override protected string InternalWrite(Line2D obj) {
            var arr = new object[] { obj.Begin.X, obj.Begin.Y, obj.End.X, obj.End.Y };
            return WriteShort(arr, ios);
        }
        ///<inheritdoc/>
        override protected Line2D InternalDefault {
            get {
                return new Line2D();
            }
        }
        public readonly Line2DExtendedIO ExtendedFormat = new Line2DExtendedIO();



    }

    ///<summary>
    ///Обеспечивает вывод типа `Двумерная прямая/отрезок` в формате `X1=<X1> Y1=<Y1> X2=<X2> Y2=<Y2>`
    ///</summary>
    public partial class Line2DExtendedIO : BasicTypeIO<Line2D> {
        ///<inheritdoc/>
        override public string Description { get { return "Двумерная прямая/отрезок. Формат: X1=<X1> Y1=<Y1> X2=<X2> Y2=<Y2>"; } }
        TypeIO[] ios = new TypeIO[] { TypeIO.Double, TypeIO.Double, TypeIO.Double, TypeIO.Double };
        string[] names = new string[] { "X1", "Y1", "X2", "Y2" };
        ///<inheritdoc/>
        override protected Line2D InternalParse(string str) {
            try {
                var parts = ParseFull(str, names, ios);
                return
    new Line2D(new Point2D(TypeIO.Double.Parse(parts[0]), TypeIO.Double.Parse(parts[1])), new Point2D(TypeIO.Double.Parse(parts[2]), TypeIO.Double.Parse(parts[3])))
    ;
            } catch {
                var parts = str.Split(';');
                return
    new Line2D(new Point2D(TypeIO.Double.Parse(parts[0]), TypeIO.Double.Parse(parts[1])), new Point2D(TypeIO.Double.Parse(parts[2]), TypeIO.Double.Parse(parts[3])))
    ;
            }
        }
        ///<inheritdoc/>
        override protected string InternalWrite(Line2D obj) {
            var arr = new object[] { obj.Begin.X, obj.Begin.Y, obj.End.X, obj.End.Y };
            return WriteFull(arr, ios, names);
        }
        ///<inheritdoc/>
        override protected Line2D InternalDefault {
            get {
                return new Line2D();
            }
        }
    }
}

namespace DCIMAP.Thornado.IOs {
    using DCIMAP.Mathematics;
    using DCIMAP.Thornado;

    ///<summary>
    ///Обеспечивает вывод типа `Трехмерная прямая/отрезок` в формате `Begin.X;Begin.Y;Begin.Z;End.X;End.Y;End.Z`
    ///</summary>
    public partial class Line3DIO : BasicTypeIO<Line3D> {
        ///<inheritdoc/>
        override public string Description { get { return "Трехмерная прямая/отрезок. Формат: Begin.X;Begin.Y;Begin.Z;End.X;End.Y;End.Z"; } }
        TypeIO[] ios = new TypeIO[] { TypeIO.Double, TypeIO.Double, TypeIO.Double, TypeIO.Double, TypeIO.Double, TypeIO.Double };
        string[] names = new string[] { "X1", "Y1", "Z1", "X2", "Y2", "Z2" };
        ///<inheritdoc/>
        override protected Line3D InternalParse(string str) {
            try {
                var parts = str.Split(';');
                return
    new Line3D(new Point3D(TypeIO.Double.Parse(parts[0]), TypeIO.Double.Parse(parts[1]), TypeIO.Double.Parse(parts[2])), new Point3D(TypeIO.Double.Parse(parts[3]), TypeIO.Double.Parse(parts[4]), TypeIO.Double.Parse(parts[5])))
    ;
            } catch {
                var parts = ParseFull(str, names, ios);
                return
    new Line3D(new Point3D(TypeIO.Double.Parse(parts[0]), TypeIO.Double.Parse(parts[1]), TypeIO.Double.Parse(parts[2])), new Point3D(TypeIO.Double.Parse(parts[3]), TypeIO.Double.Parse(parts[4]), TypeIO.Double.Parse(parts[5])))
    ;
            }
        }
        ///<inheritdoc/>
        override protected string InternalWrite(Line3D obj) {
            var arr = new object[] { obj.Begin.X, obj.Begin.Y, obj.Begin.Z, obj.End.X, obj.End.Y, obj.End.Z };
            return WriteShort(arr, ios);
        }
        ///<inheritdoc/>
        override protected Line3D InternalDefault {
            get {
                return new Line3D();
            }
        }
        public readonly Line3DExtendedIO ExtendedFormat = new Line3DExtendedIO();



    }

    ///<summary>
    ///Обеспечивает вывод типа `Трехмерная прямая/отрезок` в формате `X1=<X1> Y1=<Y1> Z1=<Z1> X2=<X2> Y2=<Y2> Z2=<Z2>`
    ///</summary>
    public partial class Line3DExtendedIO : BasicTypeIO<Line3D> {
        ///<inheritdoc/>
        override public string Description { get { return "Трехмерная прямая/отрезок. Формат: X1=<X1> Y1=<Y1> Z1=<Z1> X2=<X2> Y2=<Y2> Z2=<Z2>"; } }
        TypeIO[] ios = new TypeIO[] { TypeIO.Double, TypeIO.Double, TypeIO.Double, TypeIO.Double, TypeIO.Double, TypeIO.Double };
        string[] names = new string[] { "X1", "Y1", "Z1", "X2", "Y2", "Z2" };
        ///<inheritdoc/>
        override protected Line3D InternalParse(string str) {
            try {
                var parts = ParseFull(str, names, ios);
                return
    new Line3D(new Point3D(TypeIO.Double.Parse(parts[0]), TypeIO.Double.Parse(parts[1]), TypeIO.Double.Parse(parts[2])), new Point3D(TypeIO.Double.Parse(parts[3]), TypeIO.Double.Parse(parts[4]), TypeIO.Double.Parse(parts[5])))
    ;
            } catch {
                var parts = str.Split(';');
                return
    new Line3D(new Point3D(TypeIO.Double.Parse(parts[0]), TypeIO.Double.Parse(parts[1]), TypeIO.Double.Parse(parts[2])), new Point3D(TypeIO.Double.Parse(parts[3]), TypeIO.Double.Parse(parts[4]), TypeIO.Double.Parse(parts[5])))
    ;
            }
        }
        ///<inheritdoc/>
        override protected string InternalWrite(Line3D obj) {
            var arr = new object[] { obj.Begin.X, obj.Begin.Y, obj.Begin.Z, obj.End.X, obj.End.Y, obj.End.Z };
            return WriteFull(arr, ios, names);
        }
        ///<inheritdoc/>
        override protected Line3D InternalDefault {
            get {
                return new Line3D();
            }
        }
    }
}

namespace DCIMAP.Thornado.IOs {
    using DCIMAP.Mathematics;
    using DCIMAP.Thornado;

    ///<summary>
    ///Обеспечивает вывод типа `Двумерный фрейм` в формате `X;Y;Angle`
    ///</summary>
    public partial class Frame2DIO : BasicTypeIO<Frame2D> {
        ///<inheritdoc/>
        override public string Description { get { return "Двумерный фрейм. Формат: X;Y;Angle"; } }
        TypeIO[] ios = new TypeIO[] { TypeIO.Double, TypeIO.Double, MathIO.Gradus };
        string[] names = new string[] { "X", "Y", "A" };
        ///<inheritdoc/>
        override protected Frame2D InternalParse(string str) {
            try {
                var parts = str.Split(';');
                return
    new Frame2D(TypeIO.Double.Parse(parts[0]), TypeIO.Double.Parse(parts[1]), MathIO.Gradus.Parse(parts[2]))
    ;
            } catch {
                var parts = ParseFull(str, names, ios);
                return
    new Frame2D(TypeIO.Double.Parse(parts[0]), TypeIO.Double.Parse(parts[1]), MathIO.Gradus.Parse(parts[2]))
    ;
            }
        }
        ///<inheritdoc/>
        override protected string InternalWrite(Frame2D obj) {
            var arr = new object[] { obj.X, obj.Y, obj.Angle };
            return WriteShort(arr, ios);
        }
        ///<inheritdoc/>
        override protected Frame2D InternalDefault {
            get {
                return new Frame2D();
            }
        }
        public readonly Frame2DExtendedIO ExtendedFormat = new Frame2DExtendedIO();



    }

    ///<summary>
    ///Обеспечивает вывод типа `Двумерный фрейм` в формате `X=<X> Y=<Y> A=<A>`
    ///</summary>
    public partial class Frame2DExtendedIO : BasicTypeIO<Frame2D> {
        ///<inheritdoc/>
        override public string Description { get { return "Двумерный фрейм. Формат: X=<X> Y=<Y> A=<A>"; } }
        TypeIO[] ios = new TypeIO[] { TypeIO.Double, TypeIO.Double, MathIO.Gradus };
        string[] names = new string[] { "X", "Y", "A" };
        ///<inheritdoc/>
        override protected Frame2D InternalParse(string str) {
            try {
                var parts = ParseFull(str, names, ios);
                return
    new Frame2D(TypeIO.Double.Parse(parts[0]), TypeIO.Double.Parse(parts[1]), MathIO.Gradus.Parse(parts[2]))
    ;
            } catch {
                var parts = str.Split(';');
                return
    new Frame2D(TypeIO.Double.Parse(parts[0]), TypeIO.Double.Parse(parts[1]), MathIO.Gradus.Parse(parts[2]))
    ;
            }
        }
        ///<inheritdoc/>
        override protected string InternalWrite(Frame2D obj) {
            var arr = new object[] { obj.X, obj.Y, obj.Angle };
            return WriteFull(arr, ios, names);
        }
        ///<inheritdoc/>
        override protected Frame2D InternalDefault {
            get {
                return new Frame2D();
            }
        }
    }
}

namespace DCIMAP.Thornado.IOs {
    using DCIMAP.Mathematics;
    using DCIMAP.Thornado;

    ///<summary>
    ///Обеспечивает вывод типа `Трехмерный фрейм` в формате `X;Y;Z;Pitch;Yaw;Roll`
    ///</summary>
    public partial class Frame3DIO : BasicTypeIO<Frame3D> {
        ///<inheritdoc/>
        override public string Description { get { return "Трехмерный фрейм. Формат: X;Y;Z;Pitch;Yaw;Roll"; } }
        TypeIO[] ios = new TypeIO[] { TypeIO.Double, TypeIO.Double, TypeIO.Double, MathIO.Gradus, MathIO.Gradus, MathIO.Gradus };
        string[] names = new string[] { "X", "Y", "Z", "Pitch", "Yaw", "Roll" };
        ///<inheritdoc/>
        override protected Frame3D InternalParse(string str) {
            try {
                var parts = str.Split(';');
                return
    new Frame3D(TypeIO.Double.Parse(parts[0]), TypeIO.Double.Parse(parts[1]), TypeIO.Double.Parse(parts[2]), MathIO.Gradus.Parse(parts[3]), MathIO.Gradus.Parse(parts[4]), MathIO.Gradus.Parse(parts[5]))
    ;
            } catch {
                var parts = ParseFull(str, names, ios);
                return
    new Frame3D(TypeIO.Double.Parse(parts[0]), TypeIO.Double.Parse(parts[1]), TypeIO.Double.Parse(parts[2]), MathIO.Gradus.Parse(parts[3]), MathIO.Gradus.Parse(parts[4]), MathIO.Gradus.Parse(parts[5]))
    ;
            }
        }
        ///<inheritdoc/>
        override protected string InternalWrite(Frame3D obj) {
            var arr = new object[] { obj.X, obj.Y, obj.Z, obj.Pitch, obj.Yaw, obj.Roll };
            return WriteShort(arr, ios);
        }
        ///<inheritdoc/>
        override protected Frame3D InternalDefault {
            get {
                return new Frame3D();
            }
        }
        public readonly Frame3DExtendedIO ExtendedFormat = new Frame3DExtendedIO();



    }

    ///<summary>
    ///Обеспечивает вывод типа `Трехмерный фрейм` в формате `X=<X> Y=<Y> Z=<Z> Pitch=<Pitch> Yaw=<Yaw> Roll=<Roll>`
    ///</summary>
    public partial class Frame3DExtendedIO : BasicTypeIO<Frame3D> {
        ///<inheritdoc/>
        override public string Description { get { return "Трехмерный фрейм. Формат: X=<X> Y=<Y> Z=<Z> Pitch=<Pitch> Yaw=<Yaw> Roll=<Roll>"; } }
        TypeIO[] ios = new TypeIO[] { TypeIO.Double, TypeIO.Double, TypeIO.Double, MathIO.Gradus, MathIO.Gradus, MathIO.Gradus };
        string[] names = new string[] { "X", "Y", "Z", "Pitch", "Yaw", "Roll" };
        ///<inheritdoc/>
        override protected Frame3D InternalParse(string str) {
            try {
                var parts = ParseFull(str, names, ios);
                return
    new Frame3D(TypeIO.Double.Parse(parts[0]), TypeIO.Double.Parse(parts[1]), TypeIO.Double.Parse(parts[2]), MathIO.Gradus.Parse(parts[3]), MathIO.Gradus.Parse(parts[4]), MathIO.Gradus.Parse(parts[5]))
    ;
            } catch {
                var parts = str.Split(';');
                return
    new Frame3D(TypeIO.Double.Parse(parts[0]), TypeIO.Double.Parse(parts[1]), TypeIO.Double.Parse(parts[2]), MathIO.Gradus.Parse(parts[3]), MathIO.Gradus.Parse(parts[4]), MathIO.Gradus.Parse(parts[5]))
    ;
            }
        }
        ///<inheritdoc/>
        override protected string InternalWrite(Frame3D obj) {
            var arr = new object[] { obj.X, obj.Y, obj.Z, obj.Pitch, obj.Yaw, obj.Roll };
            return WriteFull(arr, ios, names);
        }
        ///<inheritdoc/>
        override protected Frame3D InternalDefault {
            get {
                return new Frame3D();
            }
        }
    }
}
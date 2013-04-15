using DCIMAP.Thornado;
using System;

namespace DCIMAP.Mathematics {
    public abstract class AngleIO : ValueTypeIO<Angle> {
        private bool gradusIsDefault;
        private int precision;
        protected AngleIO(bool gradusIsDefault, int precision) {
            this.gradusIsDefault = gradusIsDefault;
            this.precision = precision;
        }

        protected override string InternalWrite(Angle obj) {
            return
                (precision < 0 ? obj.Grad.ToString() : obj.Grad.ToString("F" + precision))
                +
                (gradusIsDefault ? "G" : "R");
        }

        protected override Angle InternalParse(string str) {
            if(str.Length == 0) throw new Exception("");
            char c = str[str.Length - 1];
            var str1 = str.Substring(0, str.Length - 1);
            if(c == 'G' || c == 'g')
                return Angle.FromGrad(TypeIO.Double.Parse(str1));
            if(c == 'R' || c == 'r')
                return Angle.FromRad(TypeIO.Double.Parse(str1));
            if(gradusIsDefault)
                return Angle.FromGrad(TypeIO.Double.Parse(str));
            else
                return Angle.FromRad(TypeIO.Double.Parse(str));
        }

        protected override Angle InternalDefault {
            get { return new Angle(); }
        }
    }

    public class GradusIO : AngleIO {
        public override string Description {
            get { return "Градусы"; }
        }
        public GradusIO()
            : base(true, -1) {
        }
        public GradusIO(int prec)
            : base(true, prec) {
        }
        public GradusIO Precision(int prec) {
            return new GradusIO(prec);
        }
    }

    public class RadianIO : AngleIO {
        public override string Description {
            get { return "Радиус"; }
        }
        public RadianIO()
            : base(false, -1) {
        }
        public RadianIO(int prec)
            : base(false, prec) {
        }
        public RadianIO Precision(int prec) {
            return new RadianIO(prec);
        }
    }
}

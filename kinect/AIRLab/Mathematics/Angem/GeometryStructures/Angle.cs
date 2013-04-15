using System;

namespace AIRLab.Mathematics {
    /// <summary>
    /// This class represents angle, which have radian and gradus forms. This class is immtable.
    /// </summary>
    [Serializable]
    public struct Angle {
        /// <summary>
        /// inner representation is radian
        /// </summary>
        readonly double radian;
        /// <summary>
        /// Gets gradii representation of angle
        /// </summary>
        public double Grad { get { return radian * 180 / Math.PI; } }
        /// <summary>
        /// Gets radian representation of angle
        /// </summary>
        public double Radian { get { return radian; } }
        /// <summary>
        /// Make the angle inside [-180,180] interval
        /// </summary>
        public Angle Simplify180() {
            var r = radian;
            while(r < -Math.PI) r += 2 * Math.PI;
            while(r > Math.PI) r -= 2 * Math.PI; ;
            return new Angle(r);
        }

        /// <summary>
        /// Make the angle inside [0,360] interval
        /// </summary>
        public Angle Simplify360() {
            var r = radian;
            while(r < 0) r += 2 * Math.PI;
            while(r > 2 * Math.PI) r -= 2 * Math.PI; ;
            return new Angle(r);
        }

        /// <summary>
        /// Creates angle from radian
        /// </summary>
        private Angle(double rad) {
            this.radian = rad;
        }


        /// <summary>
        /// Creates angle from gradii representation
        /// </summary>
        public static Angle FromGrad(double grad) {
            return new Angle(grad * Math.PI / 180);
        }
        /// <summary>
        /// Creates angle from radian representation
        /// </summary>
        public static Angle FromRad(double rad) {
            return new Angle(rad);
        }

        public static Angle Zero{ get { return new Angle(0); } }
        public static Angle Pi { get { return new Angle(Math.PI); } }
        public static Angle HalfPi { get { return new Angle(Math.PI/2); } }

        ///<inheritdoc/>
        public override string ToString() {
            return Grad + "G";
        }

        public static Angle Abs(Angle angle)
        {
            return Angle.FromGrad(Math.Abs(angle.Grad));
        }

        #region Arithmetic
        public static Angle operator +(Angle v1, Angle v2) {
            return new Angle(v1.radian + v2.radian);
        }

        public static Angle operator -(Angle v1, Angle v2) {
            return new Angle(v1.radian - v2.radian);
        }
        public static Angle operator -(Angle v) {
            return new Angle(-v.radian);
        }

        public static Angle operator *(Angle v1, double v2) {
            return new Angle(v1.radian * v2);
        }
        public static Angle operator *(double v2, Angle v1) {
            return v1 * v2;
        }
        public static Angle operator /(Angle v1, double v2) {
            return new Angle(v1.radian / v2);
        }


        public static double operator /(Angle v1, Angle v2) {
            return v1.Grad / v2.Grad;
        }

        /// <summary>
        /// Adds specified gradii to angle
        /// </summary>
        public Angle AddGrad(double grad) {
            return new Angle(radian + grad * Math.PI / 180);
        }

        /// <summary>
        /// Adds specified radians to angle
        /// </summary>
        public Angle AddRad(double rad) {
            return new Angle(radian + rad);
        }

        #endregion
        #region Comparison
        public static bool operator <(Angle a, Angle b) {
            return a.radian < b.radian;
        }
        public static bool operator >(Angle a, Angle b) {
            return a.radian > b.radian;
        }
        public static bool operator <=(Angle a, Angle b) {
            return a.radian <= b.radian;
        }
        public static bool operator >=(Angle a, Angle b) {
            return a.radian >= b.radian;
        }
        #endregion
    }
}

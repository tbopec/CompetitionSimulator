using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using AIRLab.Mathematics;

namespace AIRLab.Thornado
{
    partial class TypeResolver
    {
        static void InitImmutableConstructors()
        {
        
            #region DateTime

            ImmutableResolver<DateTime>.AddConstructor(
                new Func<int, int, int, int, int, int, int, DateTime>(
                    (Year, Month, Day, Hour, Minute, Second, Millisecond) => new DateTime(Year, Month, Day, Hour, Minute, Second, Millisecond)));

            ImmutableResolver<DateTime>.AddConstructor(
                            new Func<int, int, int, int, int, int, DateTime>(
                                (Year, Month, Day, Hour, Minute, Second) => new DateTime(Year, Month, Day, Hour, Minute, Second)));
            ImmutableResolver<DateTime>.AddConstructor(
                        new Func<int, int, int, int, int, DateTime>(
                            (Year, Month, Day, Hour, Minute) => new DateTime(Year, Month, Day, Hour, Minute, 0)));
            ImmutableResolver<DateTime>.AddConstructor(
                    new Func<int, int, int, int, DateTime>(
                        (Year, Month, Day, Hour) => new DateTime(Year, Month, Day, Hour, 0, 0)));
            ImmutableResolver<DateTime>.AddConstructor(
                    new Func<int, int, int, DateTime>(
                        (Year, Month, Day) => new DateTime(Year, Month, Day)));
            /*добить time*/
            #endregion

            ImmutableResolver<Color>.AddConstructor(new Func<int, int, int, Color>((R, G, B) => Color.FromArgb(R, G, B)));

            ImmutableResolver<Frame2D>.AddConstructor(new Func<double, double, Angle, Frame2D>((X, Y, Angle) => new Frame2D(X, Y, Angle)));

            ImmutableResolver<Frame3D>.AddConstructor(new Func<double, double, double, Angle, Angle, Angle, Frame3D>((X, Y, Z, Pitch, Yaw, Roll) => new Frame3D(X, Y, Z, Pitch, Yaw, Roll)));

            ImmutableResolver<Rectangle>.AddConstructor(new Func<int, int, int, int, Rectangle>((X, Y, Width, Height) => new Rectangle(X, Y, Width, Height)));
        }

    }
}

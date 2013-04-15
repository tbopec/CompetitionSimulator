using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using DCIMAP.Thornado;

namespace RoboCoP.Plus.Common.Data
{
    public class ClassifiedObjects
    {
        [ThornadoList("Objects")]
        public List<ClassifiedObject> Objects = new List<ClassifiedObject>();

        public override string ToString()
        {
            return string.Concat(Objects.Select(a => a.ToString()+";\n"));
        }
    }

    public class ClassifiedObject
    {
        [ThornadoList("Point of object")]
        public List<Point> Points = new List<Point>();

        [ThornadoField("Class")]
        public int Class;
        /// <summary>
        /// Create classified object from rectangle
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        public static ClassifiedObject FromRect(Rectangle rect, int @class)
        {
            return new ClassifiedObject() { Class = @class, Points = new List<Point>() { new Point(rect.X, rect.Y), new Point(rect.X + rect.Width, rect.Y), new Point(rect.X + rect.Width, rect.Y + rect.Height), new Point(rect.X, rect.Y + rect.Height) } };
        }

        public override string ToString()
        {
            return Class.ToString() + " [" + string.Concat(Points.Select(a => "(" + a.X + "; " + a.Y + ")")) + "]";
        }
    }
}

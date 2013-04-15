using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIRLab.Thornado
{
    public class IO
    {
        public static StringIO String { get { return new StringIO(); } }
        public static XML XML { get { return new XML(); } }
        public static INI INI { get { return new INI(); } }
    }
}

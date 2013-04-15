using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIRLab.Thornado
{
    public static class TensorIntermediateType<T>
    {
        public class Base
        {
            public Array Array;
            public int[] args;
            public int ptr;
        }

        public class Ordinal1 : Base { }
        public class Ordinal2 : Base { }
        public class Ordinal3 : Base { }
        public class Ordinal4 : Base { }
        public class Ordinal5 : Base { }
        public class Ordinal6 : Base { }
        public class Ordinal7 : Base { }
        public class Ordinal8 : Base { }
        public class Ordinal9 : Base { }


        static List<Type> types = new List<Type>
        {
            typeof(Ordinal1),
            typeof(Ordinal2),
            typeof(Ordinal3),
            typeof(Ordinal4),
            typeof(Ordinal5),
            typeof(Ordinal6),
            typeof(Ordinal7),
            typeof(Ordinal8),
            typeof(Ordinal9)
        };

        public static int GetOrdinalIndex(Type t)
        {
            var res = types.IndexOf(t);
            if (res == -1) return -1;
            return res + 1;
        }

        public static Type GetOrdinalType(int i)
        {
            return types[i - 1];
        }

        public static bool IsOrdinal(Type t)
        {
            return types.Contains(t);
        }

        static Type[] tp = new Type[0];

        public static Base CreateOrdinal(int i)
        {
            return (Base)GetOrdinalType(i).GetConstructor(tp).Invoke(null);
        }

        public static Base NextOrdinal(Base ord, int index)
        {
            var ord1 = CreateOrdinal(GetOrdinalIndex(ord.GetType())-1);
            ord1.Array = ord.Array;
            ord1.args = ord.args;
            ord1.ptr = ord.ptr;
            ord1.args[ord1.ptr] = index;
            ord1.ptr++;
            return ord1;
        }
    }
}
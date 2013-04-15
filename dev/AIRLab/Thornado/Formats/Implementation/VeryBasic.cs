//TODO:тут надо получше обработать некоторые форматы, потому что decimal, например, посыпется на точке/запятой
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using AIRLab.Mathematics;

namespace AIRLab.Thornado.TypeFormats
{
    public class TypeTypeFormat : BasicTypeFormat<Type>
    {
        public TypeTypeFormat() : base(a => a.FullName, a => Type.GetType(a), "Тип") { }
    }

    [PrimaryFormat]
    public class IntFormat : BasicTypeFormat<int>
    {
        public IntFormat() : base(z => z.ToString(), z => int.Parse(z), "целое число") { }
    }

    [PrimaryFormat]
    public class LongFormat : BasicTypeFormat<long>
    {
        public LongFormat() : base(z => z.ToString(), z => long.Parse(z), "целое число") { }
    }

    [PrimaryFormat]
    public class FloatFormat : BasicTypeFormat<float>
    {
        public FloatFormat() : base(z => z.ToString(), z => float.Parse(z),"число c плавающей точкой") { }
    }

    [PrimaryFormat]
    public class DecimalFormat : BasicTypeFormat<decimal>
    {
        public DecimalFormat() : base(z => z.ToString(), z => decimal.Parse(z), "десятичное число") { }
    }

    [PrimaryFormat]
    public class CharFormat : BasicTypeFormat<char>
    {
        public CharFormat() : base(z => z.ToString(), z => z[0], "символ") { }
    }

    [PrimaryFormat]
    public class SByteFormat : BasicTypeFormat<sbyte>
    {
        public SByteFormat() : base(z => z.ToString(), z => sbyte.Parse(z),  "signed byte") { }
    }

    [PrimaryFormat]
    public class ShortFormat : BasicTypeFormat<short>
    {
        public ShortFormat() : base(z => z.ToString(), z => short.Parse(z), "малое целое число") { }
    }

    [PrimaryFormat]
    public class BoolFormat : BasicTypeFormat<bool>
    {
        public BoolFormat() : base(z => z.ToString(), z => bool.Parse(z), "логическое значение") { }
    }

    [PrimaryFormat]
    public class ByteFormat : BasicTypeFormat<byte>
    {
        public ByteFormat() : base(z => z.ToString(), z => byte.Parse(z), "байт") { }
    }

    [PrimaryFormat]
    public class StringFormat : BasicTypeFormat<string>
    {
        public StringFormat() : base(z => z, z => z, "строка") { }
    }

    [PrimaryFormat]
    public class EnumFormat<T> : BasicTypeFormat<T>
        where T : struct
    {
    	public EnumFormat()
            : base(z => z.ToString(), z => (T)Enum.Parse(typeof(T), z), "Перечисление " + typeof(T).Name)
        {
            T t = default(T);
            if (!(t is Enum))
                throw new Exception("EnumFormat must be created only with enumeration");
        }
    }

    [PrimaryFormat]
    public class DoubleFormat : BasicTypeFormat<double>
    {
        static double MyParse(string str)
        {
            var form = new System.Globalization.NumberFormatInfo();
            form.NumberDecimalSeparator = ",";
            str = str.Replace('.', ',');
            return double.Parse(str, form);
        }

        public DoubleFormat() : base(z => z.ToString(), MyParse, "число двойной точности") { }
    }
}
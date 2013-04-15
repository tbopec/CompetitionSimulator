using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIRLab.Thornado
{
    public class Constructor
    {
        public Delegate Func { get; protected set; }
        public Dictionary<string, Type> Parameters { get; protected set; }
    }

    public class Constructor<T> : Constructor
    {
        public Constructor(Delegate del)
        {
            Parameters = new Dictionary<string, Type>();
            if (del.Method.ReturnType != typeof(T) && !del.Method.ReturnType.IsSubclassOf(typeof(T)))
                throw new Exception("Функция должна возвращать объект типа " + typeof(T).Name);
            foreach (var mi in del.Method.GetParameters())
                Parameters.Add(mi.Name, mi.ParameterType);
            Func = del;
        }
    }
}

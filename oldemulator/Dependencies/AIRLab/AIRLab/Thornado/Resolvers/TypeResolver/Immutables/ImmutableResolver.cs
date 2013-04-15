using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIRLab.Thornado
{
    public class ImmutableResolver<T> : TypeResolver
    {
        static List<Constructor<T>> Constructors = new List<Constructor<T>>();

        public static void AddConstructor(Delegate delegete)
        {
            if (!TypeResolver.knownImmutables.Contains(typeof(T)))
                TypeResolver.knownImmutables.Add(typeof(T));
            Constructors.Add(new Constructor<T>(delegete));
        }



        public override void SetElement(object obj,string sub, object value)
        {
            throw new Exception("Setter for " + sub + " field of " + Type.Name + " type is inaccessible");
          }

        public override IEnumerable<string> GetSubaddresses(object obj)
        {
            return Constructors.SelectMany(a => a.Parameters).Select(a => a.Key).Distinct();
        }

        public override object GetElement(object obj, string sub)
        {
            //убрал выборку из АД-а, потому что так ИМХО делать некорректно. Но парсинг Immutable в принципе надо менять =(
            var fs = typeof(T).GetProperty(sub);
            if (fs != null)
                return fs.GetValue(obj, new object[0]);
            else
            {
                var fd = typeof(T).GetField(sub);
                return fd.GetValue(obj);
            }

        }

        public override IEnumerable<string> GetDefinedSubaddresses()
        {
            return Constructors.SelectMany(a => a.Parameters).Select(a => a.Key).Distinct();
        }

        public override Type GetDefinedType(string sub)
        {
            var fs = typeof(T).GetProperty(sub);
            if (fs != null)
                return fs.PropertyType;
            else
            {
                var fd = typeof(T).GetField(sub);
                return fd.FieldType;
            }

        }

        internal override object ParseMoldInner(TextMold mold, LogicErrorList errors, ContextDependedParser cdp)
        {
            foreach (var cntr in Constructors)
            {
                if (cntr.Parameters.Count != mold.Nodes.Count) continue;
                if (cntr.Parameters.Where(a => !mold.Nodes.ContainsKey(a.Key)).Count() != 0) continue;
                var lst = new object[cntr.Parameters.Count];
                int ptr = 0;
                foreach (var a in cntr.Parameters)
                {
                    object obj = null;
                    if (mold.Nodes[a.Key].Value != null && !mold.Nodes[a.Key].IsNull)
                        obj = TypeResolver.GetResolver(a.Value).Format.ParseObject(mold.Nodes[a.Key].Value);
                    lst[ptr++] = obj;
                }
                return (T)cntr.Func.DynamicInvoke(lst);
            }
            errors.Add(LogicErrorLevel.Error, "Для Immutable " + typeof(T) + " по адресу " + mold.Address + " указано неверное количество аргументов (" + mold.Nodes.Select(a => a.Key + ", ") + ")");
            return null;
        }
    }
}

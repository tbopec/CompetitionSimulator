using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace AIRLab.Thornado
{
    public class ListResolver<T> : TypeResolver
    {
        public override IEnumerable<string> GetSubaddresses(object obj)
        {
            for (int i = 0; i < ((IList)obj).Count; i++) yield return i.ToString();
        }

        public override object GetElement(object obj, string sub)
        {
            return ((IList<T>)obj)[Formats.Int.Parse(sub)];
        }

        public override void SetElement(object obj, string sub, object value)
        {
            ((IList<T>)obj)[Formats.Int.Parse(sub)] = (T)value;
        }

        public override IEnumerable<string> GetDefinedSubaddresses()
        {
            yield return "*";
        }

        public override Type GetDefinedType(string sub)
        {
            return typeof(T);
        }

        public override object Touch(object obj, FieldAddress tail)
        {
            int k = Formats.Int.Parse(tail.FirstElement);
            var l = (IList<T>)obj;
            while (l.Count <= k)
                l.Add(default(T));
            return null;
        }

        internal override object ParseMoldInner(TextMold mold, LogicErrorList errors, ContextDependedParser cdp)
        {
            var list = (IList<T>)CreateDefaultObject();
            foreach (var e in mold.Nodes.Keys)
            {
                int ind = 0;
                try
                {
                    ind = int.Parse(e);
                }
                catch
                {
                    errors.Add(LogicErrorLevel.Error, "Неверный индекс " + e + " в листе");
                    continue;
                }
                var obj = ParseMold(typeof(T), mold.Nodes[e], errors,cdp);
                if (obj!=null && !(obj is T))
                {
                    errors.Add(LogicErrorLevel.Error, "Неверный тип элемента " + obj.GetType() + " в листе по адресу " + e);
                    continue;
                }

                while (list.Count <= ind)
                    list.Add(default(T));
                list[ind] = (T)obj;
                
            }
            return list;
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace AIRLab.Thornado
{
    public class DictionaryResolver<K,T> : TypeResolver
    {
        static TypeFormat<K> KeyFormat;

        static DictionaryResolver()
        {
            KeyFormat = (TypeFormat<K>)TypeFormat.GetDefaultFormat(typeof(K));
        }

        public override IEnumerable<string> GetSubaddresses(object obj)
        {
            return ((IDictionary<K, T>)obj).Keys.Select(z => KeyFormat.Write(z));
        }

        public override object GetElement(object obj, string sub)
        {
            return ((IDictionary<K, T>)obj)[KeyFormat.Parse(sub)];
        }

        public override void SetElement(object obj, string sub, object value)
        {
            ((IDictionary<K, T>)obj)[KeyFormat.Parse(sub)] = (T)value;
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
            var d = ((IDictionary<K, T>)obj);
            var arg = KeyFormat.Parse(tail.FirstElement);
            if (!d.ContainsKey(arg))
                d[arg] = default(T);
            return null;
        }

        internal override object ParseMoldInner(TextMold mold, LogicErrorList errors, ContextDependedParser cdp)
        {
            var dict = (IDictionary)CreateDefaultObject();
            foreach (var e in mold.Nodes.Keys)
            {
                K key = default(K);
                try
                {
                    key = KeyFormat.Parse(e);
                }
                catch (Exception)
                {
                    errors.Add(LogicErrorLevel.Error, "Неверный ключ словаря " + e);
                    continue;
                }

                var obj = ParseMold(typeof(T), mold.Nodes[e], errors, cdp);
                if (obj!=null && !(obj is T))
                {
                    errors.Add(LogicErrorLevel.Error, " Неверный тип объекта " + obj.GetType() + " в словаре по адресу " + e);
                    continue;
                }
                dict[key] = (T)obj;
            }
            return dict;
        }
    }
}

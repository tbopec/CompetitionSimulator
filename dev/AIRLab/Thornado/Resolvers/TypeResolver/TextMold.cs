using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIRLab.Thornado
{
    public class TextMold
    {
        public FieldAddress Address;
        public int Line;
        public Type CustomType; //не null, если отличается от заявленного
        public Type ActualType;
        public bool IsNull;
        public string Value;
        public Dictionary<string, TextMold> Nodes = new Dictionary<string, TextMold>();
    }

    public delegate void ContextDependedParser(TextMold mold, Type type, LogicErrorList list);

    partial class TypeResolver
    {
        public TextMold WriteToMold(object obj)
        {
            var mold = new TextMold();
            if (obj == null) mold.IsNull = true;
            else
            {
                mold.IsNull = false;
                mold.ActualType = obj.GetType();
                if (Format != null)
                    mold.Value = Format.WriteObject(obj);
                else
                    foreach (var e in GetSubaddresses(obj))
                    {
                        var ch = GetElement(obj, e);
                        TextMold chmold = null;
                        if (ch == null)
                        {
                            chmold = new TextMold();
                            chmold.IsNull = true;
                        }
                        else
                        {
                            var chres = TypeResolver.GetResolver(ch.GetType());
                            chmold = chres.WriteToMold(ch);
                            if (chres.Type != GetDefinedType(e))
                                chmold.CustomType = chres.Type;
                        }
                        mold.Nodes[e] = chmold;
                    }
            }
            return mold;
        }

        protected object ParseMold(Type expectedType, TextMold mold, LogicErrorList list, ContextDependedParser cdp)
        {
            if (mold.IsNull) return null;

            if (mold.CustomType != null) expectedType = mold.CustomType;
            
            if (cdp!=null)
                cdp(mold, expectedType, list);

            return GetResolver(expectedType).ParseMoldInner(mold,list,cdp);

        }

        internal abstract object ParseMoldInner(TextMold mold, LogicErrorList errors, ContextDependedParser cdp);

        public static object ParseMold(TextMold mold, Type rootType, LogicErrorList list, ContextDependedParser cdp)
        {
            return GetResolver(rootType).ParseMold(rootType, mold, list, cdp);
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIRLab.Thornado
{
    public class FieldResolver<T> : TypeResolver
    {
        public override IEnumerable<string> GetSubaddresses(object obj)
        {
            yield break;
        }

        public override object GetElement(object obj, string sub)
        {
            throw new Exception("There is no children in Fields");
        }

        public override void SetElement(object obj, string sub, object value)
        {
            throw new Exception("There is no children in Fields");
        }

        public override object Touch(object obj, FieldAddress tail)
        {
            throw new Exception("There is no children in Fields");
        }

        public override IEnumerable<string> GetDefinedSubaddresses()
        {
            yield break;
        }

        public override Type GetDefinedType(string sub)
        {
            throw new Exception("There is no subaddresses in Fields");
        }

        internal override object ParseMoldInner(TextMold mold, LogicErrorList errors, ContextDependedParser cdp)
        {
            try
            {
                return Format.ParseObject(mold.Value);
            }
            catch(Exception e)
            {
                errors.Add(LogicErrorLevel.Error, "Ошибка парсинга значения " + mold.Value + " в типе " + Type + ": " + e.Message);
                return CreateDefaultObject();
            }
        }
    }
}

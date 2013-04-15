using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Linq.Expressions;

namespace AIRLab.Thornado
{
    public class ClassResolver<T> : TypeResolver
    {

        #region Статика
        /// <summary>
        /// Здесь храняться поля/свойства со значениями Значимых атрибутов
        /// </summary>
        static Dictionary<string, FieldScan> fields;
        /// <summary>
        /// здесь хранятся скомпилированные геттеры
        /// </summary>
        static Dictionary<string, Func<object, object>> getters;
       
        /// <summary>
        /// Заполнение информации о полях/свойствах класса
        /// </summary>
        static ClassResolver()
        {
            List<MemberInfo> mems = new List<MemberInfo>();
            Type check = typeof(T);
            while (true)
            {
                if (check == typeof(object)) break;
                mems.AddRange(check.GetMembers(BindingFlags.GetField | BindingFlags.GetProperty | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance));
                check = check.BaseType;
                
            }
            fields = new Dictionary<string, FieldScan>();
            foreach (var e in mems.Where(z => z is PropertyInfo || z is FieldInfo).Select(z => new FieldScan(z)).Where(a =>a.FieldName != null))
                fields[e.FieldName] = e;

          
            getters = new Dictionary<string, Func<object, object>>();
            foreach(var e  in fields)
            {
                var objParam=Expression.Parameter(typeof(object),"obj");
                var cast=Expression.TypeAs(objParam,typeof(T));
                Expression res=null;
                if (e.Value.Field!=null) res=Expression.Field(cast,e.Value.Field);
                else res=Expression.Property(cast,e.Value.Property);
                res=Expression.TypeAs(res,typeof(object));
                getters[e.Key]=Expression.Lambda<Func<object,object>>(res,objParam).Compile();
            }
        }
        #endregion

        public override void SetElement(object obj, string sub, object value)
        {
            fields[sub].Set(obj, value);
        }

        public override IEnumerable<string> GetDefinedSubaddresses()
        {
            return fields.Keys;
        }

        public override Type GetDefinedType(string sub)
        {
            return fields[sub].Type;
        }

        public override IEnumerable<object> GetAttributedData(string sub)
        {
            var a=fields[sub];
                foreach (var info in a.BindingInfoFromAttributes)
                {
                    var o = info.GenerateAssotiatedData(Type);
                    foreach (var ob in o)
                    {
                        yield return ob;
                    }
                }
        }

        public override IEnumerable<string> GetSubaddresses(object obj)
        {
            return fields.Keys;
        }

        public override object GetElement(object obj, string sub)
        {
            return getters[sub].Invoke(obj);
        }


        internal override object ParseMoldInner(TextMold mold, LogicErrorList errors, ContextDependedParser cdp)
        {
            var r = CreateDefaultObject();
            foreach (var e in mold.Nodes.Keys)
            {
                if (!fields.ContainsKey(e))
                {
                    errors.Add(LogicErrorLevel.Error, "Класс " + typeof(T) + " не содержит поля " + e);
                    continue;
                }
                SetElementAndProcessErrors(r, e, ParseMold(fields[e].Type, mold.Nodes[e], errors, cdp), errors);
            }
            return r;
        }
    }
}

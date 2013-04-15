using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace AIRLab.Thornado
{
    class FieldScan 
    {
        /// <summary>
        /// Информация об атрибутах типа Binding
        /// </summary>
        public IEnumerable<IADProducingAttribute> BindingInfoFromAttributes =  new List<IADProducingAttribute>();

        #region Доступ к данным
        /// <summary>
        /// Устанавливает, что это поле или свойство, заполняет атрибуты
        /// </summary>
        public FieldScan(MemberInfo info)
            : base()
        {
            var attrs = info.GetCustomAttributes(typeof(IADProducingAttribute), false) as IADProducingAttribute[];
            if (attrs.Count() == 0)
                return;
            Member = info;
            Type t = null;
            // Если свойство
            if (info is PropertyInfo)
            {
                Property = info as PropertyInfo;
                t = Property.PropertyType;
            }
            // Если поле
            if (info is FieldInfo)
            {
                Field = info as FieldInfo;
                t = Field.FieldType;
            }
            Type = t;

            if (attrs != null && attrs.Length > 0)
                BindingInfoFromAttributes = attrs.ToList();
            FieldName = info.Name;
         
            //выбрали все IThornadoAttribute из атрибутов, и у каждого из них попросили BiundInfo, и заполнить boundInfo
        }

        public static object[] emp = new object[0];

        /// <summary>
        /// Имя поля
        /// </summary>
        public string FieldName { get; private set; }
        /// <summary>
        /// ReflexiveInfo about this field/property
        /// </summary>
        public MemberInfo Member { get; private set; }
        /// <summary>
        /// ReflexiveInfo about this property (if it is)
        /// </summary>
        public PropertyInfo Property { get; private set; }
        /// <summary>
        /// ReflexiveInfo about this field (if it is)
        /// </summary>
        public FieldInfo Field { get; private set; }

        public Type Type { get; private set; }
        #endregion
        #region Sugar for value
        /// <summary>
        /// Получить объект из данной ...
        /// </summary>
        public object Get(object obj)
        {
            if (Property != null)
                return Property.GetValue(obj, emp);
            else
                return Field.GetValue(obj);
        }
        /// <summary>
        /// Установить значение данной ...
        /// </summary>
        public void Set(object obj, object value)
        {
            if (Property != null)
                Property.SetValue(obj, value, emp);
            else
                Field.SetValue(obj, value);

        }
        #endregion
    }
}

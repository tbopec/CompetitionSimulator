using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIRLab.Thornado
{
    public abstract partial class TypeResolver
    {
        public TypeFormat Format { get; private set; }
        public Categories Category { get; private set; }
        public Type Type { get; private set; }

        
        public abstract IEnumerable<string> GetSubaddresses(object obj);
        public abstract object GetElement(object obj, string sub);
        public abstract void SetElement(object obj, string sub, object value);
        public virtual object Touch(object obj, FieldAddress tail) { return null; }

        public virtual IEnumerable<object> GetAttributedData(string sub) { yield break; }
        
   
        public abstract IEnumerable<string> GetDefinedSubaddresses();

        public abstract Type GetDefinedType(string sub);

        
       
        public object CreateDefaultObject()
        {
            if (Type.IsValueType)
               return Activator.CreateInstance(Type);
            if (Type.IsArray)
                return Array.CreateInstance(Type.GetElementType(), new int[Type.GetArrayRank()]);
            var tp=Type.GetConstructor(Type.EmptyTypes);
            if (tp!=null) return tp.Invoke(new object[] { });
            if (Type == typeof(string)) return "";
            throw new Exception("Cannot create instance of Type " + Type.Name);
        }

        public void SetElementAndProcessErrors(object obj, string sub, object value, LogicErrorList errors)
        {
            try
            {
                SetElement(obj, sub, value);
            }
            catch (Exception e)
            {
                //развернуть ошибку TargetInvokation, тип всегда error, если листа нет, то выбросить внутренний эксепшн
                // Если не установлен список ошибок в ThornadoDispatch
                // то добавленную туда ошибку, вероятно, никто не ждет
                // поэтому громко сообщаем о ней пробросом эксепшена наружу
                if (errors == null)
                {
                    throw new Exception(e.InnerException.Message);
                }

                // Приводим полученное исключение к типу ThornadoException,
                // если это возможно
                try
                {
                    var thornadoException = (ThornadoException)e.InnerException;
                    errors.Add(
                        thornadoException.Level,
                        thornadoException.Message);
                }
                catch
                {
                    if (e.InnerException != null)
                        // Иначе полученное исключение общего вида
                        errors.Add(
                            LogicErrorLevel.Error,
                            e.InnerException.Message);
                    else
                        errors.Add(
                            LogicErrorLevel.Error,
                            e.Message);
                }
            }
        }

        #region Обходы дерева объектов
        public static void TreeRunDown(FieldAddress address, object obj, Action<FieldAddress, object> rootFirstAction, Action<FieldAddress, object> leafFirstAction)
        {
            if (obj==null) return;
            if (rootFirstAction != null)
                rootFirstAction(address, obj);
            var res = TypeResolver.GetResolver(obj.GetType());
            foreach (var e in res.GetSubaddresses(obj))
                TreeRunDown(address.Child(e),res.GetElement(obj, e), rootFirstAction, leafFirstAction);
            if (leafFirstAction != null)
                leafFirstAction(address, obj);
        }

        public static void CheckConsistancy(object obj, LogicErrorList list)
        {
            TreeRunDown(new FieldAddress(), obj, (a,o) => { if (o is ICheckable) (o as ICheckable).CheckSelfConsistancy(list); }, null);
        }
        #endregion


    }
}

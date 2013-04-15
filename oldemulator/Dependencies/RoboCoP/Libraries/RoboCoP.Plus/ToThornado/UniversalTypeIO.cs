using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using DCIMAP.Thornado;

namespace DCIMAP.Thornado
{
    public class TypeIOFieldAttribute : Attribute
    {
        internal Type typeIO;
        public TypeIOFieldAttribute(Type typeIO)
        {
            this.typeIO = typeIO;
        }
    }


    public class UniversalTypeIO<T> : BasicTypeIO<T>
        where T: new()
    {
        static string[] names;
        static TypeIO[] ios;
        static MemberInfo[] fields;


        
        static UniversalTypeIO()
        {
            List<MemberInfo> mems = new List<MemberInfo>();
            Type check=typeof(T);
            while (true)
            {
                mems.AddRange(check.GetMembers(BindingFlags.GetField | BindingFlags.GetProperty | BindingFlags.NonPublic | BindingFlags.Public| BindingFlags.DeclaredOnly| BindingFlags.Instance));
                check = check.BaseType;
                if (check == typeof(object)) break;
            }
            for (int i=0;i<mems.Count;i++)
                if (!(mems[i] is PropertyInfo) && !(mems[i] is System.Reflection.FieldInfo))
                {
                    mems.RemoveAt(i);
                    i--;
                }


            var names1 = new List<string>();
            var ios1 = new List<TypeIO>();
        var fields1=new List<MemberInfo>();


            foreach (var p in mems) 
                foreach (var a in p.GetCustomAttributes(typeof(TypeIOFieldAttribute), true))
                {
                    names1.Add(p.Name);
                    fields1.Add(p);
                    var io = ((TypeIOFieldAttribute)a).typeIO;
                    var ex=(TypeIO)io.GetConstructor(Type.EmptyTypes).Invoke(new object[0]);
                    ios1.Add(ex);
                }
            names = names1.ToArray();
            fields = fields1.ToArray();
            ios = ios1.ToArray();
        }

    
        protected override T InternalDefault
        {
            get { return new T(); }
        }

        void MakeT(T obj, string[] parsed)
        {
            for (int i = 0; i < names.Length; i++)
            {
                var val=ios[i].ParseObject(parsed[i]);
                if (fields[i] is PropertyInfo)
                    ((PropertyInfo)fields[i]).SetValue(obj, val, null);
                else
                    ((System.Reflection.FieldInfo)fields[i]).SetValue(obj, val);
            }
        }

        protected virtual char Separator { get { return ';'; } }

        protected override T InternalParse(string str)
        {
            T obj = new T();
            try
            {
                var parts = str.Split(Separator);
                MakeT(obj, parts);
                return obj;
                
            }
            catch
            {
                var parts = ParseFull(str, names, ios);
                MakeT(obj, parts);
                return obj;
            }
        }

        protected override string InternalWrite(T obj)
        {
            var res="";
            for (int i = 0; i < names.Length; i++)
            {
                if (i != 0) res += " ";
                if (fields[i] is PropertyInfo)
                    res += ios[i].WriteObject(((PropertyInfo)fields[i]).GetValue(obj, null));
                else
                    res += ios[i].WriteObject(((System.Reflection.FieldInfo)fields[i]).GetValue(obj));
            }
            return res;
        }

        public override string Description
        {
            get { throw new NotImplementedException(); }
        }
    }
}

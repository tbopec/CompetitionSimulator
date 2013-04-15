using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIRLab.Thornado
{
    public abstract partial class TypeResolver
    {
        static Dictionary<Type, TypeResolver> cache;
        internal static List<Type>  knownImmutables;
        static TypeResolver()
        {
            cache = new Dictionary<Type, TypeResolver>();
            knownImmutables = new List<Type>();
            InitImmutableConstructors();
        }

        static object[] emptyObjects = new object[0];

        static TypeResolver CreateResolverObject(Type orType, Categories cat, Type genericType, params Type[] genericArgs)
        {
            var type = genericType.MakeGenericType(genericArgs);
            var ctor = type.GetConstructor(Type.EmptyTypes);
            var obj = ctor.Invoke(emptyObjects);
            var res=(TypeResolver)obj;
            res.Category = cat;
            res.Type = orType;
            return res;
        }

        static TypeResolver CreateResolver(Type t)
        {
            if (knownImmutables.Contains(t)) return CreateResolverObject(t,Categories.Immutable,typeof(ImmutableResolver<>), t);

            var format = TypeFormat.GetDefaultFormat(t);
            if (format != null)
            {
                var res=CreateResolverObject(t,Categories.Field,typeof(FieldResolver<>), t);
                res.Format = format;
                return res;
            }

            if (t.IsArray) return CreateResolverObject(t, Categories.Tensor, typeof(TensorResolver<,>), t.GetElementType(), t);

            if (t.FullName.Contains("TensorIntermediateType`1+Ordinal"))
                return CreateResolverObject(t, Categories.Tensor, typeof(TensorResolver<,>), t.GetGenericArguments()[0], t);
            
    
            var inter = t.GetInterfaces();
            foreach (var i in inter)
            {
                if (i.GetGenericArguments().Count() > 0)
                {
                    if (i.GetGenericTypeDefinition() == typeof(IList<>))
                    {
                        var elementType = i.GetGenericArguments()[0];
                        return CreateResolverObject(t, Categories.List, typeof(ListResolver<>), elementType);
                    }

                    if (i.GetGenericTypeDefinition() == typeof(IDictionary<,>))
                    {
                        var keyType = i.GetGenericArguments()[0];
                        var elementType = i.GetGenericArguments()[1];
                        if (TypeFormat.GetDefaultFormat(keyType) == null)
                            throw new Exception("To work with dictionaries, its first generic argument must have format");
                        return CreateResolverObject(t, Categories.Dictionary, typeof(DictionaryResolver<,>), keyType, elementType);
                    }
                }

            }

            if (t.GetConstructor(Type.EmptyTypes) != null || t.IsInterface) //почему t.IsInterface?? такое вообще разве может быть??
                return CreateResolverObject(t, Categories.Node, typeof(ClassResolver<>), t);

            throw new Exception("Не удалось определить категорию типа " + t.FullName + ". Скорее всего, следует добавить в него пустой конструктор");
        }

        static public TypeResolver GetResolver(Type index)
        {
            lock (cache)
            {
                if (!cache.ContainsKey(index))
                    cache[index] = CreateResolver(index);
                return cache[index];
            }
        }

        public static object CreateDefaultObject(Type t) { return GetResolver(t).CreateDefaultObject(); }
    }
}

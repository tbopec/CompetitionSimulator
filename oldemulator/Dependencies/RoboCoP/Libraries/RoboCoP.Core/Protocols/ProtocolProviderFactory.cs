using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using RoboCoP.Internal;

namespace RoboCoP.Protocols
{
    /// <summary>
    /// Static class for obtaining <see cref="ProtocolProvider{T}"/> for specific types of <see cref="INetworkAddress"/> (provided by theirs instance)
    /// and calling methods <see cref="ProtocolProvider{T}.GetConnectionFunc"/> and <see cref="ProtocolProvider{T}.GetConnectionManager"/>
    /// on it.
    /// </summary>
    public static class ProtocolProviderFactory
    {
        private static readonly ConcurrentDictionary<Type, ProtocolProviderFactoryCache> protocolProviders =
            new ConcurrentDictionary<Type, ProtocolProviderFactoryCache>();

        /// <summary>
        /// Call <see cref="ProtocolProvider{T}.GetConnectionFunc"/> on <see cref="ProtocolProvider{T}"/> suitable for the <paramref name="address"/>.
        /// </summary>
        public static Func<IConnection> GetConnectionFunc(INetworkAddress address)
        {
            if(address == null)
                throw new ArgumentNullException("address");
            return GetInstance(address.GetType()).getConnectionFunc(address);
        }

        /// <summary>
        /// Call <see cref="ProtocolProvider{T}.GetConnectionManager"/> on <see cref="ProtocolProvider{T}"/> suitable for the <paramref name="address"/>.
        /// </summary>
        public static IConnectionsManager GetConnectionManager(INetworkAddress address)
        {
            if(address == null)
                throw new ArgumentNullException("address");
            return GetInstance(address.GetType()).getConnectionManager(address);
        }

        private static IEnumerable<Type> BaseTypes(this Type type)
        {
            var result = new List<Type>();
            while(type != null) {
                result.Add(type);
                type = type.BaseType;
            }
            return result;
        }

        private static ProtocolProviderFactoryCache GetInstance(Type addressType)
        {
            Type baseAddressType = addressType
                .BaseTypes()
                .Where(typeof(INetworkAddress).IsAssignableFrom)
                .Last();

            return protocolProviders.GetOrAdd(
                addressType,
                delegate {
                    Type baseProtocolProviderType = typeof(ProtocolProvider<>).MakeGenericType(baseAddressType);
                    Type protocolProviderType = Assembly.GetExecutingAssembly()
                        .GetTypes()
                        .Where(type => type.IsSubclassOf(baseProtocolProviderType))
                        .Single();
                    ConstantExpression xProtocolProvider = Expression.Constant(Activator.CreateInstance(protocolProviderType));

                    ParameterExpression xParameter = Expression.Parameter(typeof(INetworkAddress), "address");
                    return new ProtocolProviderFactoryCache
                           {
                               getConnectionFunc = Expression.Lambda<Func<INetworkAddress, Func<IConnection>>>(
                                   Expression.Call(
                                       xProtocolProvider,
                                       protocolProviderType.GetMethod("GetConnectionFunc"),
                                       Expression.Convert(xParameter, baseAddressType)),
                                   xParameter).Compile(),
                               getConnectionManager = Expression.Lambda<Func<INetworkAddress, IConnectionsManager>>(
                                   Expression.Call(
                                       xProtocolProvider,
                                       protocolProviderType.GetMethod("GetConnectionManager"),
                                       Expression.Convert(xParameter, baseAddressType)),
                                   xParameter).Compile(),
                           };
                });
        }

        #region Nested type: ProtocolProviderFactoryCache

        private class ProtocolProviderFactoryCache
        {
            public Func<INetworkAddress, Func<IConnection>> getConnectionFunc;
            public Func<INetworkAddress, IConnectionsManager> getConnectionManager;
        }

        #endregion
    }
}
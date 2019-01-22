using Trepub.Common.Facade;
using System;
using System.Collections.Concurrent;

namespace Trepub.Common
{
    public class BusinessFacade
    {
        protected static ConcurrentDictionary<Type, object> bsos = new ConcurrentDictionary<Type, object>();
        protected static TxInterceptor txInterceptor = new TxInterceptor();

        public static T GetBSO<T>() where T : class
        {
            return (T)GetBSO(typeof(T));
        }
        
        public static object GetBSO(Type type)
        {
            object result;
            if (!bsos.ContainsKey(type))
            {
                result = new Castle.DynamicProxy.ProxyGenerator().CreateClassProxy(type, txInterceptor);
                bsos.TryAdd(type, result);

            }
            else
            {
                result = bsos[type];
            }
            return result;

        }

    }
}

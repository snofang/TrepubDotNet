using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Trepub.IFS.Database
{
    public class AppRepositoryContext
    {
        protected static ConcurrentDictionary<Type, object> repos = new ConcurrentDictionary<Type, object>();

        public static AppRepository<TEntity> GetRepo<TEntity>() where TEntity : class
        {
            AppRepository<TEntity> result = null;
            if (!repos.ContainsKey(typeof(TEntity)))
            {
                result = new AppRepository<TEntity>();
                repos.TryAdd(typeof(TEntity), result);
            }
            else
            {
                result = (AppRepository<TEntity>)repos[typeof(TEntity)];
            }
            return result;
        }

    }
}

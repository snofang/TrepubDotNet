using MicroOrm.Dapper.Repositories.SqlGenerator;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Trepub.IFS.Database
{
    public class SqlGeneratorContext
    {

        protected static ConcurrentDictionary<Type, object> sqlGenerators = new ConcurrentDictionary<Type, object>();

        public static AppSqlGenerator<TEntity> GetSqlGenerator<TEntity>() where TEntity : class
        {
            AppSqlGenerator<TEntity> result = null;
            if (!sqlGenerators.ContainsKey(typeof(TEntity)))
            {
                result = new AppSqlGenerator<TEntity>(); 
                sqlGenerators.TryAdd(typeof(TEntity), result);
            }
            else
            {
                result = (AppSqlGenerator<TEntity>)sqlGenerators[typeof(TEntity)];
            }
            return result;
        }
    }
}

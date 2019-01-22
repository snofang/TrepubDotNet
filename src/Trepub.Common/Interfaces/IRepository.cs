using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Trepub.Common.Interfaces
{
    public interface IRepository<T> where T : class
    {
        T GetSingle(Expression<Func<T, bool>> criteria);
        T GetSingle(ISpecification<T> spec);
        List<T> List();
        List<T> List(ISpecification<T> spec);
        List<T> List(Expression<Func<T, bool>> criteria);
        T Add(T entity);
        void Update(T entity);
        void Delete(T entity);
        //IEnumerable<dynamic> Query(string sql);
    }
}

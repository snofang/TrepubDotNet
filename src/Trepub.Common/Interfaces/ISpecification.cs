
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Trepub.Common.Interfaces
{
    public interface ISpecification<T> where T : class
    {
        Expression<Func<T, bool>> Criteria { get; set; }
        //List<Expression<Func<T, object>>> Includes { get; }
        //void AddInclude(Expression<Func<T, object>> includeExpression);
        int Page { get; set; }
        int PageSize { get; set; }
    }
}

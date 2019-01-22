using Trepub.Common.Entities;
using Trepub.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq.Expressions;

namespace Trepub.Common.Specifications
{
    public class BaseSpecification<T> : ISpecification<T> where T: class
    {
        public BaseSpecification()
        {
            this.PageSize = 10;
            this.Page = 0;
        }

        //public List<Expression<Func<T, object>>> Includes { get; } = new List<Expression<Func<T, object>>>();

        private Expression<Func<T, bool>> criteria = null;// i => true;
        public Expression<Func<T, bool>> Criteria
        {
            get
            {
                return this.criteria;
            }
            set
            {
                this.criteria = value;
            }
        }

        //public void AddInclude(Expression<Func<T, object>> includeExpression)
        //{
        //    Includes.Add(includeExpression);
        //}

        public int Page { get; set; }
        public int PageSize { get; set; }


    }
}

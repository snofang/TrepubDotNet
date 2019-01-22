using MicroOrm.Dapper.Repositories;
using Trepub.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq.Expressions;
using MicroOrm.Dapper.Repositories.SqlGenerator;
using System.Data;
using System.Linq;
using Dapper;
using System.Collections;
using Trepub.Common.Exceptions;
using Trepub.Common;
using Trepub.Common.Utils;
using Trepub.IFS.Database;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Trepub.Common.Facade;

namespace Trepub.IFS.Database
{
    public class AppRepository<T> : IRepository<T> where T : class
    {

        protected ILogger<AppRepository<T>> logger;

        public AppRepository(){
            this.logger = FacadeProvider.IfsFacade.GetLogger<AppRepository<T>>();
        }

        public virtual T Add(T entity)
        {
            var q = SqlGeneratorContext.GetSqlGenerator<T>().GetInsert(entity);
            var c = AppDbContext.Instance.Connection;
            if(c.Execute(q.GetSql(), q.Param, 
                AppDbContext.Instance.Transaction) <= 0)
            {
                throw new Exception("failed to insert entity");
            }
            return entity;
        }

        public virtual void Delete(T entity)
        {
            var q = SqlGeneratorContext.GetSqlGenerator<T>().GetDelete(entity);
            AppDbContext.Instance.Connection.Execute(q.GetSql(), q.Param,
                AppDbContext.Instance.Transaction);
        }

        public virtual T GetSingle(Expression<Func<T, bool>> criteria)
        {
            var q = SqlGeneratorContext.GetSqlGenerator<T>().GetSelectFirst(criteria);
            return AppDbContext.Instance.Connection.QueryFirstOrDefault<T>(q.GetSql(), q.Param, 
                AppDbContext.Instance.Transaction);
        }

        public virtual T GetSingle(ISpecification<T> spec)
        {
            return GetSingle(spec.Criteria);
        }

        public virtual List<T> List()
        {
            return List((Expression<Func<T, bool>>)null);
        }

        public virtual List<T> List(ISpecification<T> spec)
        {
            var q = SqlGeneratorContext.GetSqlGenerator<T>().GetSelectAll(spec.Criteria);
            return  AppDbContext.Instance.Connection.Query<T>(q.GetSql(), q.Param, 
                AppDbContext.Instance.Transaction).ToList();
        }

        public virtual List<T> List(Expression<Func<T, bool>> criteria)
        {
            var q = SqlGeneratorContext.GetSqlGenerator<T>().GetSelectAll(criteria);
            return AppDbContext.Instance.Connection.Query<T>(q.GetSql(), q.Param, 
                AppDbContext.Instance.Transaction).ToList();
        }

        public virtual void Update(T entity)
        {
            var q = SqlGeneratorContext.GetSqlGenerator<T>().GetUpdate(entity);
            if (AppDbContext.Instance.Connection.Execute(q.GetSql(), entity,
                AppDbContext.Instance.Transaction) <= 0)
            {

                q = SqlGeneratorContext.GetSqlGenerator<T>().GetSelectByIds(entity);
                var e = AppDbContext.Instance.Connection.QueryFirstOrDefault<T>(q.GetSql(), entity, AppDbContext.Instance.Transaction);
                if(e == null)
                {
                    StringBuilder sb = new StringBuilder("Entity updated by another user. Existing Entity:\n");
                    sb.Append(ObjectDumper.Dump(entity));
                    throw new AppException(ErrorConstants.DAL_ENTITY_UPDATEDBYOTHER, sb.ToString());
                }
            }
        }

    }
}

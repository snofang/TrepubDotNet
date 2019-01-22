using MicroOrm.Dapper.Repositories.SqlGenerator;
using System;
using System.Collections.Generic;
using System.Text;

namespace Trepub.IFS.Database
{
    public interface IAppSqlGenerator<TEntity> where TEntity : class 
    {
        SqlQuery GetSelectByIds(TEntity entity);

    }
}

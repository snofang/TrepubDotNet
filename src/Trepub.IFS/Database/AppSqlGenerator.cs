using MicroOrm.Dapper.Repositories.SqlGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Trepub.IFS.Database
{
    public class AppSqlGenerator<TEntity> : SqlGenerator<TEntity>, IAppSqlGenerator<TEntity> where TEntity: class 
    {

        public AppSqlGenerator() : base()
        {
            this.Config = new SqlGeneratorConfig
            {
                SqlConnector = ESqlConnector.MySQL,
                UseQuotationMarks = true
            };
            this.TableName = TableName.ToLower();
        }

        public override SqlQuery GetInsert(TEntity entity)
        {
            var properties = (IsIdentity ? SqlProperties.Where(p => !p.PropertyName.Equals(IdentitySqlProperty.PropertyName, StringComparison.OrdinalIgnoreCase)) : 
                SqlProperties.Where(p =>!p.IgnoreUpdate)).ToList();

            if (HasUpdatedAt)
                UpdatedAtProperty.SetValue(entity, DateTime.UtcNow);

            var query = new SqlQuery(entity);

            query.SqlBuilder.Append(
                "INSERT INTO " + TableName
                + " (" + string.Join(", ", properties.Select(p => p.ColumnName)) + ")" // columNames
                + " VALUES (" + string.Join(", ", properties.Select(p => "@" + p.PropertyName)) + ")"); // values

            if (IsIdentity)
                switch (Config.SqlConnector)
                {
                    case ESqlConnector.MSSQL:
                        query.SqlBuilder.Append(" SELECT SCOPE_IDENTITY() AS " + IdentitySqlProperty.ColumnName);
                        break;

                    case ESqlConnector.MySQL:
                        query.SqlBuilder.Append("; SELECT CONVERT(LAST_INSERT_ID(), SIGNED INTEGER) AS " + IdentitySqlProperty.ColumnName);
                        break;

                    case ESqlConnector.PostgreSQL:
                        query.SqlBuilder.Append(" RETURNING " + IdentitySqlProperty.ColumnName);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }

            return query;
        }

        public override SqlQuery GetUpdate(TEntity entity)
        {
            var properties = SqlProperties.Where(
                p => !KeySqlProperties.Any(k => k.PropertyName.Equals(p.PropertyName, StringComparison.OrdinalIgnoreCase))).ToArray();
            if (!properties.Any())
                throw new ArgumentException("Can't update without [Key]");

            if (HasUpdatedAt)
                UpdatedAtProperty.SetValue(entity, DateTime.UtcNow);

            var query = new SqlQuery(entity);
            query.SqlBuilder.Append("UPDATE " + TableName + " SET " + string.Join(", ", properties.Where(p=>!p.IgnoreUpdate).Select(p => p.ColumnName + " = @" + p.PropertyName))
                + " WHERE " + string.Join(" AND ", KeySqlProperties.Select(p => p.ColumnName + " = @" + p.PropertyName)));

            return query;
        }

        public SqlQuery GetSelectByIds(TEntity entity)
        {
            var query = new SqlQuery(entity);
            query.SqlBuilder.Append("SELECT * FROM " + TableName 
                + " WHERE " + string.Join(" AND ", KeySqlProperties.Select(p => p.ColumnName + " = @" + p.PropertyName)));
            return query;
        }


    }
}

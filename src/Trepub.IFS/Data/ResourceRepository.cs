using Dapper;
using Trepub.Common.Entities;
using Trepub.Common.EntitiesExt;
using Trepub.Common.Extensions;
using Trepub.Common.Interfaces;
using Trepub.IFS.Database;
using System.Collections.Generic;
using System.Linq;
using Trepub.IFS.Extensions;

namespace Trepub.IFS.Data
{
    public class ResourceRepository : AppRepository<Resource>, IResourceRepository
    {

        public List<ResourceExt> GetResources(ResourceExt resourceExt, int page, int pageSize)
        {
            resourceExt.RoleItemId = resourceExt.RoleItemId;
            resourceExt.Status = resourceExt.Status.ToLikeEncodedParameter();

            string sql = "";
            if (page == -1 && pageSize == -1)
                sql = @"SELECT r.ResourceID as ResourceID, r.Name as Name, r.Status as Status, rRole.RoleItemID as RoleItemID, r.ResourceContent
                            From Resource r inner join (select rir.ResourceID,ri.RoleItemID as RoleItemID,rir.CreationDate from RoleItemResource rir inner join RoleItem ri on rir.RoleItemID = ri.RoleItemID) as rRole
                            on r.ResourceID = rRole.ResourceID #RESOURCESTATUSFILTER# #RESOURCEROLEFILTER# order by rRole.CreationDate";
            else
                sql = @"SELECT r.ResourceID as ResourceID, r.Name as Name, r.Status as Status, rRole.RoleItemID as RoleItemID, r.ResourceContent
                            From Resource r inner join (select rir.ResourceID,ri.RoleItemID as RoleItemID,rir.CreationDate from RoleItemResource rir inner join RoleItem ri on rir.RoleItemID = ri.RoleItemID) as rRole
                            on r.ResourceID = rRole.ResourceID #RESOURCESTATUSFILTER# #RESOURCEROLEFILTER# order by rRole.CreationDate OFFSET @FromPage ROWS FETCH NEXT @PageSize ROWS ONLY";

            //RESOURCESTATUSFILTER
            string str = "";
            if (!string.IsNullOrEmpty(resourceExt.Status))
            {
                sql = sql.Replace("#RESOURCESTATUSFILTER#", "where r.Status like @Status");
                str = "and";
            }
            else
            {
                sql = sql.Replace("#RESOURCESTATUSFILTER#", "");
            }


            //RESOURCEROLEFILTER
            if (resourceExt.RoleItemId != 0)
            {
                if (string.IsNullOrEmpty(str))
                    str = "where";
                sql = sql.Replace("#RESOURCEROLEFILTER#", str + " rRole.RoleItemID = @RoleItemId");
            }
            else
            {
                sql = sql.Replace("#RESOURCEROLEFILTER#", "");
            }

            //page size check
            if (pageSize <= 0 || pageSize > 100)
            {
                pageSize = 20;
            }

            int from = page * pageSize;

            return AppDbContext.Instance.Connection.Query<ResourceExt>(sql, new
            {
                FromPage = from,
                PageSize = pageSize,
                Status = resourceExt.Status,
                RoleItemId = resourceExt.RoleItemId
                //RoleItemName = profileExt.RoleItemName

            }, AppDbContext.Instance.Transaction).ToList();
        }
    }
}

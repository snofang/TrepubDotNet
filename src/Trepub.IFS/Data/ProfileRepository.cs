using Dapper;
using Trepub.Common.Entities;
using Trepub.Common.EntitiesExt;
using Trepub.Common.Extensions;
using Trepub.Common.Interfaces;
using Trepub.IFS.Database;
using System.Collections.Generic;
using System.Linq;
using Trepub.IFS.Extensions;
using Trepub.Common.DataStructures;
using System.Text;

namespace Trepub.IFS.Data
{
    public class ProfileRepository : AppRepository<Profile>, IProfileRepository
    {

        public List<ProfileExt> GetProfiles(ProfileFilter filter, int? page, int? pageSize)
        {
            StringBuilder sb = new StringBuilder("SELECT DISTINCT * FROM profile pf INNER JOIN party pt ON pt.PartyId=pf.PartyId INNER JOIN profilerole pr ON pr.ProfileId=pf.ProfileId ");
            AddWhereClause(filter, sb);
            sb.Append("ORDER BY CreationDate LIMIT @FromPage, @PageSize");
            return AppDbContext.Instance.Connection.Query<ProfileExt>(sb.ToString(), new
            {
                UserId = filter.UserId.ToLikeEncodedParameter(),
                Name = filter.Name.ToLikeEncodedParameter(),
                LastName = filter.LastName.ToLikeEncodedParameter(),
                MobileNumber = filter.MobileNumber.ToLikeEncodedParameter(),
                Status = filter.Status,
                RoleItemId = filter.RoleItemId,
                PageSize = pageSize.ToPageSizeNormalize(),
                FromPage = page.ToPageNormalize() * pageSize.ToPageSizeNormalize()
            }, AppDbContext.Instance.Transaction).ToList();
        }
        public int GetProfilesTotalCount(ProfileFilter filter)
        {
            StringBuilder sb = new StringBuilder("SELECT DISTINCT COUNT(*) FROM profile pf INNER JOIN party pt ON pt.PartyId=pf.PartyId INNER JOIN profilerole pr ON pr.ProfileId=pf.ProfileId ");
            AddWhereClause(filter, sb);
            return AppDbContext.Instance.Connection.ExecuteScalar<int>(sb.ToString(), new
            {
                UserId = filter.UserId.ToLikeEncodedParameter(),
                Name = filter.Name.ToLikeEncodedParameter(),
                LastName = filter.LastName.ToLikeEncodedParameter(),
                MobileNumber = filter.MobileNumber.ToLikeEncodedParameter(),
                Status = filter.Status,
                RoleItemId = filter.RoleItemId,
            }, AppDbContext.Instance.Transaction);
        }

        private void AddWhereClause(ProfileFilter filter, StringBuilder sb)
        {

            //default where clause 
            sb.Append("WHERE 1 ");

            //UserId
            if (!string.IsNullOrEmpty(filter.UserId))
            {
                sb.Append("AND pf.UserId LIKE @UserId ");
            }

            //Name
            if (!string.IsNullOrEmpty(filter.Name))
            {
                sb.Append("AND pt.Name LIKE @Name ");
            }

            //LastName
            if (!string.IsNullOrEmpty(filter.LastName))
            {
                sb.Append("AND pt.LastName LIKE @LastName ");
            }

            //MobileNumber
            if (!string.IsNullOrEmpty(filter.MobileNumber))
            {
                sb.Append("AND pf.MobileNumber LIKE @MobileNumber ");
            }

            //Status
            if (!string.IsNullOrEmpty(filter.Status))
            {
                sb.Append("AND pf.Status=@Status ");
            }

            //RoleItemId
            if (filter.RoleItemId.HasValue)
            {
                sb.Append("AND pr.RoleItemId=@RoleItemId ");
            }
        }

        private const string PROFILE_SELECT_CLAUSE = "SELECT * FROM profile pf INNER JOIN party ";

    }

}

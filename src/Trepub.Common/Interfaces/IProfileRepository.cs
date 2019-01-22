using System;
using System.Collections.Generic;
using System.Text;
using Trepub.Common.DataStructures;
using Trepub.Common.Entities;
using Trepub.Common.EntitiesExt;

namespace Trepub.Common.Interfaces
{
    public interface IProfileRepository : IRepository<Profile>
    {
        List<ProfileExt> GetProfiles(ProfileFilter filter, int? page, int? pageSize);
        int GetProfilesTotalCount(ProfileFilter filter);
    }

}

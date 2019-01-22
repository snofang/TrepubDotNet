using Trepub.Common.Entities;
using Trepub.Common.EntitiesExt;
using System;
using System.Collections.Generic;
using System.Text;

namespace Trepub.Common.Interfaces
{
    public interface IResourceRepository : IRepository<Resource>
    {
        List<ResourceExt> GetResources(ResourceExt resourceExt, int page, int pageSize);
    }
}

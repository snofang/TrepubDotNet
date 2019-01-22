using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trepub.Common.DataStructures;

namespace Trepub.Web.API.ViewModels.Profile
{
    public class ProfileFilterView : ProfileFilter
    {
        public int? Page { get; set; }
        public int? PageSize { get; set; }
    }
}

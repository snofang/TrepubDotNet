using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trepub.Common.DataStructures;

namespace Trepub.Web.API.ViewModels.FileResource
{
    public class FileResourceFilterView : FileResourceFilter
    {
        public int? Page { get; set; }
        public int? PageSize { get; set; }

    }
}

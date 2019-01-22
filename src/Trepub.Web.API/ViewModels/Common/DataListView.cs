using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Trepub.Web.API.ViewModels.Common
{
    public class DataListView<T> where T:class
    {
        public IEnumerable<T> DataList { get; set; }
        public int TotalCount { get; set; }

    }
}

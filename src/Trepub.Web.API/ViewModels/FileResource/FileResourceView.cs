using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Trepub.Web.API.ViewModels.FileResource
{
    public class FileResourceView
    {
        public int FileResourceId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public DateTime? CreationDate { get; set; }
        public decimal? Duration { get; set; }


    }
}

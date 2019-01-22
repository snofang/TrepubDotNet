using System;
using System.Collections.Generic;
using System.Text;

namespace Trepub.IFS.Extensions
{
    public static class QueryUtils
    {
        public static string ToLikeEncodedParameter(this string param)
        {
            if (string.IsNullOrEmpty(param))
            {
                return "%";
            }
            else
            {
                return "%" + param.Replace("[", "[[]").Replace("%", "[%]") + "%";
            }
        }

        public static int ToPageSizeNormalize(this int? pageSize)
        {
            if (pageSize.HasValue && pageSize.Value <= 100 && pageSize.Value > 0)
            {
                return pageSize.Value;
            }
            return 100;
        }

        public static int ToPageNormalize(this int? page)
        {
            if (page.HasValue && page >= 0)
            {
                return page.Value;
            }
            return 0;
        }

    }
}

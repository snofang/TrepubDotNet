using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;
using Trepub.Common.Extensions;

namespace Trepub.Common.Utils
{
    public class ExportUtils
    {
        public static void ListToCsvFile<T>(IEnumerable<T> list, string filePath, bool convertDateTimeToJalali)
        {
            string delimiter = ",";

            Type t = typeof(T);
            PropertyInfo[] properties = t.GetProperties();

            string header = String.Join(delimiter, properties.Select(f => f.Name).ToArray());

            StringBuilder csvdata = new StringBuilder();
            csvdata.AppendLine(header);

            foreach (T rv in list)
            {
                csvdata.AppendLine(ToCsvFields(delimiter, properties, rv, convertDateTimeToJalali));
            }
            System.IO.File.WriteAllText(filePath, csvdata.ToString(), Encoding.UTF8);
        }


        private static string ToCsvFields(string separator, PropertyInfo[] properties, object o, bool convertDateTimeToJalali)
        {
            StringBuilder sb = new StringBuilder();

            foreach (var f in properties)
            {
                if (sb.Length > 0)
                {
                    sb.Append(separator);
                }

                var x = f.GetValue(o);

                if (x != null)
                {
                    if (convertDateTimeToJalali && f.PropertyType.Equals(typeof(DateTime)))
                    {
                        sb.Append(((DateTime)x).ToJalaliDateTime());
                    }
                    else if (f.PropertyType.Equals(typeof(string)))
                    {
                        sb.Append(((string)x).Replace("\n", " ").Replace(",", "&comma"));
                    }
                    else
                    {
                        sb.Append(x.ToString());
                    }
                }
            }
            return sb.ToString();
        }

    }
}

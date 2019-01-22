
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Net.Http;
using Newtonsoft.Json;

/// <summary>
/// Note: this file hase been copied from microsoft sample and have some modifications.
/// </summary>
namespace Trepub.Common.Utils
{
    public class ObjectDumper
    {

        public static string Dump(object element)
        {
            //ObjectDumper dumper = new ObjectDumper(0);
            //dumper.sb = new StringBuilder();
            //dumper.WriteObject(null, element);
            //return dumper.sb.ToString();

            return JsonConvert.SerializeObject(element, Formatting.Indented);

        }
    }
}

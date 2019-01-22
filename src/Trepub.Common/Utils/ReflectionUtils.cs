using System;
using System.Linq;

namespace Trepub.Common.Utils
{
    public class ReflectionUtils
    {
        public static object Instantiate(string className)
        {
            var requiredClass = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                 from type in assembly.GetTypes()
                                 where string.Equals(type.Name, className, StringComparison.OrdinalIgnoreCase)
                                 select type).FirstOrDefault();
            if (requiredClass != null)
            {
                return Activator.CreateInstance(requiredClass);
            }
            return null;
        }

    }
}

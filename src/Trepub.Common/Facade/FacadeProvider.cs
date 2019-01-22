using Trepub.IFS;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Trepub.Common.Utils;

namespace Trepub.Common.Facade
{
    public class FacadeProvider
    {
        protected static IIfsFacade ifsFacade;

        public static IIfsFacade IfsFacade
        {
            get
            {
                if(ifsFacade == null)
                {
                    ifsFacade = (IIfsFacade)ReflectionUtils.Instantiate("IfsFacade");
                }
                return ifsFacade;
            }
        }

    }
}

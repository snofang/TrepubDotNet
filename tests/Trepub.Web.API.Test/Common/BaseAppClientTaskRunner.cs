using System;
using Trepub.Web.API.Test.Common;
using Microsoft.Extensions.CommandLineUtils;
using System.Collections.Generic;

namespace Trepub.Web.API
{
    public abstract class BaseAppClientTaskRunner
    {

        public abstract void RunTask(string baseUrl, List<string> args);

        protected static void Log(string message)
        {
            Console.WriteLine(message);
        }

    }
}

using System;
using Trepub.Web.API.Test.Common;

namespace Trepub.Web.API
{
    public abstract class BaseWebTest
    {

        protected TestClient client;

        public abstract void RunTest();

        protected static void Log(string message)
        {
            Console.WriteLine(message);
        }

    }
}

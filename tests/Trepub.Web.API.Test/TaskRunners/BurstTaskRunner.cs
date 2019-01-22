using Trepub.Web.API.Test.Common;
using Trepub.Web.API.Test.TestClientBSO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Trepub.Web.API.Test.TaskRunners
{
    public class BurstTaskRunner : BaseAppClientTaskRunner
    {
        private string baseUrl;
        public override void RunTask(string baseUrl, List<string> args)
        {
            this.baseUrl = baseUrl;
            int counter = int.Parse(args[0]);
            List<Thread> tList = new List<Thread>();
            for (int i=0; i<counter; i++)
            {
                Thread t = new Thread(LoginTask);
                tList.Add(t);
            }

            foreach(var t in tList)
            {
                t.Start();
            }


        }

        public void LoginTask()
        {
            Log(this.baseUrl);
            var client = new AdminTestClient(this.baseUrl);
            try
            {
                Log($"trying login threadId={Thread.CurrentThread.ManagedThreadId}");
                client.Login();
            }
            catch
            {
                Log($"login failed threadId={Thread.CurrentThread.ManagedThreadId}");
            }

        }
    }
}

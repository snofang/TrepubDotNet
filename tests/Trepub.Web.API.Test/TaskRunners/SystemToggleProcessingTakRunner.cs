using Trepub.Common.Utils;
using Trepub.Web.API.Test.Common;
using Trepub.Web.API.Test.TestClientBSO;
using Trepub.Web.API.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Trepub.Web.API.Test.ConfigTasks
{
    public class SystemToggleProcessingTakRunner : BaseAppClientTaskRunner
    {
        public override void RunTask(string baseUrl, List<string> args)
        {
            var client = new AdminTestClient(baseUrl);
            client.Login();
            Log($"calling toggle system's ProcessingRequestsMode");
            var result = client.Post<SingleValueView>(new SingleValueView() { Value = "someInput"}, "/api/system/toggleProcessingRequestsMode");
            Log($"current status: {result.Value}");
        }

    }
}

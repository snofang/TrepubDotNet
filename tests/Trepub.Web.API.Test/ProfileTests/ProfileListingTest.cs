using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Trepub.Common;
using Trepub.Web.API.Test.Common;
using Trepub.Web.API.Test.TestClientBSO;
using Trepub.Web.API.ViewModels.Profile;

namespace Trepub.Web.API.Test.ProfileTests
{
    public class ProfileListingTest : BaseWebTest
    {
        public override void RunTest()
        {
            this.client = new TestClient();
            var adminClient = AdminTestClient.Instance;
            var pv = this.client.RegisterProfile();
            this.client.Login();

            var plist = adminClient.Profile_List(new ProfileFilterView()
            {
                UserId = this.client.Profile.UserId,
                Status = EntityConstants.PROFILEROLE_STATUS_ACTIVE,
            });
            if(plist.TotalCount != 1 || plist.DataList.Count() != 1)
            {
                throw new Exception("listing for just created profile using profileId and status filter is not working properly");
            }

            //updating profile name
            string name1 = "Name_" + DateTime.Now.Ticks.ToString();
            pv = this.client.Profile_GetMine();
            pv.Name = name1;
            pv = this.client.Profile_UpdateMine(pv);

            plist = adminClient.Profile_List(new ProfileFilterView()
            {
                Name = name1,
            });
            if (plist.TotalCount != 1 || plist.DataList.Count() != 1)
            {
                throw new Exception("listing for just created profile using name filter is not working properly");
            }

            this.client.UnregisterProfile();
            Log("it seems profile listing works properly.");
        }

    }
}

using Trepub.Common;
using Trepub.Common.Exceptions;
using Trepub.Web.API.Test.Common;
using Trepub.Web.API.Test.TestClientBSO;
using Trepub.Web.API.ViewModels;
using System;
using System.Threading;

namespace Trepub.Web.API.Test.Clients.ProfileTests
{
    public class ProfileUpdateTest : BaseWebTest
    {

        public override void RunTest()
        {
            this.client = new TestClient();
            var pv = this.client.RegisterProfile();
            this.client.Login();

            //sample data
            string name = "sampleFirstName";
            string LastName = "sampleLastName";
            string phone = "1234567890";
            string address = "123, sample street, sample city";

            //updating
            pv = this.client.Profile_GetMine();
            pv.Name = name;
            pv.LastName = LastName;
            pv.Phone = phone;
            pv.Address = address;
            pv = this.client.Profile_UpdateMine(pv);

            //verifying
            if(pv.Name.Equals(name) && 
                pv.LastName.Equals(LastName) &&
                pv.Phone.Equals(phone) &&
                pv.Address.Equals(address)
                )
            {
                Log("profile update is doing well");
            }
            else
            {
                throw new Exception("profile update failed");
            }

            this.client.UnregisterProfile();
        }

    }
}

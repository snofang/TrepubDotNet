using Trepub.Common;
using Trepub.Web.API.Test.TestClientBSO;
using Trepub.Web.API.ViewModels;
using Trepub.Web.API.ViewModels.Profile;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Trepub.Web.API.Test.Common
{
    public class AdminTestClient : TestClient
    {
        public AdminTestClient(string baseUrl) : base(baseUrl, new ProfileView()
        {
            //BirthDate = new PersianCalendar().ToDateTime(1365, 6, 12, 0, 0, 0, 0),
            //MobileNumber = "09999999999",
            //NationalCode = "administrator",
            Password = "Trepub123",
            UserId = "administrator",
        }
        )
        { }

        public AdminTestClient() : this(null) { }


        protected static AdminTestClient instance = null;

        public static AdminTestClient Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AdminTestClient();
                    instance.Login();
                }
                return instance;
            }
        }





        public static ProfileView GetSampleStaffProfile()
        {
            return new ProfileView()
            {
                ////Mrs Hanieh Rahimi Hamrah
                //BirthDate = new PersianCalendar().ToDateTime(1371, 11, 10, 0, 0, 0, 0),
                MobileNumber = "01234567890",
                //NationalCode = "3860530216",
                //Password = "Trepub123",
                UserId = "3860530216",
                Password = "Trepub123",
            };

        }


    }
}

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
            Password = "trepub123",
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

    }
}

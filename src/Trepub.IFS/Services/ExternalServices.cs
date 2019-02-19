using Trepub.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Globalization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Trepub.Common.Facade;

namespace Trepub.IFS.Services
{
    public class ExternalServices : IExternalServices
    {

        protected ILogger<ExternalServices> logger;
        protected IConfiguration configuration;


        public ExternalServices()
        {
            this.logger = FacadeProvider.IfsFacade.GetLogger<ExternalServices>();
            this.configuration = FacadeProvider.IfsFacade.GetConfigurationRoot();
        }

    }

}

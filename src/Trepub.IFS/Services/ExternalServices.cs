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
        protected IrSmsSenderService irSmsSenderService;


        public ExternalServices()
        {
            this.logger = FacadeProvider.IfsFacade.GetLogger<ExternalServices>();
            this.configuration = FacadeProvider.IfsFacade.GetConfigurationRoot();
            this.irSmsSenderService = new IrSmsSenderService();
        }

        public void SendSMS(string number, string message)
        {
            Boolean sendSms;
            if (!Boolean.TryParse(configuration.GetSection("SMS")["SendSMS"], out sendSms))
            {
                sendSms = true;
            }
            if (sendSms)
            {
                this.irSmsSenderService.SendSMS(number, message);
                logger.LogInformation($"SMS SENT. number={number}, message={message}");
            }
            else
            {
                logger.LogWarning($"SMS DISABLED. number={number}, message={message}");
            }
        }

    }

}

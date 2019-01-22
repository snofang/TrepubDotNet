using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using Trepub.Common;
using Trepub.Common.Exceptions;
using Trepub.Common.Facade;
using Trepub.Common.Interfaces;
using Trepub.Common.Utils;

namespace Trepub.IFS.Services
{
    public class IrSmsSenderService : ISmsSender
    {
        protected ILogger<ExternalServices> logger;
        protected IConfiguration configuration;
        protected string _tokenKey;
        protected DateTime _tokenDate;
        protected string _lineNumber;

        public IrSmsSenderService()
        {
            this.logger = FacadeProvider.IfsFacade.GetLogger<ExternalServices>();
            this.configuration = FacadeProvider.IfsFacade.GetConfigurationRoot();
            this._tokenDate = DateTime.MinValue;
            this._lineNumber = null;
        }


        public void SendSMS(string number, string message)
        {
            using (var c = new HttpClient())
            {
                var req = new SendSmsRequest()
                {
                    Messages = new List<string>() { message },
                    MobileNumbers = new List<string>() { number },
                    LineNumber = LineNumber
                };
                var url  = this.configuration.GetSection("SMS")["irSmsSender_sendSmsUrl"];
                c.DefaultRequestHeaders.Add("x-sms-ir-secure-token", TokenKey);
                var res = c.Post<SendSmsRequest, SendSmsResponse>(req, url);
                if (!res.IsSuccessful)
                {
                    throw new AppException(ErrorConstants.SMS_SEND_FAILED, res.Message);
                }
            }
        }

        protected string GetTocken()
        {
            var url = this.configuration.GetSection("SMS")["irSmsSender_tokenUrl"];
            using (var c = new HttpClient())
            {
                var req = new TokenRequest();
                req.UserApiKey = this.configuration.GetSection("SMS")["irSmsSender_userApiKey"];
                req.SecretKey = this.configuration.GetSection("SMS")["irSmsSender_secretKey"];
                var res = c.Post<TokenRequest, TokenResponse>(req, url);
                if (!res.IsSuccessful)
                {
                    throw new AppException(ErrorConstants.SMS_AUTH_FAILED, res.Message);
                }
                return res.TokenKey;
            }
        }

        public string GetSmsLine()
        {
            using (var c = new HttpClient())
            {
                var url = this.configuration.GetSection("SMS")["irSmsSender_getSmsLineUrl"];
                c.DefaultRequestHeaders.Add("x-sms-ir-secure-token", TokenKey);
                var res = c.Get<SmsLinesResponse>(url);
                if (!res.IsSuccessful)
                {
                    throw new AppException(ErrorConstants.SMS_SEND_FAILED, res.Message);
                }
                return res.SMSLines.First().LineNumber;
            }
        }

        protected string TokenKey
        {
            get
            {
                if (DateTime.Now.Subtract(this._tokenDate).Ticks > new TimeSpan(0, 28, 0).Ticks)
                {
                    this._tokenKey = GetTocken();
                    this._tokenDate = DateTime.Now;
                }
                return this._tokenKey;
            }
        }

        protected string LineNumber
        {
            get
            {
                if (string.IsNullOrEmpty(this._lineNumber))
                {
                    this._lineNumber = GetSmsLine();
                }
                return _lineNumber;
            }
        }

        protected class TokenRequest
        {
            public string UserApiKey { get; set; }
            public string SecretKey { get; set; }
        }
        protected class TokenResponse
        {
            public bool IsSuccessful { get; set; }
            public string Message { get; set; }
            public string TokenKey { get; set; }
        }
        protected class SendSmsRequest
        {
            public List<string> Messages { get; set; }
            public List<string> MobileNumbers { get; set; }
            public string LineNumber { get; set; } = "3000123456789";
            public string SendDateTime { get; set; } = string.Empty;
            public string CanContinueInCaseOfError { get; set; } = "false";
        }

        protected class SendSmsResponse
        {
            public List<SendSmsResponseId> Ids { get; set; }
            public string BatchKey { get; set; }
            public bool IsSuccessful { get; set; }
            public string Message { get; set; }

        }

        protected class SendSmsResponseId
        {
            public int ID { get; set; }
            public string MobileNo { get; set; }
        }

        protected class SmsLinesRequest
        {

        }

        protected class SmsLinesResponse
        {
            public List<SmsLine> SMSLines { get; set; }
            public bool IsSuccessful { get; set; }
            public string Message { get; set; }
        }

        protected class SmsLine
        {
            public int ID { get; set; }
            public string LineNumber { get; set; }
        }
    }
}

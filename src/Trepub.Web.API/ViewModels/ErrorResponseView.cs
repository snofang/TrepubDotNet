using Trepub.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Trepub.Web.API.ViewModels
{
    public class ErrorResponseView
    {
        public int StatusCode { get; }

        //[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ErrorCode { get; }

        public string Message { get; }

        public ErrorResponseView(int statusCode, string message, string errorCode)
        {
            StatusCode = statusCode;
            ErrorCode = errorCode ?? ErrorConstants.NOT_SPECIFIED;
            Message = message;
        }

    }
}

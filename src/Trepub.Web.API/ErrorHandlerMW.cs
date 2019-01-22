using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Trepub.Common;
using Trepub.Common.Exceptions;
using Trepub.Common.Utils;
using Trepub.Web.API.ViewModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trepub.Web.API
{
    public class ErrorHandlerMW
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlerMW> _logger;

        public ErrorHandlerMW(RequestDelegate next, ILogger<ErrorHandlerMW> logger)
        {
            _next = next;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Invoke(HttpContext context)
        {
            string errorMessage = string.Empty;
            string errorCode = ErrorConstants.NOT_SPECIFIED;
            try
            {
                await _next.Invoke(context);
            }
            catch (Exception ex)
            {
                StringBuilder sb = new StringBuilder(" *** Trepub Request ERROR *** ");
                AppException nsExceptioin = ex as AppException;
                if (nsExceptioin != null)
                {
                    sb.Append("\n *** ERROR CODE *** :\n");
                    errorCode = nsExceptioin.ErrorCode;
                    sb.Append(nsExceptioin.ErrorCode);
                    sb.Append("\n *** OCCURED EXCEPTION ***:\n");
                    if (nsExceptioin.ErrorCode.Equals(ErrorConstants.NOT_SPECIFIED))
                    {
                        sb.Append(ex.ToString());
                    }
                    else
                    {
                        sb.Append(ex.Message);
                    }
                    if (nsExceptioin.Object != null)
                    {
                        sb.Append("\n *** OBJECT DUMP *** :\n");
                        sb.Append(ObjectDumper.Dump(nsExceptioin.Object));
                    }
                }
                else
                {
                    sb.Append("\n *** OCCURED EXCEPTION ***:\n");
                    sb.Append(ex.ToString());
                }
                errorMessage = ex.Message;
                _logger.LogError(sb.ToString());
                context.Response.StatusCode = 409;
            }


            if (context.Response.StatusCode == 409 && !context.Response.HasStarted)
            {
                context.Response.ContentType = "application/json";
                var response = new ErrorResponseView(context.Response.StatusCode, errorMessage, errorCode);
                var json = JsonConvert.SerializeObject(response, new JsonSerializerSettings
                {
                    ContractResolver = new DefaultContractResolver()
                });
                await context.Response.WriteAsync(json);
            }
        }

    }
}

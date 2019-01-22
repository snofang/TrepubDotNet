using System;
using System.Collections.Generic;
using System.Text;

namespace Trepub.Common.Exceptions
{
    public class AppException : ApplicationException
    {

        public string ErrorCode { get; private set; }
        public object Object { get; private set; }
        public AppException(string message) : this(ErrorConstants.NOT_SPECIFIED, message, null) { }
        public AppException(string message, Exception innerException) : this(ErrorConstants.NOT_SPECIFIED, message, innerException) { }
        public AppException(string errorCode, string message) : this(errorCode, message, null) { }
        public AppException(string errorCode, string message, Exception innerException) :base(message, innerException)
        {
            this.ErrorCode = errorCode;
        }
        public AppException(string errorCode, string message, object obj): this(errorCode, message)
        {
            this.Object = obj;
        }

        public AppException(string errorCode, string message, object obj, Exception innerException) : this(errorCode,  message, innerException)
        {
            this.Object = obj;
        }

    }
}

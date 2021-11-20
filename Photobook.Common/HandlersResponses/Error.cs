using System;
using System.Net;

namespace Photobook.Common.HandlersResponses
{
    public class Error
    {
        public Guid Code { get; }
        public string Message { get; }
        public HttpStatusCode StatusCode { get; }

        public Error(Guid code, string message, HttpStatusCode statusCode)
        {
            Code = code;
            Message = message;
            StatusCode = statusCode;
        }
    }
}

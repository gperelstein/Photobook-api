using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Photobook.Common.HandlersResponses
{
    public class Response<T>
    {
        public bool Success { get; }
        public Error Error { get; }
        public T Value { get; }
        public HttpStatusCode StatusCode { get; }

        public Response(T value)
        {
            Success = true;
            Value = value;
            StatusCode = HttpStatusCode.OK;
        }

        public Response(Error error)
        {
            Success = false;
            Error = error;
            StatusCode = error.StatusCode;
        }
    }
}

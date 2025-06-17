using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TransactionService.Models.Models.Response
{
    public abstract class HttpDataResultBase
    {
        public HttpStatusCode StatusCode { get; set; }

        public string Message { get; set; }

        public IEnumerable<ProblemError> Errors { get; set; }

        public HttpDataResultBase()
        {
        }

        public HttpDataResultBase(HttpDataResultBase httpDataResult)
            : this(httpDataResult.StatusCode, httpDataResult.Message, httpDataResult.Errors)
        {
        }

        public HttpDataResultBase(HttpStatusCode statusCode, string message = null, IEnumerable<ProblemError> errors = null)
        {
            StatusCode = statusCode;
            Message = message;
            Errors = errors;
        }
    }
}
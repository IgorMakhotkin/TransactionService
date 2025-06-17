using System.Net;

namespace TransactionService.Models.Models.Response
{
    public class HttpDataResult<TResponseModel> : HttpDataResultBase
    {
        public TResponseModel Data { get; set; }

        public HttpDataResult()
        {
        }

        public HttpDataResult(HttpDataResultBase httpDataResult)
            : base(httpDataResult)
        {
        }

        public HttpDataResult(HttpStatusCode statusCode)
            : base(statusCode)
        {
        }

        public HttpDataResult(HttpStatusCode statusCode, string message)
            : base(statusCode, message)
        {
        }

        public HttpDataResult(HttpStatusCode statusCode, string message = null, IEnumerable<ProblemError> errors = null)
            : base(statusCode, message, errors)
        {
        }

        public HttpDataResult(TResponseModel data, HttpStatusCode statusCode, string message)
            : this(data, statusCode, message, (IEnumerable<ProblemError>)null)
        {
        }

        public HttpDataResult(TResponseModel data, HttpStatusCode statusCode, string message = null, IEnumerable<ProblemError> errors = null)
            : this(statusCode, message, errors)
        {
            Data = data;
        }
    }
}

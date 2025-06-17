using System.Net;
using TransactionService.Models.Models.Response;

namespace TransactionService.Extensions
{
    public static class HttpExtensions
    {
        public static bool IsSuccessStatusCode(this HttpStatusCode statusCode)
        {
            if (statusCode >= HttpStatusCode.OK)
            {
                return statusCode <= (HttpStatusCode)299;
            }

            return false;
        }

        public static bool IsFailedStatusCode(this HttpStatusCode statusCode)
        {
            return !statusCode.IsSuccessStatusCode();
        }

        public static bool IsSuccessStatusCode(this HttpDataResultBase httpDataResult)
        {
            return httpDataResult.StatusCode.IsSuccessStatusCode();
        }
    }
}

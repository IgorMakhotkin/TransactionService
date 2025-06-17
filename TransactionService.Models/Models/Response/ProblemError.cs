using System.Text.Json.Serialization;

namespace TransactionService.Models.Models.Response
{
    public class ProblemError
    {
        [JsonIgnore]
        public int ErrorCode { get; }

        public string Message { get; set; }

        public ProblemError()
        {
        }

        public ProblemError(int errorCode, string message)
        {
            if (errorCode < 1 || errorCode > 999)
            {
                throw new ArgumentOutOfRangeException("errorCode", "Код ошибки должен находиться в диапазоне от 1 до 999");
            }

            ErrorCode = errorCode;
            Message = message;
        }
    }
}

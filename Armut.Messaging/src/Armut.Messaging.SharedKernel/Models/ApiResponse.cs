using System.Net;

namespace Armut.Messaging.SharedKernel.Models
{
    public class ApiResponse
    {
        public string? Error { get; set; }

        public bool IsSuccess { get; set; }

        public object? Result { get; set; }

        public HttpStatusCode StatusCode { get; set; }

        public ApiResponse(string? error, bool isSuccess, object? result, HttpStatusCode statusCode)
        {
            Error = error;
            IsSuccess = isSuccess;
            Result = result;
            StatusCode = statusCode;
        }

        public static ApiResponse Create(HttpStatusCode statusCode, object? body, string? error)
        {
            return new ApiResponse(error, string.IsNullOrWhiteSpace(error), body, statusCode);
        }
    }
}

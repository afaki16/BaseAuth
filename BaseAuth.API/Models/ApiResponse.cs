namespace BaseAuth.API.Models
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public T Data { get; set; }
        public string Message { get; set; }
        public List<string> Errors { get; set; } = new List<string>();

        public static ApiResponse<T> SuccessResult(T data, string message = null)
        {
            return new ApiResponse<T>
            {
                Success = true,
                Data = data,
                Message = message ?? "Operation completed successfully"
            };
        }

        public static ApiResponse<T> ErrorResult(string error)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Errors = new List<string> { error }
            };
        }

        public static ApiResponse<T> ErrorResult(List<string> errors)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Errors = errors
            };
        }
    }

    public class ApiResponse : ApiResponse<object>
    {
        public static ApiResponse SuccessResult(string message = null)
        {
            return new ApiResponse
            {
                Success = true,
                Message = message ?? "Operation completed successfully"
            };
        }

        public new static ApiResponse ErrorResult(string error)
        {
            return new ApiResponse
            {
                Success = false,
                Errors = new List<string> { error }
            };
        }

        public new static ApiResponse ErrorResult(List<string> errors)
        {
            return new ApiResponse
            {
                Success = false,
                Errors = errors
            };
        }
    }
} 
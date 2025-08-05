using System.Collections.Generic;
using System.Linq;

namespace BaseAuth.Domain.Common
{
    public class Result
    {
        public bool IsSuccess { get; private set; }
        public string Error { get; private set; }
        public List<string> Errors { get; private set; }

        protected Result(bool isSuccess, string error)
        {
            IsSuccess = isSuccess;
            Error = error;
            Errors = new List<string>();
            if (!string.IsNullOrEmpty(error))
                Errors.Add(error);
        }

        protected Result(bool isSuccess, List<string> errors)
        {
            IsSuccess = isSuccess;
            Errors = errors ?? new List<string>();
            Error = errors?.FirstOrDefault();
        }

        public static Result Success() => new Result(true, string.Empty);
        public static Result Failure(string error) => new Result(false, error);
        public static Result Failure(List<string> errors) => new Result(false, errors);

        public static Result<T> Success<T>(T data) => new Result<T>(data, true, string.Empty);
        public static Result<T> Failure<T>(string error) => new Result<T>(default(T), false, error);
        public static Result<T> Failure<T>(List<string> errors) => new Result<T>(default(T), false, errors);
    }

    public class Result<T> : Result
    {
        public T Data { get; private set; }

        protected internal Result(T data, bool isSuccess, string error) : base(isSuccess, error)
        {
            Data = data;
        }

        protected internal Result(T data, bool isSuccess, List<string> errors) : base(isSuccess, errors)
        {
            Data = data;
        }
    }
} 
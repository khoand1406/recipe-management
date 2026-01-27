using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeMgt.Application.DTOs
{
    public class Result
    {
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public string Error { get; }

        protected Result(bool isSuccess, string error)
        {
            if (isSuccess && !string.IsNullOrEmpty(error))
                throw new InvalidOperationException();

            if (!isSuccess && string.IsNullOrEmpty(error))
                throw new InvalidOperationException();

            IsSuccess = isSuccess;
            Error = error;
        }

        public static Result Success()
            => new(true, string.Empty);

        public static Result Failure(string error)
            => new(false, error);
    }

    public class Result<T> : Result
    {
        public T? Value { get; }

        private Result(T value) : base(true, string.Empty)
        {
            Value = value;
        }

        private Result(string error) : base(false, error)
        {
            Value = default;
        }

        public static Result<T> Success(T value)
            => new(value);

        public static new Result<T> Failure(string error)
            => new(error);
    }
}

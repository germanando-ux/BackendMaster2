using System;
using System.Collections.Generic;
using System.Text;

namespace BackendMaster2.Shared.Common
{
    /// <summary>
    /// Represemta un resultado de una operación que puede ser exitosa o fallida.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Result<T>
    {
        public bool IsSuccess { get; }
        public T? Value { get; }
        public string Error { get; }

        private Result(bool isSuccess, T? value, string error)
        {
            IsSuccess = isSuccess;
            Value = value;
            Error = error;
        }

        public static Result<T> Success(T value)
        {
            return new Result<T>(true, value, string.Empty);
        }
        public static Result<T> Failure(string error) => new(false, default, error);

        // Método de conveniencia para APIs (mapea el resultado a respuestas HTTP)
        public TResult Match<TResult>(Func<T, TResult> onSuccess, Func<string, TResult> onFailure)
        {
            return IsSuccess ? onSuccess(Value!) : onFailure(Error);
        }
    }
}

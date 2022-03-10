using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusinessLayer.Models
{
    public class ServiceResult
    {
        public bool IsSuccess => Errors == null || !Errors.Any();

        public IEnumerable<string> Errors { get; set; }


        public static ServiceResult Success() => new ServiceResult();

        public static ServiceResult Error(params string[] errors) => new ServiceResult { Errors = errors };

        public static ServiceResult<T> Success<T>(T result) where T : class
        {
            return new ServiceResult<T> { Result = result };
        }
    }

    public class ServiceResult<T> : ServiceResult where T :class
    {
        public T Result { get; set; }

        public static ServiceResult<T> Error(params string[] errors) => new ServiceResult<T> { Errors = errors };

    }
}

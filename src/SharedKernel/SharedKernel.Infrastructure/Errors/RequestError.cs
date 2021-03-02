using CSharpFunctionalExtensions;
using System.Collections.Generic;

namespace SharedKernel.Infrastructure.Errors
{
    public abstract class RequestError : ValueObject
    {
        public string Code { get; protected set; }
        public dynamic Message { get; protected set; }

        protected RequestError() { }


        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Code;
        }
    }
}

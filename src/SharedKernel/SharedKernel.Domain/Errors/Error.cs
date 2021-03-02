using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;

namespace SharedKernel.Domain.Errors
{
    /// <summary>
    /// Combineable error.
    /// </summary>
    public sealed class Error : ICombine
    {
        private readonly List<string> _errors;
        public Error(string error)
            : this(new List<string> { error })
        {
        }

        private Error(List<string> errors)
        {
            _errors = errors ?? throw new ArgumentNullException(nameof(errors));
        }

        public IReadOnlyCollection<string> Errors => _errors;

        public ICombine Combine(ICombine value)
        {
            var errorMsg = value as Error;
            var _errorList = new List<string>(errorMsg._errors);
            _errorList.AddRange(_errors);
            return new Error(_errorList);
        }
    }
}

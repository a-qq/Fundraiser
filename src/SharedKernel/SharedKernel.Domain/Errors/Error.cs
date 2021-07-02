using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;

namespace SharedKernel.Domain.Errors
{
    /// <summary>
    ///     Combineable error.
    /// </summary>
    public sealed class Error : ICombine
    {
        private readonly HashSet<string> _errors;

        public Error(string error)
            : this(string.IsNullOrWhiteSpace(error) ? new HashSet<string>() : new HashSet<string> { error })
        {
        }

        private Error(HashSet<string> errors)
        {
            _errors = errors ?? throw new ArgumentNullException(nameof(errors));
        }

        public IReadOnlyCollection<string> Errors => _errors;

        public ICombine Combine(ICombine value)
        {
            if (!(value is Error errorMsg))
                throw new InvalidCastException(nameof(ICombine) + " -> " + nameof(Error));

            var errorList = new HashSet<string>(errorMsg._errors, errorMsg._errors.Comparer);
            foreach (var error in _errors)
                errorList.Add(error);
            
            return new Error(errorList);
        }
    }
}
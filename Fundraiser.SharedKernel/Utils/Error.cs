using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;

namespace Fundraiser.SharedKernel.Utils
{
    public class Error : ICombine
    {
        private readonly List<string> _errors;
        public Error(string error)
            : this(new List<string> { error })
        {
        }

        public Error(List<string> errors)
        {
            _errors = errors ?? throw new ArgumentNullException(nameof(errors));
        }

        public IReadOnlyCollection<string> Errors => _errors;

        public ICombine Combine(ICombine value)
        {
            var errorMsg = value as Error;
            var _errorList = new List<string>(errorMsg._errors);
            _errorList.AddRange(this._errors);
            return new Error(_errorList);
        }
    }
}

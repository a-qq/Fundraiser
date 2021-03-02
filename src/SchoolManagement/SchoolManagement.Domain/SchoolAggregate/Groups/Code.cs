using System;
using System.Collections.Generic;
using CSharpFunctionalExtensions;

namespace SchoolManagement.Domain.SchoolAggregate.Groups
{
    public class Code : ValueObject
    {
        public Code(Number number, Sign sign)
        {
            if (number is null)
                throw new ArgumentNullException(nameof(number));

            if (sign is null)
                throw new ArgumentNullException(nameof(sign));

            Value = number + sign;
        }

        public string Value { get; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value.ToLower();
        }

        public static implicit operator string(Code code)
        {
            return code.Value;
        }

        public override string ToString()
        {
            return Value;
        }
    }
}
using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;

namespace SchoolManagement.Domain.SchoolAggregate.Groups
{
    public class Code : ValueObject
    {
        public string Value { get; }

        public Code(Number number, Sign sign)
        {
            if (number is null)
                throw new ArgumentNullException(nameof(number));

            if (sign is null)
                throw new ArgumentNullException(nameof(sign));

            Value = number + sign;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value.ToLower();
        }

        public static implicit operator string(Code code)
        {
            return code.Value.ToString();
        }

        public override string ToString()
        {
            return this.Value;
        }

    }
}

using CSharpFunctionalExtensions;
using System.Collections.Generic;

namespace SchoolManagement.Core.SchoolAggregate.Schools
{
    public class YearsOfEducation : ValueObject
    {
        private static int MinValue => 2;
        private static int MaxValue => 4;
        public byte Value { get; }

        private YearsOfEducation(byte yearsOfEducation)
        {
            Value = yearsOfEducation;
        }
        public static Result<YearsOfEducation> Create(int yearsOfEducation)
        {
            Result validation = Validate(yearsOfEducation);
            if (validation.IsFailure)
                return validation.ConvertFailure<YearsOfEducation>();

            return new YearsOfEducation((byte)yearsOfEducation);
        }

        public static Result Validate(int yearsOfEducation, string propertyName = nameof(YearsOfEducation))
        {
            if (yearsOfEducation < MinValue)
                return Result.Failure($"{propertyName} is required and must be at least {MinValue}!");

            if (yearsOfEducation > MaxValue)
                return Result.Failure($"{propertyName} can not be greater than {MaxValue}!");

            return Result.Success();
        }

        public static implicit operator string(YearsOfEducation yearsOfEducation)
        {
            return yearsOfEducation.Value.ToString();
        }

        public static implicit operator int(YearsOfEducation yearsOfEducation)
        {
            return yearsOfEducation.Value;
        }

        public override string ToString()
        {
            return this.Value.ToString();
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}

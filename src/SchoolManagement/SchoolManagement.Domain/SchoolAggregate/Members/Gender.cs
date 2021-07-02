using System;
using System.Collections.Generic;
using CSharpFunctionalExtensions;
using SK = SharedKernel.Domain.Constants;

namespace SchoolManagement.Domain.SchoolAggregate.Members
{
    public class Gender : ValueObject
    {
        public static Gender Male = Create(SK.Gender.Male.ToString()).Value;
        public static Gender Female = Create(SK.Gender.Female.ToString()).Value;

        private Gender(SK.Gender value)
        {
            Value = value;
        }

        public SK.Gender Value { get; }

        public static Result<Gender> Create(string gender, string propertyName = nameof(Gender))
        {
            var validationResult = ValidateAndConvert(gender, propertyName);

            if (validationResult.IsFailure)
                return validationResult.ConvertFailure<Gender>();

            return Result.Success(new Gender(validationResult.Value));
        }

        public static Result<SK.Gender> ValidateAndConvert(string gender, string propertyName = nameof(Gender))
        {
            if (string.IsNullOrWhiteSpace(gender))
                return Result.Failure<SK.Gender>($"{propertyName} is required!");

            gender = gender.Trim();

            if (!Enum.TryParse(gender, true, out SK.Gender holder))
                return Result.Failure<SK.Gender>($"{propertyName} is invalid!");

            return Result.Success(holder);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public static implicit operator string(Gender gender)
        {
            return gender.Value.ToString();
        }
    }
}
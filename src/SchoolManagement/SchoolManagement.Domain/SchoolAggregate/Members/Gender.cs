﻿using System;
using System.Collections.Generic;
using CSharpFunctionalExtensions;

namespace SchoolManagement.Domain.SchoolAggregate.Members
{
    public class Gender : ValueObject
    {
        internal static Gender Male = Create(GenderEnum.Male.ToString()).Value;
        internal static Gender Female = Create(GenderEnum.Female.ToString()).Value;

        private Gender(GenderEnum value)
        {
            Value = value;
        }

        public GenderEnum Value { get; }

        public static Result<Gender> Create(string gender)
        {
            var validationResult = ValidateAndConvert(gender);

            if (validationResult.IsFailure)
                return validationResult.ConvertFailure<Gender>();

            return Result.Success(new Gender(validationResult.Value));
        }

        public static Result<GenderEnum> ValidateAndConvert(string gender, string propertyName = nameof(Gender))
        {
            if (string.IsNullOrWhiteSpace(gender))
                return Result.Failure<GenderEnum>($"{propertyName} is required!");

            gender = gender.Trim();

            if (!Enum.TryParse(gender, true, out GenderEnum holder))
                return Result.Failure<GenderEnum>($"{propertyName} is invalid!");

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

    public enum GenderEnum
    {
        Male = 1,
        Female = 2
    }
}
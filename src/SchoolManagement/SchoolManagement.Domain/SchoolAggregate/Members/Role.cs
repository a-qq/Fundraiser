using CSharpFunctionalExtensions;
using SharedKernel.Domain.Constants;
using System;
using System.Collections.Generic;

namespace SchoolManagement.Domain.SchoolAggregate.Members
{
    public class Role : ValueObject
    {
        public static readonly Role Headmaster = Create(SchoolRole.Headmaster.ToString()).Value;
        public static readonly Role Teacher = Create(SchoolRole.Teacher.ToString()).Value;
        public static readonly Role Student = Create(SchoolRole.Student.ToString()).Value;

        private Role(SchoolRole value)
        {
            Value = value;
        }

        public SchoolRole Value { get; }

        public static Result<Role> Create(string role, string propertyName = nameof(Role))
        {
            var validationResult = ValidateAndConvert(role, propertyName);

            if (validationResult.IsFailure)
                return validationResult.ConvertFailure<Role>();

            return Result.Success(new Role(validationResult.Value));
        }

        public static Result<SchoolRole> ValidateAndConvert(string role, string propertyName = nameof(Role))
        {
            if (string.IsNullOrWhiteSpace(role))
                return Result.Failure<SchoolRole>($"{propertyName} is required!");

            role = role.Trim();

            if (!Enum.TryParse(role, true, out SchoolRole holder))
                return Result.Failure<SchoolRole>($"{propertyName} is invalid!");

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

        public static implicit operator string(Role role)
        {
            return role.Value.ToString();
        }

        public static bool operator >(Role a, Role b)
        {
            return a.Value > b.Value;
        }

        public static bool operator <(Role a, Role b)
        {
            return a.Value < b.Value;
        }

        public static bool operator <=(Role a, Role b)
        {
            return a.Value <= b.Value;
        }

        public static bool operator >=(Role a, Role b)
        {
            return a.Value >= b.Value;
        }
    }
}
using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;

namespace SchoolManagement.Core.SchoolAggregate.Members
{
    public class Role : ValueObject
    {
        public static readonly Role Headmaster = Create(RoleEnum.Headmaster.ToString()).Value;
        public static readonly Role Teacher = Create(RoleEnum.Teacher.ToString()).Value;
        public static readonly Role Student = Create(RoleEnum.Student.ToString()).Value;

        public RoleEnum Value { get; }
        private Role(RoleEnum value)
        {
            Value = value;
        }

        public static Result<Role> Create(string role)
        {
            Result<RoleEnum> validationResult = ValidateAndConvert(role);

            if (validationResult.IsFailure)
                return validationResult.ConvertFailure<Role>();

            return Result.Success(new Role(validationResult.Value));
        }

        public static Result<RoleEnum> ValidateAndConvert(string role, string propertyName = nameof(Role))
        {
            if (string.IsNullOrWhiteSpace(role))
                return Result.Failure<RoleEnum>($"{propertyName} is required!");

            role = role.Trim();

            if (!Enum.TryParse(role, true, out RoleEnum holder))
                return Result.Failure<RoleEnum>($"{propertyName} is invalid!");

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

    public enum RoleEnum
    {
        Student = 1,
        Teacher = 2,
        Headmaster = 3
    }
}


﻿using CSharpFunctionalExtensions;
using System.Collections.Generic;

namespace SchoolManagement.Core.SchoolAggregate.Schools
{
    public class GroupMembersLimit : ValueObject
    {
        private static int MinValue => 1;
        private static int MaxValue => 500;
        public ushort? Value { get; }

        private GroupMembersLimit(ushort? number)
        {
            Value = number;
        }

        public static Result<GroupMembersLimit> Create(int? limit)
        {
            Result validation = Validate(limit);
            if (validation.IsFailure)
                return validation.ConvertFailure<GroupMembersLimit>();

            return new GroupMembersLimit((ushort?)limit);
        }

        public static Result Validate(int? number, string propertyName = nameof(GroupMembersLimit))
        {
            if (number.HasValue && number < MinValue)
                return Result.Failure($"{propertyName} is required and must be at least {MinValue}!");

            if (number.HasValue && number > MaxValue)
                return Result.Failure($"{propertyName} can not be greater than {MaxValue}!");

            return Result.Success();
        }

        public static implicit operator string(GroupMembersLimit limit)
        {
            return limit.Value.ToString();
        }

        public static implicit operator ushort?(GroupMembersLimit limit)
        {
            return limit.Value;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
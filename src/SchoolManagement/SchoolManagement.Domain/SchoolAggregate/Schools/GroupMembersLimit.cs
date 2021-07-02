using System.Collections.Generic;
using CSharpFunctionalExtensions;

namespace SchoolManagement.Domain.SchoolAggregate.Schools
{
    public class GroupMembersLimit : ValueObject
    {
        private GroupMembersLimit(ushort? number)
        {
            Value = number;
        }

        public static int MinValue => 1;
        public static int MaxValue => 500;
        public ushort? Value { get; }

        public static Result<GroupMembersLimit> Create(int? limit, string propertyName = nameof(GroupMembersLimit))
        {
            var validation = Validate(limit, propertyName);
            if (validation.IsFailure)
                return validation.ConvertFailure<GroupMembersLimit>();

            return new GroupMembersLimit((ushort?) limit);
        }

        public static Result Validate(int? number, string propertyName = nameof(GroupMembersLimit))
        {
            if (!(number is null) && number < MinValue)
                return Result.Failure($"{propertyName} must be at least {MinValue}!");

            if (!(number is null) && number > MaxValue)
                return Result.Failure($"{propertyName} can not be greater than {MaxValue}!");

            return Result.Success();
        }

        public static implicit operator string(GroupMembersLimit limit)
        {
            return limit?.Value?.ToString() ?? string.Empty;
        }

        public override string ToString()
        {
            return this?.Value?.ToString() ?? string.Empty;
        }

        public static implicit operator int?(GroupMembersLimit limit)
        {
            return limit?.Value;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
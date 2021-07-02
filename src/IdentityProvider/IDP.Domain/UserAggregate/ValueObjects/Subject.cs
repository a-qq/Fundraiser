using CSharpFunctionalExtensions;
using System.Collections.Generic;

namespace IDP.Domain.UserAggregate.ValueObjects
{
    public class Subject : ValueObject
    {
        private const int MaxCharNumber = 450;
        public string Value { get; }

        private Subject(string value)
        {
            Value = value;  
        }

        public static Result<Subject> Create(string subject)
        {
            var validationResult = Validate(subject);

            if (validationResult.IsFailure)
                return validationResult.ConvertFailure<Subject>();

            return new Subject(subject);
        }

        public static Result Validate(string subject, string propertyName = nameof(Subject))
        {
            if (string.IsNullOrWhiteSpace(subject))
                return Result.Failure($"{propertyName} is required!");

            subject = subject.Trim();

            if (subject.Length > MaxCharNumber)
                return Result.Failure($"{propertyName} cannot be longer then {MaxCharNumber} characters!");

            return Result.Success();
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public override string ToString()
        {
            return Value;
        }

        public static implicit operator string(Subject subject)
        {
            return subject.Value;
        }
    }
}

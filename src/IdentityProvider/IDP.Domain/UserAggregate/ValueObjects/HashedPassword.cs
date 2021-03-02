using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using CSharpFunctionalExtensions;
using IDP.Domain.UserAggregate.Entities;
using Microsoft.AspNetCore.Identity;
using SharedKernel.Domain.Errors;

namespace IDP.Domain.UserAggregate.ValueObjects
{
    public class HashedPassword : ValueObject
    {
        private static readonly int MaxLength = 20;
        private static readonly int MinLength = 8;

        private HashedPassword(string value)
        {
            Value = value;
        }

        public string Value { get; }

        public static Result<HashedPassword, Error> Create(string password, IPasswordHasher<User> passwordHasher)
        {
            if (passwordHasher == null)
                throw new ArgumentNullException(nameof(passwordHasher));

            var validationResult = Validate(password);

            if (validationResult.IsFailure)
                validationResult.ConvertFailure<HashedPassword>();

            var hashedPassword = passwordHasher.HashPassword(null, password);

            return new HashedPassword(hashedPassword);
        }

        public static Result<bool, Error> Validate(string password, string propertyName = "Password")
        {
            if (string.IsNullOrWhiteSpace(password))
                return Result.Failure<bool, Error>(new Error($"{propertyName} is required"));

            return Result.Combine(
                Result.FailureIf(password.Length < MinLength, true,
                    new Error($"{propertyName} must have at least {MinLength} characters!")),
                Result.FailureIf(password.Length > MaxLength, true,
                    new Error($"{propertyName} must have at most {MaxLength} characters!")),
                Result.FailureIf(!Regex.IsMatch(password, @"[A-Z]+"), true,
                    new Error($"{propertyName} must contain at least one uppercase letter!")),
                Result.FailureIf(!Regex.IsMatch(password, @"[0-9]+"), true,
                    new Error($"{propertyName} must contain at least one number!")),
                Result.FailureIf(!Regex.IsMatch(password, @"[a-z]+"), true,
                    new Error($"{propertyName} must contain at least one lowercase letter!")),
                Result.FailureIf(!Regex.IsMatch(password, @"[~`!@#$%^&*()\-\\+=[\]{}'|/.,_?<>]+"), true,
                    new Error($"{propertyName} must contain at least one special character!")));
        }

        public static HashedPassword CheckHashAndConvert(string hashedPassword)
        {
            if (string.IsNullOrWhiteSpace(hashedPassword) ||
                !Convert.TryFromBase64String(hashedPassword, _ = new byte[hashedPassword.Length * 3 / 4], out _))
                throw new FormatException();

            return new HashedPassword(hashedPassword);
        }

        public static implicit operator string(HashedPassword password)
        {
            return password.Value;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
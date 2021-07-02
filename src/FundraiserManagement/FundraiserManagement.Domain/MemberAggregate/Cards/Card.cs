using System;
using System.Collections.Generic;
using Ardalis.GuardClauses;
using CSharpFunctionalExtensions;

namespace FundraiserManagement.Domain.MemberAggregate.Cards
{
    public class Card : ValueObject
    {
        public Number Number { get; }
        public Month Month { get; }
        public Year Year { get; }
        public Cvc Cvc { get; }

        private Card(Number number, Month month, Year year, Cvc cvc)
        {
            Number = number;
            Month = month;
            Year = year;
            Cvc = cvc;
        }

        public static Result<Card> Create(Number number, Month month, Year year, Cvc cvc,
            DateTimeOffset now, string propertyName = nameof(Card))
        {
            var validation = Validate(number, month, year, cvc, now, propertyName);
            if (validation.IsFailure)
                return validation.ConvertFailure<Card>();

            return Result.Success(new Card(number, month, year, cvc));
        }

        public static Result Validate(Number number, Month month, Year year, Cvc cvc,
            DateTimeOffset now, string propertyName = nameof(Card))
        {
            Guard.Against.Null(number, nameof(number));
            Guard.Against.Null(month, nameof(month));
            Guard.Against.Null(year, nameof(year));
            Guard.Against.Null(cvc, nameof(cvc));

            if (year == now.Year && month < now.Month && !(month == now.Month - 1 && now.Day < 5) || 
                (year < now.Year && !(year == now.Year - 1 && now.DayOfYear < 5) || (year > now.Year + 10)))
                return Result.Failure($"{propertyName} is outdated!");

            return Result.Success();
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Number;
            yield return Month;
            yield return Year;
            yield return Cvc;
        }
    }
}

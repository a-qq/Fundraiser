using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;

namespace IDP.Domain.UserAggregate.ValueObjects
{
    public class ExpirationDate : ValueObject
    {
        public static readonly ExpirationDate Infinite = new ExpirationDate(null);

        public DateTime? Date { get; }

        public bool IsExpired(DateTime now) => this != Infinite && Date < now;

        private ExpirationDate(DateTime? date)
        {
            Date = date;
        }

        public static Result<ExpirationDate> Create(DateTime date)
        {
            return Result.Success(new ExpirationDate(date));
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Date;
        }


        public static explicit operator ExpirationDate(DateTime? date)
        {
            if (date.HasValue)
                return Create(date.Value).Value;

            return Infinite;
        }

        public static implicit operator DateTime?(ExpirationDate date)
        {
            return date.Date;
        }

        public static bool operator <(DateTime? a, ExpirationDate b)
        {
            if (!b.Date.HasValue)
                return true;
            else
                return a < b.Date;
        }

        public static bool operator >(DateTime? a, ExpirationDate b)
        {
            if (!b.Date.HasValue)
                return false;
            else
                return a > b.Date;
        }

        public static bool operator <(ExpirationDate a, DateTime? b)
        {
            if (!a.Date.HasValue)
                return false;
            else
                return a.Date < b;
        }

        public static bool operator >(ExpirationDate a, DateTime? b)
        {
            if (!a.Date.HasValue)
                return true;
            else
                return a.Date > b;
        }

        public static bool operator <=(DateTime a, ExpirationDate b)
        {
            if (!b.Date.HasValue)
                return true;
            else
                return a <= b.Date;
        }

        public static bool operator >=(DateTime a, ExpirationDate b)
        {
            if (!b.Date.HasValue)
                return false;
            else
                return a >= b.Date;
        }

        public static bool operator <=(ExpirationDate a, DateTime b)
        {
            if (!a.Date.HasValue)
                return false;
            else
                return a.Date >= b;
        }

        public static bool operator >=(ExpirationDate a, DateTime b)
        {
            if (!a.Date.HasValue)
                return true;
            else
                return a.Date >= b;
        }
    }

}

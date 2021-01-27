using Fundraiser.SharedKernel.RequestErrors;
using System;

namespace Fundraiser.API.Controllers
{
    public class Envelope<T>
    {
        public T Result { get; }
        public RequestError Error { get; }
        public DateTime TimeGenerated { get; }

        protected internal Envelope(T result, RequestError error)
        {
            Result = result;
            Error = error;
            TimeGenerated = DateTime.UtcNow;
        }
    }

    public class Envelope : Envelope<string>
    {
        protected Envelope(RequestError error)
            : base(null, error)
        {
        }

        public static Envelope<T> Ok<T>(T result)
        {
            return new Envelope<T>(result, null);
        }

        public static Envelope Ok()
        {
            return new Envelope(null);
        }

        public static new Envelope Error(RequestError error)
        {
            return new Envelope(error);
        }
    }
}
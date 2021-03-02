using System;
using SharedKernel.Infrastructure.Errors;

namespace Backend.API.Controllers
{
    public class Envelope<T>
    {
        protected internal Envelope(T result, RequestError error)
        {
            Result = result;
            Error = error;
            TimeGenerated = DateTime.UtcNow;
        }

        public T Result { get; }
        public RequestError Error { get; }
        public DateTime TimeGenerated { get; }
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

        public static Envelope Error(RequestError error)
        {
            return new Envelope(error);
        }
    }
}
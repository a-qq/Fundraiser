using CSharpFunctionalExtensions;
using System.Collections.Generic;

namespace Fundraiser.SharedKernel.ResultErrors
{
    public sealed class RequestError : ValueObject
    {
        //private const string Separator = "||";

        public string Code { get; }
        public dynamic Message { get; }

        public RequestError(string code, dynamic message)
        {
            Code = code;
            Message = message;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Code;
        }
        //public string Serialize()
        //{
        //    return $"{Code}{Separator}{Message}";
        //}

        //public static RequestError Deserialize(string serialized)
        //{
        //    string[] data = serialized.Split(
        //        new[] { Separator },
        //        StringSplitOptions.RemoveEmptyEntries);

        //    if(data.Length >= 2)
        //        throw new Exception($"Invalid error serialization: '{serialized}'");

        //    return new RequestError(data[0], data[1]);
        //}
    }
}

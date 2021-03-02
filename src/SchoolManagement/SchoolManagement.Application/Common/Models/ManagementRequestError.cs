using SharedKernel.Domain.ValueObjects;
using SharedKernel.Infrastructure.Errors;
using System.Collections.Generic;

namespace SchoolManagement.Application.Common.Models
{
    internal sealed class ManagementRequestError : RequestError
    {
        private ManagementRequestError(string code, dynamic message)
        {
            Code = code;
            Message = message;
        }

        public static class Csv
        {
            public static RequestError DuplicateEmails(IEnumerable<Email> emails) =>
                new ManagementRequestError("invalid.csv.emails", $"Duplicate emails in input file: {string.Join(", ", emails)}");

            public static RequestError InvalidHeader(string message) =>
                 new ManagementRequestError("invalid.csv.header", message);
        }

    }
}

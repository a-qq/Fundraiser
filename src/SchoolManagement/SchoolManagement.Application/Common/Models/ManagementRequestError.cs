using System.Collections.Generic;
using SharedKernel.Domain.ValueObjects;
using SharedKernel.Infrastructure.Errors;

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
            public static RequestError DuplicateEmails(IReadOnlyCollection<Email> emails)
            {
                return new ManagementRequestError("invalid.csv.emails",
                    $"Duplicate emails in input file: {string.Join(", ", emails)}");
            }

            public static RequestError InvalidHeader(string message)
            {
                return new ManagementRequestError("invalid.csv.header", message);
            }
        }
    }
}